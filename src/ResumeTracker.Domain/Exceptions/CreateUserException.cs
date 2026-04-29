namespace ResumeTracker.Domain.Exceptions;

public class CreateUserException:Exception
{
    public CreateUserException(string message) : base(message)
    {
    }
    
    public CreateUserException():base(){}
    
}