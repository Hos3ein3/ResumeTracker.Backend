using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Persistence.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }   // rotation chain

    // Navigation
    public ApplicationUser User { get; set; } = default!;
}