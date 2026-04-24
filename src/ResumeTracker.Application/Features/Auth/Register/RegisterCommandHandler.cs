
using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions;
using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;

namespace ResumeTracker.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, OperationResult<AuthResponse>>
{

    private readonly IIdentityService _identityService;
    private readonly ILogger<RegisterCommandHandler> _logger;
    public RegisterCommandHandler(IIdentityService identityService, ILogger<RegisterCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<OperationResult<AuthResponse>> HandleAsync(RegisterCommand command, CancellationToken ct)
    {
        var existing = await _identityService.FindByEmailAsync(command.Email);
        if (existing.Data is not null)
            return OperationResult<AuthResponse>.Conflict(
                $"An account with email '{existing.Data.Email}' already exists.");


        var user = User.Create(command.Email, command.Email, command.FirstName, command.LastName, command.preferredLanguage, command.timeZone);

        var result = await _identityService.CreateAsync(user?.Data, command.Password);

        if (result.IsFailure)
            return OperationResult<AuthResponse>.ValidationFailure("", errors: result.Errors);

        var addRoleResult = await _identityService.AddToRoleAsync(result.Data, "User");


        _logger.LogInformation("User {UserId} registered from {IP}", user.Data.Id, "ipAddress");
        //return await IssueTokenPairAsync(user, ipAddress, ct);

        return OperationResult<AuthResponse>.Success(null);
    }
}
