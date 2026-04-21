using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.UserPreferences;

public static class GetCurrentUserPreferences
{
    public static async Task<IResult> Handle(
       ClaimsPrincipal user,
     [FromServices] IUserPreferencesService service,
       CancellationToken cancellationToken)
    {
        var userId = user.GetRequiredUserId();

        var result = await service.GetByUserIdAsync(userId, cancellationToken);
        return result.ToHttpResult();
    }
}
