
namespace ResumeTracker.Application.Events;

public sealed record UserRegisteredEvent(Guid UserId, string Email);
