using System.Security.Claims;

using k8s.KubeConfigModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using ResumeTracker.Application.Features.UserPreferences;
using ResumeTracker.Domain.Exceptions;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.API.Features.UserPreferences;

public class GetUserPreferences
{
    public static async Task<IResult> Handle(
       string userId,
     [FromServices] IUserPreferencesService service,
        UserManager<ApplicationUser> userManager,
       CancellationToken cancellationToken)
    {

        //Check in Users that user exists or not, if not return NotFound

        if (userId is null)
            return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new NotFoundException(userId);

        var result = await service.GetByUserIdAsync(Guid.Parse(userId), cancellationToken);
        return result.ToHttpResult();
    }
}
