using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.FileStorage;

public interface IFileStorageService
{
    Task<ResumeFile?> GetFileMetadataAsync(
        string fileId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResumeFile>> GetResumeFilesAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the physical file to server disk and persists the metadata record in MongoDB.
    /// Returns the MongoDB document Id.
    /// </summary>
    Task<string> SaveFileAsync(
        Guid resumeId,
        Guid userId,
        FileType fileType,
        string fileName,
        string contentType,
        Stream fileStream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the physical file from disk and removes the MongoDB metadata record.
    /// </summary>
    Task DeleteFileAsync(
        string fileId,
        CancellationToken cancellationToken = default);

    Task DeleteAllResumeFilesAsync(
        Guid resumeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the physical file stream for download.
    /// </summary>
    Task<(Stream Stream, ResumeFile Metadata)> DownloadFileAsync(
        string fileId,
        CancellationToken cancellationToken = default);
}