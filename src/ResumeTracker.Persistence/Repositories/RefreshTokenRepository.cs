using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Persistence.Entities;

namespace ResumeTracker.Persistence.Repositories;

public sealed class RefreshTokenRepository : GenericRepository<RefreshToken, Guid>, IRefreshTokenRepository
{
    private readonly ResumeTrackerDbContext _context;

    public RefreshTokenRepository(ResumeTrackerDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<RefreshTokenData?> GetByTokenAsync(
        string token, CancellationToken ct = default)
    {
        var entity = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == token, ct);

        return entity is null ? null : MapToData(entity);
    }

    public async Task AddAsync(RefreshTokenData data, CancellationToken ct = default)
    {
        var entity = MapToEntity(data);
        await _context.RefreshTokens.AddAsync(entity, ct);
    }

    public Task UpdateAsync(RefreshTokenData data, CancellationToken ct = default)
    {
        var entity = MapToEntity(data);
        _context.RefreshTokens.Update(entity);
        return Task.CompletedTask;
    }

    public async Task RevokeAllByUserIdAsync(
        Guid userId, string revokedByIp, CancellationToken ct = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
            .ToListAsync(ct);

        foreach (var t in tokens)
            t.Revoke(revokedByIp);
    }

    // ── Mappers ───────────────────────────────────────────────────
    private static RefreshTokenData MapToData(RefreshToken e) => new()
    {
        Id = e.Id,
        Token = e.Token,
        UserId = e.UserId,
        ExpiresAtUtc = e.ExpiresAtUtc,
        CreatedAtUtc = e.CreatedAtUtc,
        RevokedAtUtc = e.RevokedAtUtc,
        ReplacedByToken = e.ReplacedByToken,
        CreatedByIp = e.CreatedByIp,
        RevokedByIp = e.RevokedByIp
    };

    private static RefreshToken MapToEntity(RefreshTokenData d)
    => RefreshToken.Reconstitute(   // ← factory method, no constructor call
        id: d.Id,
        userId: d.UserId,
        token: d.Token,
        createdByIp: d.CreatedByIp,
        createdAtUtc: d.CreatedAtUtc,
        expiresAtUtc: d.ExpiresAtUtc,
        revokedAtUtc: d.RevokedAtUtc,
        replacedByToken: d.ReplacedByToken,
        revokedByIp: d.RevokedByIp);
}