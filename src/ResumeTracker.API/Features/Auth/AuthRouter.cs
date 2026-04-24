


namespace ResumeTracker.API.Features.Auth;

public class AuthRouter : IVersionedEndpointRouter
{
    public double ApiVersion => ApiVersions.V1;

    public void MapRoutes(RouteGroupBuilder group)
    {
        var auth = group.MapGroup("auth2")
             .WithTags("Auth")
             .AllowAnonymous();

        auth.MapPost("register", Register.Handle)
                    .WithName("Auth.Register")
                    .WithSummary("Register a new account")
                    .WithValidator<RegisterRequest>();

    }
}
