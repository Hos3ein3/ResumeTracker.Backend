using FluentValidation;

namespace ResumeTracker.Application.Features.Auth.RegisterByPhone;

public sealed class RegisterByPhoneCommandValidator:AbstractValidator<RegisterByPhoneCommand>
{
    public RegisterByPhoneCommandValidator()
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