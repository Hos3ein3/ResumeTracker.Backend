

using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;

namespace ResumeTracker.Application.Abstractions;


public interface IIdentityService
{
    Task<OperationResult<User>> FindByEmailAsync(string email);
    Task<OperationResult<User>> CreateAsync(User user, string password);
    Task<OperationResult<User>> AddToRoleAsync(User user, string role);
    Task<OperationResult<IList<string>>> GetRolesAsync(User user);
    Task<OperationResult> CheckPasswordAsync(User user, string password);
}
