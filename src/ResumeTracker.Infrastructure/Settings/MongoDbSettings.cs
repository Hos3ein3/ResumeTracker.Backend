namespace ResumeTracker.Infrastructure.Settings;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; init; } = default!;
    public string DatabaseName { get; init; } = default!;
    public string FilesCollection { get; init; } = "resume_files";
}