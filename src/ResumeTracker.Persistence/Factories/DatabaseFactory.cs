
using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Abstractions.Persistence;

namespace ResumeTracker.Persistence.Factories;

public sealed class DatabaseFactory : IDatabaseFactory
{
    private readonly IDbContextFactory<ResumeTrackerDbContext> _factory;

    public DatabaseFactory(IDbContextFactory<ResumeTrackerDbContext> factory)
    {
        _factory = factory;
    }

    public ResumeTrackerDbContext CreateDbContext()
    {
        return _factory.CreateDbContext();
    }
}
