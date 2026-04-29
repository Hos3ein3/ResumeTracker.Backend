
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Domain;

public abstract class BaseEntity<TId>:IHasConcurrencyStamp
{
    public TId Id { get; protected set; } = default!;
    
    public Guid ConcurrencyStamp { get; private set; } = Guid.CreateVersion7();

    public void RotateConcurrencyStamp()
        => ConcurrencyStamp = Guid.CreateVersion7();
}
