using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Repositories;
using ResumeTracker.Persistence.Dispatchers;
using ResumeTracker.Persistence.Factories;
using ResumeTracker.Persistence.Identity;
using ResumeTracker.Persistence.Repositories;

namespace ResumeTracker.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ResumeTrackerDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ResumeTrackerDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure();
            });

            // EF Core requires both delegates when using UseAsyncSeeding
            // Actual Identity seeding happens in Program.cs (needs UserManager/RoleManager)
            options.UseSeeding((_, _) => { });
            options.UseAsyncSeeding((_, _, _) => Task.CompletedTask);
        });

        services.AddDbContextFactory<ResumeTrackerDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ResumeTrackerDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure();
            });

            options.UseSeeding((_, _) => { });
            options.UseAsyncSeeding((_, _, _) => Task.CompletedTask);
        },
        lifetime: ServiceLifetime.Scoped);

        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ResumeTrackerDbContext>()
            .AddUserManager<ApplicationUserManager>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ResumeTrackerDbContext>());
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDatabaseFactory, DatabaseFactory>();
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<IResumeRepository, ResumeRepository>();

        return services;
    }
}