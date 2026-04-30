using Asp.Versioning;

namespace ResumeTracker.API.Features.Auth;

public static class AuthRouter 
{
    
    public static void MapAuthenticationRouter(this IEndpointRouteBuilder app)
    {
        var versionSet = app.GetVersionSet();
        var auth = app.MapGroup("/api/v{version:apiVersion}/auth")
            .WithTags("Auth")
            .WithApiVersionSet(versionSet)
            .HasApiVersion(1, 0)
            .HasApiVersion(2, 0);

        auth.MapPost("register-by-email", RegisterByEmail.Handle).WithName("register-by-email")
            .WithDisplayName("Register by email").WithDescription("Register by email.");
        
        auth.MapPost("register-by-phone",RegisterByPhone.Handle).WithName("register-by-phone")
            .WithDisplayName("Register by phone").WithDescription("Register by phone.");
    }
}