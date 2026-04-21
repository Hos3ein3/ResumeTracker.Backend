using System.Reflection;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;


namespace ResumeTracker.Infrastructure.Configurations;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidationSetup(this IServiceCollection services)
    {
        // Scans the Application assembly and registers all AbstractValidator<T> automatically
        // Use the assembly of any class that lives in Application
        services.AddValidatorsFromAssembly(
            Assembly.Load("ResumeTracker.Application"),
            lifetime: ServiceLifetime.Scoped,
            includeInternalTypes: true);

        return services;
    }
}