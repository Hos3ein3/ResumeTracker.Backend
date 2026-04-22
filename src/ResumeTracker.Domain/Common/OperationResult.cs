using System.Net;

namespace ResumeTracker.Domain.Common;


public sealed class OperationResult<T> : OperationResult
{
    public T? Data { get; private init; }

    private OperationResult() { }

    public static OperationResult<T> Success(
        T data,
        string message = "Operation completed successfully.",
        string title = "Success",
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccess = true,
            Data = data,
            Title = title,
            Message = message,
            StatusCode = statusCode,
            Status = OperationResultStatus.Success
        };

    public static OperationResult<T> Created(
        T data,
        string message = "Resource created successfully.",
        string title = "Created")
        => new()
        {
            IsSuccess = true,
            Data = data,
            Title = title,
            Message = message,
            Status = OperationResultStatus.Success,
            StatusCode = HttpStatusCode.Created
        };

    public new static OperationResult<T> Failure(
        string message,
        string title = "Error",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IEnumerable<string>? errors = null)
        => new()
        {
            IsSuccess = false,
            Title = title,
            Message = message,
            StatusCode = statusCode,
            Status = OperationResultStatus.Error,
            Errors = errors?.ToList() ?? []
        };

    public static OperationResult<T> Info(
            T? data,
            string message,
            string title = "Information",
         HttpStatusCode statusCode = HttpStatusCode.Continue)
        => new()
        {
            IsSuccess = true,
            Data = data,
            Title = title,
            Message = message,
            StatusCode = statusCode,
            Status = OperationResultStatus.Info

        };

    public new static OperationResult<T> NotFound(
        string message,
        string title = "Not Found")
        => Failure(message, title, HttpStatusCode.NotFound);

    public new static OperationResult<T> Conflict(
        string message,
        string title = "Conflict")
        => Failure(message, title, HttpStatusCode.Conflict);

    public new static OperationResult<T> ValidationFailure(
        string message,
        IEnumerable<string> errors,
        string title = "Validation Failed")
        => Failure(message, title, HttpStatusCode.UnprocessableEntity, errors);

    /// <summary>Converts to IResult for minimal API endpoints.</summary>

}


/// <summary>
/// Non-generic result — for operations that perform an action but return nothing.
/// e.g. Delete, Update, Send email
/// </summary>
public class OperationResult
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
    public string Title { get; protected init; } = default!;
    public string Message { get; protected init; } = default!;
    public HttpStatusCode StatusCode { get; protected init; }
    public IReadOnlyList<string> Errors { get; protected init; } = [];
    public OperationResultStatus Status { get; set; }
    protected OperationResult() { }

    public static OperationResult Success(
        string message = "Operation completed successfully.",
        string title = "Success",
        HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccess = true,
            Title = title,
            Message = message,
            StatusCode = statusCode,
            Status = OperationResultStatus.Success
        };

    public static OperationResult Failure(
        string message,
        string title = "Error",
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        IEnumerable<string>? errors = null)
        => new()
        {
            IsSuccess = false,
            Title = title,
            Message = message,
            StatusCode = statusCode,
            Status = OperationResultStatus.Error,
            Errors = errors?.ToList() ?? []
        };

    public static OperationResult Info(
            string message,
            string title = "Information",
            HttpStatusCode statusCode = HttpStatusCode.Continue)
            => new()
            {
                IsSuccess = true,
                Title = title,
                Message = message,
                StatusCode = statusCode,
                Status = OperationResultStatus.Info
            };

    public static OperationResult NotFound(
        string message,
        string title = "Not Found")
        => Failure(message, title, HttpStatusCode.NotFound);

    public static OperationResult Unauthorized(
        string message = "You are not authorized to perform this action.",
        string title = "Unauthorized")
        => Failure(message, title, HttpStatusCode.Unauthorized);

    public static OperationResult Conflict(
        string message,
        string title = "Conflict")
        => Failure(message, title, HttpStatusCode.Conflict);

    public static OperationResult ValidationFailure(
        string message,
        IEnumerable<string> errors,
        string title = "Validation Failed")
        => Failure(message, title, HttpStatusCode.UnprocessableEntity, errors);

    /// <summary>Converts to IResult for minimal API endpoints.</summary>

}

public enum OperationResultStatus
{
    Success,
    Error,
    Info
}