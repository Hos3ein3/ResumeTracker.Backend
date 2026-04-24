using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.Auth.Register;

public sealed record RegisterCommand
(string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? preferredLanguage,
    string? timeZone) : ICommand<OperationResult<AuthResponse>>;

