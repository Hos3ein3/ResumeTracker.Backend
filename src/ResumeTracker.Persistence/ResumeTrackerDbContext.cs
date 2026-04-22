

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Domain;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities;
using ResumeTracker.Persistence.Configurations;
using ResumeTracker.Persistence.Conventions;
using ResumeTracker.Persistence.Entities;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Persistence;

public class ResumeTrackerDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IUnitOfWork
{

    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public ResumeTrackerDbContext(DbContextOptions<ResumeTrackerDbContext> options,
        IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(DatabaseSchemas.App);
        base.OnModelCreating(builder);
        builder.buildIdentityConfiguration();

        builder.ApplyConfigurationsFromAssembly(typeof(ResumeTrackerDbContext).Assembly);


    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .SelectMany(x => x.DomainEvents)
            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);


        if (domainEvents.Count != 0)
        {
            await _domainEventDispatcher.DispatchAllAsync(domainEvents, cancellationToken);
        }
        foreach (var entity in ChangeTracker.Entries<AggregateRoot<Guid>>()
                             .Select(x => x.Entity)
                             .Where(x => x.DomainEvents.Any()))
        {
            entity.ClearDomainEvents();
        }
        return result;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new GuidV7Convention());
    }
}
