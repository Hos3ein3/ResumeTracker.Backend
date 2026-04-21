

namespace ResumeTracker.Application.Abstractions.Persistence;

public interface IDatabaseConnectionFactory
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}