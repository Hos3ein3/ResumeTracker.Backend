using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<string> CreateAsync(UserPreferences preferences, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserPreferences preferences, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}