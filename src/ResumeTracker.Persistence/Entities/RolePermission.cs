using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Persistence.Entities;

public sealed class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    // Navigation
    public ApplicationRole Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}