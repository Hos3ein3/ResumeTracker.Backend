namespace ResumeTracker.Persistence.Entities;

public sealed class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;  // e.g. "resumes.read"
    public string Resource { get; set; } = default!;  // e.g. "resumes"
    public string Action { get; set; } = default!;  // e.g. "read" | "write" | "delete"
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}