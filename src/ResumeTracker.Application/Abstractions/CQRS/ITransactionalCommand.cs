namespace ResumeTracker.Application.Abstractions.CQRS;

/// <summary>
/// Marker interface — commands that require a DB transaction.
/// </summary>
public interface ITransactionalCommand;