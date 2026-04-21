using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using ResumeTracker.Application.Abstractions.Cache;
using ResumeTracker.Application.Abstractions.FileStorage;
using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Features.Auth.Register;
using ResumeTracker.Application.Features.UserPreferences;
using ResumeTracker.Infrastructure.Cache;
using ResumeTracker.Infrastructure.Configurations;
using ResumeTracker.Infrastructure.FileStorage;
using ResumeTracker.Infrastructure.Localization;
using ResumeTracker.Infrastructure.Middlewares;
using ResumeTracker.Infrastructure.Middlewares.ExceptionHandling;
using ResumeTracker.Infrastructure.Middlewares.HttpLogging;
using ResumeTracker.Infrastructure.Persistence;
using ResumeTracker.Infrastructure.Repositories;
using ResumeTracker.Infrastructure.Services.Auth;
using ResumeTracker.Infrastructure.Services.UserPreferences;
using ResumeTracker.Infrastructure.Settings;

namespace ResumeTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoSettings = configuration
            .GetSection("MongoDb")
            .Get<MongoDbSettings>()
            ?? throw new InvalidOperationException("MongoDb settings are missing.");

        var redisSettings = configuration
            .GetSection("Redis")
            .Get<RedisSettings>()
            ?? throw new InvalidOperationException("Redis settings are missing.");

        // ── File storage settings ─────────────────────────────────
        var fileStorageSettings = configuration
            .GetSection("FileStorage")
            .Get<FileStorageSettings>()
            ?? throw new InvalidOperationException("FileStorage settings are missing.");

        services.AddSingleton(fileStorageSettings);
        services.AddScoped<IFileStorageService, MongoFileStorageService>();

        // ── 1. IMongoClient FIRST — everything else depends on it ──
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(mongoSettings.ConnectionString));

        // ── 2. MongoDB context + repository ───────────────────────
        services.AddSingleton(mongoSettings);
        services.AddSingleton<MongoDbContext>();   // now IMongoClient is already registered
        services.AddScoped<IFileRepository, FileRepository>();

        // ── 3. Redis ───────────────────────────────────────────────
        services.AddSingleton(redisSettings);
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
            options.InstanceName = redisSettings.InstanceName;
        });

        // ── 4. Health checks — AddMongoDb resolves IMongoClient from DI
        services.AddHealthChecksInfrastructure(configuration);

        // ── 5. CORS ────────────────────────────────────────────────
        services.AddCorsPolicy(configuration);

        services.AddFluentValidationSetup();

        // ── Polly pipelines ───────────────────────────────────────
        services.AddPollyPipelines();

        // ── Redis cache service ───────────────────────────────────
        services.AddScoped<ICacheService, RedisCacheService>();

        // ── MongoDB file storage ──────────────────────────────────
        services.AddScoped<IFileStorageService, MongoFileStorageService>();
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddCorrelationContext();
        services.AddLocalizationSetup();
        services.AddScoped<IMessageLocalizer, MessageLocalizer>();

        services.AddHttpRequestResponseLogging(options =>
{
    // override defaults if needed
    options = options with
    {
        LogRequestHeaders = true,
        LogResponseHeaders = false,
        LogRequestBody = true,
        LogResponseBody = true,
        LogOnlyErrors = false,
        MaxBodySizeBytes = 32 * 1024
    };
});

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();

        return services;
    }
}