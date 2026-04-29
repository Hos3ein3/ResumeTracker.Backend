namespace ResumeTracker.Domain.Common;

public interface IHasConcurrencyStamp
{
    Guid ConcurrencyStamp { get; }
    void RotateConcurrencyStamp();
}