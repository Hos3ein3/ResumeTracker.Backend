using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions;
using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;
using ResumeTracker.Domain.Exceptions;

namespace ResumeTracker.Application.Features.Auth.RegisterByPhone;

public sealed class RegisterByPhoneCommandHandler(IIdentityService identityService,ITokenService tokenService,ILogger<RegisterByPhoneCommandHandler> logger)
    :ICommandHandler<RegisterByPhoneCommand,OperationResult<AuthResponse>>
{
    public async Task<OperationResult<AuthResponse>> HandleAsync(RegisterByPhoneCommand command, CancellationToken ct)
    {
        var existing = await identityService.FindByPhonoeAsync(command.PhoneNumber);
        if (existing.Data is not null)
            return OperationResult<AuthResponse>.Conflict(
                $"An account with Phone '{existing.Data.PhoneNumber}' already exists.");


        var createdUser = User.Create("", command.PhoneNumber,command.PhoneNumber, command.FirstName, command.LastName,
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