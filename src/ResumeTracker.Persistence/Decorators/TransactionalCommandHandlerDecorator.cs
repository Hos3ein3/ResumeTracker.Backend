using ResumeTracker.Application.Abstractions.CQRS;
using ResumeTracker.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ResumeTracker.Persistence.Decorators;


public sealed class TransactionalCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    ResumeTrackerDbContext dbContext)
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> HandleAsync(TCommand command, CancellationToken ct)
    {
        // Pass through non-transactional commands without any overhead
        if (command is not ITransactionalCommand)
            return await inner.HandleAsync(command, ct);

        var strategy = dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database
                .BeginTransactionAsync(ct);
            try
            {
                var result = await inner.HandleAsync(command, ct);
                await transaction.CommitAsync(ct);
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                throw new DbTransactionException(Error.TransactionAborted, ex);
            }
        });
    }
}