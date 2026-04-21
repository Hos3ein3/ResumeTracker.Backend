using Microsoft.Extensions.Localization;

using ResumeTracker.Application;
using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.Resources;
using ResumeTracker.Application.Resources.Entities.UserPreferences;

namespace ResumeTracker.Infrastructure.Localization;

public sealed class MessageLocalizer : IMessageLocalizer
{
    private readonly IStringLocalizer<MessagesResources> _messagesResources;
    private readonly IStringLocalizer<UserPreferencesResources> _userPreferencesResources;
    private readonly IStringLocalizer<ExceptionResource> _exceptionResources;

    public MessageLocalizer(
        IStringLocalizer<MessagesResources> messages
, IStringLocalizer<UserPreferencesResources> userPreferencesResources
, IStringLocalizer<ExceptionResource> exceptionResources
        //IStringLocalizer<ValidationMessages> validation
        )
    {
        _messagesResources = messages;
        _userPreferencesResources = userPreferencesResources;
        _exceptionResources = exceptionResources;
        //   _validation = validation;
    }

    public string ExceptionResources(string key, params object[] args)
    => args.Length == 0
            ? _exceptionResources[key].Value
            : _exceptionResources[key, args].Value;
    public string MessageResources(string key, params object[] args)
        => args.Length == 0
            ? _messagesResources[key].Value
            : _messagesResources[key, args].Value;

    public string UserPreferencesResources(string key, params object[] args)
   => args.Length == 0
            ? _userPreferencesResources[key].Value
            : _userPreferencesResources[key, args].Value;
}