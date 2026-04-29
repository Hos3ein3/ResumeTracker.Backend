namespace ResumeTracker.Domain.Exceptions;

public class DbTransactionException:Exception
{
    public Error Error { get; }
    
    public DbTransactionException(Error error, Exception innerException)
        : base(error.Description, innerException)
    {
        Error = error;
    }
    
}