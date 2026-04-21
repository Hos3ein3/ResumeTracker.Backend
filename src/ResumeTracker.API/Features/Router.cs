

using ResumeTracker.API.Extensions;
using ResumeTracker.API.Features.Auth;
using ResumeTracker.API.Features.UserPreferences;
using ResumeTracker.Application.Features.Auth.Register;
using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API;

public class Router : IVersionedEndpointRouter

{
    public double ApiVersion => ApiVersions.V1;

    public void MapRoutes(RouteGroupBuilder group)
    {
        var auth = group.MapGroup("auth")
             .WithTags("Auth")
             .AllowAnonymous();

        var prefs = group.MapGroup("user-pref").WithTags("User Preferences");

        auth.MapPost("register", Register.Handle)
        .WithName("Auth.Register")
        .WithSummary("Register a new user account")
        .WithValidator<RegisterRequest>();




        prefs.MapGet("get-current-user", GetCurrentUserPreferences.Handle)
                    .WithName("UserPreferences.GetCurrentUser")
                    .WithSummary("Get current user preferences");

        prefs.MapGet("get-a-user", GetUserPreferences.Handle)
        .WithName("UserPreferences.GetByUserId")
        .WithSummary("Get a user preferences");

        prefs.MapPatch(string.Empty, UpdateUserPreferences.Handle)
            .WithName("UserPreferences.Update")
            .WithSummary("Update current user preferences");
        //.WithValidator<UpdateUserPreferencesRequest>();

    }
}
