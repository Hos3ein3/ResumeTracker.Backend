namespace ResumeTracker.Domain.Documents;

public sealed class ResumeFile
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();   // MongoDB ObjectId
    public Guid ResumeId { get; set; }
    public Guid UserId { get; set; }
    public FileType FileType { get; set; }               // Resume or CoverLetter
    public string FileName { get; set; } = default!;   // original file name
    public string ContentType { get; set; } = default!;   // application/pdf etc.
    public long SizeBytes { get; set; }
    public string PhysicalPath { get; set; } = default!;   // absolute path on server disk
    public string RelativePath { get; set; } = default!;   // relative path for serving
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}