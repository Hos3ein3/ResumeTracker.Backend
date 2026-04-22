

namespace ResumeTracker.Domain.Events.Users;

public record UserRegisteredEvent(Guid UserId, string Email, string FirstName, string LastName,
string? PreferredLanguage,
    string? TimeZone)
: DomainEvent;
