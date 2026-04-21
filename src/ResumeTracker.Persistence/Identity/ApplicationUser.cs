

using Microsoft.AspNetCore.Identity;

namespace ResumeTracker.Persistence.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
