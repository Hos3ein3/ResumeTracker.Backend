using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using ResumeTracker.Application.DTOs;
using ResumeTracker.Application.Features.UserPreferences;

namespace ResumeTracker.API.Features.UserPreferences;

public static class UpdateUserPreferences
{
    public static async Task<IResult> Handle(
        UpdateUserPreferencesRequest request,
        ClaimsPrincipal user,
       [FromServices] IUserPreferencesService service,
        CancellationToken cancellationToken)
    {
        var userId = user.GetRequiredUserId();



        var result = await service.UpdateAsync(
            userId, request, cancellationToken);

        return result.ToHttpResult();
    }
}