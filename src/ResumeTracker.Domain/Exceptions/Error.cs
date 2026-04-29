

namespace ResumeTracker.Domain.Exceptions;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NotFound = new("NotFound", "Resource not found");
    public static readonly Error Conflict = new("Conflict", "Resource conflict");
    public static readonly Error Validation = new("ValidationError", "Validation failed");
    public static readonly Error Unauthorized = new("Unauthorized", "Unauthorized access");
    public static readonly Error UnauthorizedAccess = new("Unauthorized", "Access denied");
    public static readonly Error UnauthorizedRefresh = new("UnauthorizedRefresh", "UnauthorizedRefresh");
    public static readonly Error UnauthorizedRefreshToken = new("UnauthorizedRefreshToken", "UnauthorizedRefreshToken");
    public static readonly Error Unkown = new("Unkown", "Unkown error");
    
    public static readonly Error InternalServerError = new("InternalServerError", "Internal server error");
    public static readonly Error NotImplemented = new("NotImplemented", "Not implemented");
    public static readonly Error BadGateway = new("BadGateway", "Bad gateway");
    public static readonly Error ServiceUnavailable = new("ServiceUnavailable", "Service unavailable");
    public static readonly Error GatewayTimeout = new("GatewayTimeout", "Gateway timeout");
    public static readonly Error HttpVersionNotSupported = new("HttpVersionNotSupported", "HTTP version not supported");
    public static readonly Error ServerError = new("ServerError", "Server error");
    public static readonly Error DatabaseError = new("DatabaseError", "Database error");
    public static readonly Error TransactionAborted = new("TransactionAborted", "Transaction aborted");
    
}
