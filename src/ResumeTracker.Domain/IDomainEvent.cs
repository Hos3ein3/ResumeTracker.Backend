
namespace ResumeTracker.Domain;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }

}
