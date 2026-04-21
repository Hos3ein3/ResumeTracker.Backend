namespace ResumeTracker.Domain.Exceptions;

public sealed class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Access to this resource is forbidden.")
        : base(message) { }
}