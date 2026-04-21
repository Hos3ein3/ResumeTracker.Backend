using System.Security.Claims;

using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.UserPreferences;

public static class UpdateUserPreferences
{
    public static async Task<IResult> Handle(
        UpdateUserPreferencesRequest request,
        ClaimsPrincipal user,
        IUserPreferencesService service,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user.FindFirstValue("sub");

        if (userId is null)
            return Results.Unauthorized();

        var result = await service.UpdateAsync(
            Guid.Parse(userId), request, cancellationToken);

        return result.ToHttpResult();
    }
}