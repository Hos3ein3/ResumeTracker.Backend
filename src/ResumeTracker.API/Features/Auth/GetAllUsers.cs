
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.API.Features.Auth;

public static class GetAllUsers
{
    public static async Task<IResult> Handle(
[FromServices] UserManager<ApplicationUser> userManager
    )
    {
        return Results.Ok(await userManager.Users.ToListAsync());
    }

}
