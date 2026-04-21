using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.FileStorage;

public interface IFileStorageService
{
    Task<ResumeFile?> GetFileAsync(string fileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumeFile>> GetResumeFilesAsync(Guid resumeId, CancellationToken cancellationToken = default);
    Task<string> UploadFileAsync(Guid resumeId, Guid userId, FileType fileType, string fileName, string contentType, Stream content, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileId, CancellationToken cancellationToken = default);
    Task DeleteAllResumeFilesAsync(Guid resumeId, CancellationToken cancellationToken = default);
}