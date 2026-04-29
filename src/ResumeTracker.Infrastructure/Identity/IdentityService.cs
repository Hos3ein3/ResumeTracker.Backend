


using Microsoft.AspNetCore.Identity;

using ResumeTracker.Application.Abstractions;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;
using ResumeTracker.Domain.Exceptions;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

    }

    public async Task<OperationResult<User>> CreateAsync(User user, string password)
    {
        var appUser = user.ToApplicationUser();
        var result = await _userManager.CreateAsync(appUser, password);

        if (!result.Succeeded) return OperationResult<User>.ValidationFailure(
            "Registration failed.",
            result.Errors.Select(e => e.Description));

        return OperationResult<User>.Success(appUser.ToDomainUser());
    }

    public async Task<OperationResult<User>> FindByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);

        if (appUser is null)
            return OperationResult<User>.Failure(Error.NotFound);
        return OperationResult<User>.Success(appUser.ToDomainUser());
    }

    public async Task<OperationResult<User>> AnyByEmailAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        
        return appUser is null ? OperationResult<User>.Failure(Error.NotFound) :
         OperationResult<User>.Success(appUser.ToDomainUser());
    }
    
    public async Task<OperationResult<User>> AddToRoleAsync(User user, string role)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());

        if (appUser is null)
            return OperationResult<User>.ValidationFailure(
                $"User with id '{user.Id}' not found.", new[] { "User not found" });

        var result = await _userManager.AddToRoleAsync(appUser, role);

        if (!result.Succeeded)
            return OperationResult<User>.ValidationFailure(
                $"Failed to assign role '{role}' to user.",
                result.Errors.Select(e => e.Description));

        return OperationResult<User>.Success(user);
    }



    public async Task<OperationResult<IList<string>>> GetRolesAsync(User user)
    => OperationResult<IList<string>>.Info(await _userManager.GetRolesAsync(user.ToApplicationUser()), "");

    public async Task<OperationResult> CheckPasswordAsync(User user, string password)
        => await _userManager.CheckPasswordAsync(user.ToApplicationUser(), password) ? OperationResult.Success()
            : OperationResult<bool>.Failure(Error.None);
}
