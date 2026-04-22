using ResumeTracker.Domain.Enums;

namespace ResumeTracker.Domain.Events.Resume;

public record ResumeStatusChangedEvent(Guid ResumeId, ResumeStatus PreviousStatus,
ResumeStatus CurrentStatus, DateTime ChangedAt) : DomainEvent;

