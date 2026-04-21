
using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Domain.Exceptions;

namespace ResumeTracker.Infrastructure.Middlewares.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;
    private readonly IServiceScopeFactory _scopeFactory;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment env,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _env = env;
        _scopeFactory = scopeFactory;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log with full detail regardless of environment
        _logger.LogError(
            exception,
            "Unhandled exception on {Method} {Path} — {ExceptionType}: {ExceptionMessage}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            exception.GetType().Name,
            exception.Message);

        await using var scope = _scopeFactory.CreateAsyncScope();
        var localizer = scope.ServiceProvider.GetRequiredService<IMessageLocalizer>();
        var (statusCode, title, message) = MapException(exception, localizer);

        var problemDetails = BuildProblemDetails(
            httpContext, exception, statusCode, title, message);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase },
            cancellationToken);

        return true;   // ← tells ASP.NET Core the exception was handled
    }

    // ─────────────────────────────────────────────────────────────
    private ProblemDetails BuildProblemDetails(
        HttpContext httpContext,
        Exception exception,
        HttpStatusCode statusCode,
        string title,
        string message)
    {
        var traceId = httpContext.TraceIdentifier;
        var correlationId = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                            ?? traceId;

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        // Always include tracing identifiers so Seq queries are easy
        problem.Extensions["traceId"] = traceId;
        problem.Extensions["correlationId"] = correlationId;
        problem.Extensions["timestamp"] = DateTime.UtcNow;

        // In Development: expose stack trace and inner exception
        if (_env.IsDevelopment())
        {
            problem.Extensions["exceptionType"] = exception.GetType().FullName;
            problem.Extensions["stackTrace"] = exception.StackTrace;
            problem.Extensions["innerException"] = exception.InnerException?.Message;
        }

        return problem;
    }

    // ─────────────────────────────────────────────────────────────
    private (HttpStatusCode StatusCode, string Title, string Message) MapException(
        Exception exception, IMessageLocalizer _localizer)
    {
        return exception switch
        {
            // ── 400 Bad Request ───────────────────────────────────
            ArgumentException or
            ArgumentNullException or
            InvalidOperationException
                when !IsCritical(exception)
                => (HttpStatusCode.BadRequest,
                    _localizer.ExceptionResources("Exception.BadRequest.Title"),
                    exception.Message),

            // ── 401 Unauthorized ──────────────────────────────────
            UnauthorizedAccessException
                => (HttpStatusCode.Unauthorized,
                    _localizer.ExceptionResources("Exception.Unauthorized.Title"),
                    _localizer.ExceptionResources("Exception.Unauthorized.Message")),

            // ── 403 Forbidden ─────────────────────────────────────
            ForbiddenAccessException
                => (HttpStatusCode.Forbidden,
                    _localizer.ExceptionResources("Exception.Forbidden.Title"),
                    _localizer.ExceptionResources("Exception.Forbidden.Message")),

            // ── 404 Not Found ─────────────────────────────────────
            NotFoundException e
                => (HttpStatusCode.NotFound,
                    _localizer.ExceptionResources("Exception.NotFound.Title"),
                    e.Message),

            // ── 409 Conflict ──────────────────────────────────────
            ConflictException e
                => (HttpStatusCode.Conflict,
                    _localizer.ExceptionResources("Exception.Conflict.Title"),
                    e.Message),

            // ── 422 Unprocessable ─────────────────────────────────
            ValidationException e
                => (HttpStatusCode.UnprocessableEntity,
                    _localizer.ExceptionResources("Exception.Validation.Title"),
                    e.Message),

            // ── 408 Timeout ───────────────────────────────────────
            TimeoutException or
            TaskCanceledException or
            OperationCanceledException
                => (HttpStatusCode.RequestTimeout,
                    _localizer.ExceptionResources("Exception.Timeout.Title"),
                    _localizer.ExceptionResources("Exception.Timeout.Message")),

            // ── 503 External service ──────────────────────────────
            HttpRequestException
                => (HttpStatusCode.ServiceUnavailable,
                    _localizer.ExceptionResources("Exception.ServiceUnavailable.Title"),
                    _localizer.ExceptionResources("Exception.ServiceUnavailable.Message")),

            // ── 500 Catch-all ─────────────────────────────────────
            _
                => (HttpStatusCode.InternalServerError,
                    _localizer.ExceptionResources("Exception.Internal.Title"),
                    _localizer.ExceptionResources("Exception.Internal.Message"))
        };
    }

    private static bool IsCritical(Exception e)
        => e.Message.Contains("connection", StringComparison.OrdinalIgnoreCase)
           || e.Message.Contains("database", StringComparison.OrdinalIgnoreCase);
}