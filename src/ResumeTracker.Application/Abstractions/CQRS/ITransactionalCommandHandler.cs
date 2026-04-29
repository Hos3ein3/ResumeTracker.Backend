namespace ResumeTracker.Application.Abstractions.CQRS;

/// <summary>
/// Convenience interface — implementing this makes the handler
/// transactional without touching the command record.
/// </summary>
public interface ITransactionalCommandHandler<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>, ITransactionalCommand
    where TCommand : ICommand<TResult>;