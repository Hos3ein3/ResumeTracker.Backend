using ResumeTracker.Domain.Common;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Persistence.Entities;

public sealed class RefreshToken : AuditableAggregateRoot<Guid>
{

    public string Token { get; set; } = default!;
    public Guid UserId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? CreatedByIp { get; set; } = default!;
    public string? RevokedByIp { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc is not null;
    public bool IsActive => !IsExpired && !IsRevoked;

    public ApplicationUser User { get; set; } = default!;
    private RefreshToken() { }

    public static RefreshToken Create(
        Guid userId,
        string token,
        string createdByIp,
        int expiryDays = 7)
        => new()
        {
            Id = NewId.Next(),
            UserId = userId,
            Token = token,
            CreatedByIp = createdByIp,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(expiryDays)
        };

    public static RefreshToken Reconstitute(
            Guid id,
            Guid userId,
            string token,
            string createdByIp,
            DateTime createdAtUtc,
            DateTime expiresAtUtc,
            DateTime? revokedAtUtc = null,
            string? replacedByToken = null,
            string? revokedByIp = null)
            => new()
            {
                Id = id,
                UserId = userId,
                Token = token,
                CreatedByIp = createdByIp,
                CreatedAtUtc = createdAtUtc,
                ExpiresAtUtc = expiresAtUtc,
                RevokedAtUtc = revokedAtUtc,
                ReplacedByToken = replacedByToken,
                RevokedByIp = revokedByIp
            };


    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        RevokedAtUtc = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}