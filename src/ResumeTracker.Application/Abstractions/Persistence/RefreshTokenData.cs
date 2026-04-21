namespace ResumeTracker.Application.Abstractions.Persistence;

/// <summary>
/// Application-layer representation of a refresh token.
/// Keeps Application free of Persistence entity types.
/// </summary>
public sealed class RefreshTokenData
{
    public Guid Id { get; set; }
    public string Token { get; set; } = default!;
    public Guid UserId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }
    public string CreatedByIp { get; set; } = default!;
    public string? RevokedByIp { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked => RevokedAtUtc is not null;
    public bool IsActive => !IsExpired && !IsRevoked;
}