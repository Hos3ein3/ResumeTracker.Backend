


namespace ResumeTracker.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ScanCQRSScurtor(this IServiceCollection services)
        => services.Scan(scan => scan
            .FromAssemblies(Application.AssemblyRef.Assembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


}
