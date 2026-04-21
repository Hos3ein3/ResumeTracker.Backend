using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;

using Refit;

namespace ResumeTracker.Infrastructure.Configurations;

public static class RefitConfiguration
{
    private static readonly RefitSettings DefaultSettings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        })
    };

    /// <summary>
    /// Registers a typed Refit client with the shared resilience pipeline.
    /// Usage: services.AddRefitClient<IMyApi>("my-api", "https://api.example.com", configuration)
    /// </summary>
    public static IServiceCollection AddRefitClient<TClient>(
        this IServiceCollection services,
        string clientName,
        string baseAddress,
        IConfiguration configuration)
        where TClient : class
    {
        services
            .AddRefitClient<TClient>(DefaultSettings)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddResilienceHandler($"{clientName}-refit-pipeline", builder =>
            {
                builder
                    .AddRetry(new Microsoft.Extensions.Http.Resilience.HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromMilliseconds(300),
                        BackoffType = Polly.DelayBackoffType.Exponential,
                        UseJitter = true
                    })
                    .AddTimeout(TimeSpan.FromSeconds(15));
            });

        return services;
    }
}