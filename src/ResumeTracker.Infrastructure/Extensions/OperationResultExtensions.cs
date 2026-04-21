

using Microsoft.AspNetCore.Http;

using ResumeTracker.Domain.Common;

namespace ResumeTracker.Infrastructure.Extensions;

public static class OperationResultExtensions
{
    public static IResult ToHttpResult(this OperationResult result)
        => result.IsSuccess
            ? Results.Ok(new { result.Title, result.Message })
            : Results.Problem(
                title: result.Title,
                detail: result.Message,
                statusCode: (int)result.StatusCode,
                extensions: result.Errors.Count > 0
                    ? new Dictionary<string, object?> { ["errors"] = result.Errors }
                    : null);

    public static IResult ToHttpResult<T>(this OperationResult<T> result)
        => result.IsSuccess
            ? Results.Json(result.Data, statusCode: (int)result.StatusCode)
            : Results.Problem(
                title: result.Title,
                detail: result.Message,
                statusCode: (int)result.StatusCode,
                extensions: result.Errors.Count > 0
                    ? new Dictionary<string, object?> { ["errors"] = result.Errors }
                    : null);
}