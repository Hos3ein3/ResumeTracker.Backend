
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;
using ResumeTracker.Application.Features.Auth.Register;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.API.Features.Auth;

public static class Register
{
    public static async Task<IResult> Handle(
    RegisterRequest request,
    [FromServices] ICommandHandler<RegisterCommand, OperationResult<AuthResponse>> handler,
    HttpContext httpContext,
    CancellationToken ct)
    {
        var ip = GetIpAddress(httpContext);


        var command = new RegisterCommand(request.FirstName, request.LastName, "",request.Email,"", request.Password, request.ConfirmPassword, request.PreferredLanguage, request.TimeZone);

        var result = await handler.HandleAsync(command, ct);

        // Map AuthResponse → RegisterResponse (if shapes differ)
        return result.ToHttpResult();
    }


    private static string GetIpAddress(HttpContext ctx)
        => ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault()
           ?? ctx.Connection.RemoteIpAddress?.ToString()
           ?? "unknown";
}
public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PreferredLanguage,
    string? TimeZone);

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.PreferredLanguage)
            .Must(lang => lang is null || new[] { "en", "fa", "it" }.Contains(lang))
            .WithMessage("Supported languages are: en, fa, it.");
    }
}
