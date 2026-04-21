
using Microsoft.AspNetCore.Identity;

using ResumeTracker.Persistence.Entities;

namespace ResumeTracker.Persistence.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public uint Priority { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
