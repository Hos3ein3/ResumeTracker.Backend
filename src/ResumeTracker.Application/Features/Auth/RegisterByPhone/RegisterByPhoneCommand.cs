using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.Auth.RegisterByPhone;

public sealed record RegisterByPhoneCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string? preferredLanguage,
    string? timeZone): ICommand<OperationResult<AuthResponse>>,ITransactionalCommand;