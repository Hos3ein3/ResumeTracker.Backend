using FluentValidation;

namespace ResumeTracker.Application.Features.Auth.Register;

public sealed class RegisterByEmailCommandValidator : AbstractValidator<RegisterByEmailCommand>
{
    public RegisterByEmailCommandValidator()
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
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            // .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            // .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
        
        RuleFor(x => x.ConfirmPassword)
            .Matches(x=>x.Password).WithMessage("Passwords must match.");

    }
}
