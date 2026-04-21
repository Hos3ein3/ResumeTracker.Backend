using ResumeTracker.Application.Abstractions.FileStorage;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Infrastructure.Settings;

namespace ResumeTracker.Infrastructure.FileStorage;

public sealed class MongoFileStorageService : IFileStorageService
{
    private readonly IFileRepository _fileRepository;
    private readonly FileStorageSettings _settings;

    public MongoFileStorageService(
        IFileRepository fileRepository,
        FileStorageSettings settings)
    {
        _fileRepository = fileRepository;
        _settings = settings;
    }

    public Task<ResumeFile?> GetFileMetadataAsync(
        string fileId,
        CancellationToken cancellationToken = default)
        => _fileRepository.GetByIdAsync(fileId, cancellationToken);

    public Task<IReadOnlyList<ResumeFile>> GetResumeFilesAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
        => _fileRepository.GetByResumeIdAsync(resumeId, cancellationToken);

    public async Task<string> SaveFileAsync(
        Guid resumeId,
        Guid userId,
        FileType fileType,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(fileName, contentType, fileStream.Length);

        // ── Build directory structure: BasePath/{userId}/{resumeId}/{fileType}/ ──
        var relativePath = Path.Combine(
            userId.ToString(),
            resumeId.ToString(),
            fileType.ToString().ToLowerInvariant());

        var physicalDirectory = Path.Combine(_settings.BasePath, relativePath);
        Directory.CreateDirectory(physicalDirectory);

        // ── Unique file name to avoid collisions ──
        var safeFileName = $"{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        var physicalPath = Path.Combine(physicalDirectory, safeFileName);
        var relativeFile = Path.Combine(relativePath, safeFileName);

        // ── Write to disk ──
        await using var fileOnDisk = File.Create(physicalPath);
        await fileStream.CopyToAsync(fileOnDisk, cancellationToken);

        // ── Persist metadata record in MongoDB ──
        var record = new ResumeFile
        {
            ResumeId = resumeId,
            UserId = userId,
            FileType = fileType,
            FileName = fileName,          // original name user uploaded
            ContentType = contentType,
            SizeBytes = fileStream.Length,
            PhysicalPath = physicalPath,      // absolute path on server
            RelativePath = relativeFile,      // relative path for URL serving
            UploadedAtUtc = DateTime.UtcNow
        };

        return await _fileRepository.AddAsync(record, cancellationToken);
    }

    public async Task DeleteFileAsync(
        string fileId,
        CancellationToken cancellationToken = default)
    {
        var record = await _fileRepository.GetByIdAsync(fileId, cancellationToken);
        if (record is null) return;

        // ── Delete physical file from disk ──
        if (File.Exists(record.PhysicalPath))
            File.Delete(record.PhysicalPath);

        // ── Remove MongoDB metadata record ──
        await _fileRepository.DeleteAsync(fileId, cancellationToken);
    }

    public async Task DeleteAllResumeFilesAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default)
    {
        var records = await _fileRepository.GetByResumeIdAsync(resumeId, cancellationToken);

        foreach (var record in records)
        {
            if (File.Exists(record.PhysicalPath))
                File.Delete(record.PhysicalPath);
        }

        await _fileRepository.DeleteByResumeIdAsync(resumeId, cancellationToken);
    }

    public async Task<(Stream Stream, ResumeFile Metadata)> DownloadFileAsync(
        string fileId,
        CancellationToken cancellationToken = default)
    {
        var record = await _fileRepository.GetByIdAsync(fileId, cancellationToken);

        if (record is null)
            throw new FileNotFoundException($"File metadata not found for id '{fileId}'.");

        if (!File.Exists(record.PhysicalPath))
            throw new FileNotFoundException(
                $"Physical file not found at '{record.PhysicalPath}'. " +
                $"Metadata record exists but file was deleted from disk.");

        var stream = new FileStream(
            record.PhysicalPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return (stream, record);
    }

    // ─────────────────────────────────────────────────────────
    private void ValidateFile(string fileName, string contentType, long sizeBytes)
    {
        if (sizeBytes > _settings.MaxFileSizeBytes)
            throw new InvalidOperationException(
                $"File size {sizeBytes} bytes exceeds the maximum allowed size of {_settings.MaxFileSizeBytes} bytes.");

        if (!_settings.AllowedContentTypes.Contains(contentType))
            throw new InvalidOperationException(
                $"Content type '{contentType}' is not allowed. " +
                $"Allowed types: {string.Join(", ", _settings.AllowedContentTypes)}.");
    }
}