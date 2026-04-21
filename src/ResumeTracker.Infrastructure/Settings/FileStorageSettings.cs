namespace ResumeTracker.Infrastructure.Settings;

public sealed class FileStorageSettings
{
    public string BasePath { get; init; } = default!;  // e.g. /var/resumetracker/uploads
    public long MaxFileSizeBytes { get; init; } = 10 * 1024 * 1024;  // 10 MB default
    public string[] AllowedContentTypes { get; init; } =
    [
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];
}