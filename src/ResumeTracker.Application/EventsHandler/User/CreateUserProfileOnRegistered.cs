


using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions.Events;
using ResumeTracker.Application.Features.UserPreferences;
using ResumeTracker.Domain.Events.Users;

namespace ResumeTracker.Application.EventsHandler.User;

public sealed class CreateUserProfileOnRegistered(IUserPreferencesService userPreferencesService, ILogger<CreateUserProfileOnRegistered> logger)
: IDomainEventHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken cancellationToken = default)
    {
        await userPreferencesService.CreateAsync(@event.UserId, userPreferencesService.CreateObject(@event.PreferredLanguage, @event.TimeZone), cancellationToken);
        logger.LogInformation("{0} Handle for user with: {1}", nameof(CreateUserProfileOnRegistered), @event.UserId);
    }
}
