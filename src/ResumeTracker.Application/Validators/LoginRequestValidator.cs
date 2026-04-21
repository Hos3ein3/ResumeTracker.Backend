

using FluentValidation;

using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.DTOs.Auth;

namespace ResumeTracker.Application.Validators;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(IMessageLocalizer localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_ => localizer.MessageResources("Email.Required"))
            .EmailAddress().WithMessage(_ => localizer.MessageResources("Email.Invalid"));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_ => localizer.MessageResources("Password.Required"));
    }
}