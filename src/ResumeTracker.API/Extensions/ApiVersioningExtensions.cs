using Asp.Versioning;
using Asp.Versioning.Builder;

namespace ResumeTracker.API.Extensions;

public static class ApiVersioningExtensions
{
    public static ApiVersionSet GetVersionSet(this IEndpointRouteBuilder app)
        => app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();
}