using ResumeTracker.Domain.Documents;

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IUserPreferencesRepository : IMongoRepository<UserPreferences>
{
    Task<UserPreferences?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}