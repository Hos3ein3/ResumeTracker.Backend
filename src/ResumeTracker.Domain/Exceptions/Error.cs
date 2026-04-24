

namespace ResumeTracker.Domain.Exceptions;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NotFound = new("NotFound", "Resource not found");
    public static readonly Error Conflict = new("Conflict", "Resource conflict");
    public static readonly Error Validation = new("ValidationError", "Validation failed");
    public static readonly Error Unauthorized = new("Unauthorized", "Unauthorized access");
}
