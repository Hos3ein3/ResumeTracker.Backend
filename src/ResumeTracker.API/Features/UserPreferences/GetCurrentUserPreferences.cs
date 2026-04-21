using System.Security.Claims;

using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.UserPreferences;

public static class GetCurrentUserPreferences
{
    public static async Task<IResult> Handle(
       ClaimsPrincipal user,
       IUserPreferencesService service,
       CancellationToken cancellationToken)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user.FindFirstValue("sub");

        if (userId is null)
            return Results.Unauthorized();

        var result = await service.GetByUserIdAsync(Guid.Parse(userId), cancellationToken);
        return result.ToHttpResult();
    }
}
