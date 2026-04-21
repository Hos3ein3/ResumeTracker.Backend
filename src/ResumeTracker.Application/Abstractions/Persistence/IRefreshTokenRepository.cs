
namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenData?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(RefreshTokenData token, CancellationToken ct = default);
    Task UpdateAsync(RefreshTokenData token, CancellationToken ct = default);
    Task RevokeAllByUserIdAsync(Guid userId, string revokedByIp, CancellationToken ct = default);
}

