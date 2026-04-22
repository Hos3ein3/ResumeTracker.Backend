

using FluentValidation;

using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.DTOs;

namespace ResumeTracker.Application.Validators;


public sealed class UpdateUserPreferencesRequestValidator
    : AbstractValidator<UpdateUserPreferencesRequest>
{
    private static readonly string[] ValidLanguages = ["en", "fa", "it"];
    private static readonly string[] ValidThemes = ["light", "dark", "system"];
    private static readonly string[] ValidSortOrders = ["asc", "desc"];
    private static readonly string[] ValidSortByFields =
        ["CreatedAtUtc", "ModifiedAtUtc", "Title"];

    public UpdateUserPreferencesRequestValidator(IMessageLocalizer localizer)
    {
        RuleFor(x => x.Language)
            .Must(l => l is null || ValidLanguages.Contains(l))
            .WithMessage(_ => localizer.UserPreferencesResources("Language.Invalid"));

        RuleFor(x => x.Theme)
            .Must(t => t is null || ValidThemes.Contains(t))
            .WithMessage(_ => localizer.UserPreferencesResources("Preferences.Theme.Invalid"));

        RuleFor(x => x.DefaultPageSize)
            .InclusiveBetween(5, 100)
            .When(x => x.DefaultPageSize.HasValue)
            .WithMessage(_ => localizer.UserPreferencesResources("Preferences.PageSize.Range"));

        RuleFor(x => x.DefaultSortOrder)
            .Must(s => s is null || ValidSortOrders.Contains(s))
            .WithMessage(_ => localizer.UserPreferencesResources("Preferences.SortOrder.Invalid"));

        RuleFor(x => x.DefaultSortBy)
            .Must(s => s is null || ValidSortByFields.Contains(s))
            .WithMessage(_ => localizer.UserPreferencesResources("Preferences.SortBy.Invalid"));
    }
}