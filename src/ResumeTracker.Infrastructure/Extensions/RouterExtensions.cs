using System.Reflection;

using Asp.Versioning;
using Asp.Versioning.Builder;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ResumeTracker.API.Extensions;

public interface IVersionedEndpointRouter
{
    double ApiVersion { get; }
    void MapRoutes(RouteGroupBuilder group);
}

public interface IRouter
{
    void RegisterRoutes(IEndpointRouteBuilder app);
}

public static class RouterExtensions
{
    public static IServiceCollection AddRouters(this IServiceCollection services)
    {
        var routerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IRouter).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

        foreach (var type in routerTypes)
            services.AddScoped(typeof(IRouter), type);

        return services;
    }
    public static IServiceCollection AddVersionedRouters(this IServiceCollection services, Assembly assembly)
    {
        var routerType = typeof(IVersionedEndpointRouter);

        assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && routerType.IsAssignableFrom(t))
            .ToList()
            .ForEach(t => services.AddSingleton(routerType, t));

        return services;
    }

    public static WebApplication MapRouters(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var routers = scope.ServiceProvider.GetServices<IRouter>();

        foreach (var router in routers)
            router.RegisterRoutes(app);

        return app;
    }
    public static WebApplication MapVersionedRouters(this WebApplication app)
    {
        ApiVersionSet versionSet = app
            .NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder root = app
            .MapGroup("/api/v{version:apiVersion}")
            .WithApiVersionSet(versionSet);

        var routers = app.Services
            .GetRequiredService<IEnumerable<IVersionedEndpointRouter>>();

        foreach (var router in routers)
        {
            RouteGroupBuilder versionedGroup = root
                .MapGroup(string.Empty)
                .MapToApiVersion(router.ApiVersion);

            router.MapRoutes(versionedGroup);
        }

        return app;
    }
}