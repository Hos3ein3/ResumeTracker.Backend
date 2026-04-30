using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth.RegisterByPhone;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.API.Features.Auth;

public static class RegisterByPhone
{
    public static async Task<IResult> Handle(
        RegisterByPhoneRequest request,
        [FromServices]ICommandHandler<RegisterByPhoneCommand,OperationResult<AuthResponse>> handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var command =  new RegisterByPhoneCommand(request.FirstName, request.LastName, request.PhoneNumber,request.Password,
            request.ConfirmPassword,request.preferredLanguage,request.timeZone);
        
        var result= await handler.HandleAsync(command, ct);

        return result.ToHttpResult();
    }
    
}

public sealed record RegisterByPhoneRequest(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Password,
    string ConfirmPassword,
    string? preferredLanguage,
    string? timeZone);

public sealed class RegisterByPhoneRequestValidator : AbstractValidator<RegisterByPhoneRequest>
{
    public RegisterByPhoneRequestValidator()
    {
        RuleFor(x=>x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(12).WithMessage("Phone number must not exceed 12 characters.");
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        // .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
        // .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        
        RuleFor(x => x.ConfirmPassword)
            .Matches(x=>x.Password).WithMessage("Passwords must match.");
    }
}