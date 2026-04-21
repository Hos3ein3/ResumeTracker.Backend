
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using ResumeTracker.Application.DTOs;
using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.UserPreferences;

public static class GetOrCreate
{
    public static async Task<IResult> Handle(
           ClaimsPrincipal user,
UpdateUserPreferencesRequest request,
         [FromServices] IUserPreferencesService service,
           CancellationToken cancellationToken)
    {
        var userId = user.GetRequiredUserId();

        var result = await service.GetByUserIdAsync(userId, cancellationToken);
        if (!result.IsSuccess)
        {
            var createResult = await service.CreateAsync(userId, request, cancellationToken);
            if (!createResult.IsSuccess)
            {
                return createResult.ToHttpResult();
            }

            // Try fetching again after creation
            result = await service.GetByUserIdAsync(userId, cancellationToken);
        }
        return result.ToHttpResult();
    }
}
