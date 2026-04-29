using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;
using ResumeTracker.Application.Features.Auth.Register;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.API.Features.Auth;

public class AuthRouter : IVersionedEndpointRouter
{
    public double ApiVersion => ApiVersions.V1;

    public void MapRoutes(RouteGroupBuilder group)
    {
        var auth = group.MapGroup("auth2")
            .WithTags("Auth")
            .AllowAnonymous();

       

        auth.MapPost("register", async (
            RegisterRequest request,
            [FromServices] ICommandHandler<RegisterCommand, OperationResult<AuthResponse>> handler,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var command = new RegisterCommand(request.FirstName, request.LastName, "",request.Email,"", request.Password,
                request.ConfirmPassword, request.PreferredLanguage, request.TimeZone);
            var result = await handler.HandleAsync(command, ct);

            return result.ToHttpResult();
        }).WithName("Auth.Register2").WithSummary("Register a new account").WithValidator<RegisterRequest>();
    }
}