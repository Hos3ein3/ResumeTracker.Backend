using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ResumeTracker.Infrastructure.Configurations;

public static class PollyConfiguration
{
    // Named pipeline key — use this when resolving ResiliencePipelineProvider<string>
    public const string DefaultPipeline = "default-pipeline";

    public static IServiceCollection AddPollyPipelines(this IServiceCollection services)
    {
        // ── Named resilience pipeline (for manual use via ResiliencePipelineProvider) ──
        services.AddResiliencePipeline(DefaultPipeline, builder =>
        {
            builder
                // 1. Retry — exponential backoff, 3 attempts
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>()
                })

                // 2. Circuit breaker — opens after 50% failures in 10s window
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(15),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>()
                })

                // 3. Timeout — per attempt
                .AddTimeout(TimeSpan.FromSeconds(10));
        });

        return services;
    }

    /// <summary>
    /// Attaches the standard resilience pipeline to a named HttpClient.
    /// Usage: services.AddResilientHttpClient("my-client", "https://api.example.com")
    /// </summary>
    public static IServiceCollection AddResilientHttpClient(
        this IServiceCollection services,
        string clientName,
        string baseAddress)
    {
        services.AddHttpClient(clientName, client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        })
        .AddResilienceHandler($"{clientName}-pipeline", builder =>
        {
            builder
                .AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                })
                .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(15)
                })
                .AddTimeout(TimeSpan.FromSeconds(10));
        });

        return services;
    }
}