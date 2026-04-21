

using Microsoft.AspNetCore.Identity;

using ResumeTracker.Persistence.Entities;

namespace ResumeTracker.Persistence.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;


    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
