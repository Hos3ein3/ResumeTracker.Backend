using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions;
using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;
using ResumeTracker.Domain.Exceptions;

namespace ResumeTracker.Application.Features.Auth.Register;

public sealed class RegisterByEmailCommandHandler(IIdentityService identityService,ITokenService tokenService, ILogger<RegisterByEmailCommandHandler> logger)
    : ICommandHandler<RegisterByEmailCommand, OperationResult<AuthResponse>>
{
    public async Task<OperationResult<AuthResponse>> HandleAsync(RegisterByEmailCommand command, CancellationToken ct)
    {
        var existing = await identityService.FindByEmailAsync(command.Email);
        if (existing.Data is not null)
            return OperationResult<AuthResponse>.Conflict(
                $"An account with email '{existing.Data.Email}' already exists.");


        var createdUser = User.Create(command.Email, command.Email,"", command.FirstName, command.LastName,
            command.preferredLanguage, command.timeZone);

        if (createdUser.Data is null) return OperationResult<AuthResponse>.Failure(Error.Unkown);

        var result = await identityService.CreateAsync(createdUser.Data, command.Password);

        if (result.IsFailure || result.Data is null)
            return OperationResult<AuthResponse>.ValidationFailure("", errors: result.Errors);

        var addRoleResult = await identityService.AddToRoleAsync(result.Data, "User");
        if (addRoleResult.IsFailure || addRoleResult.Data is null)
            return OperationResult<AuthResponse>.ValidationFailure("", errors: addRoleResult.Errors);

        //logger.LogInformation("User {UserId} registered from {IP}", user.Data.Id, "ipAddress");
        //return await IssueTokenPairAsync(user, ipAddress, ct);

        var token = await tokenService.IssueTokenPairAsync(addRoleResult.Data, ct);
        if (token.IsFailure || token.Data is null) return OperationResult<AuthResponse>.Failure(Error.Unkown);
        return OperationResult<AuthResponse>.Success(token.Data);
    }
}