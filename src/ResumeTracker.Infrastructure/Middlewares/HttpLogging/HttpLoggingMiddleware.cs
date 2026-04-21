using System.Text;

using Microsoft.AspNetCore.Http;
using Microsoft.IO;

using Serilog;
using Serilog.Context;

namespace ResumeTracker.Infrastructure.Middlewares.HttpLogging;

public sealed class HttpLoggingMiddleware
{
    // RecyclableMemoryStreamManager avoids LOH allocations when buffering bodies
    private static readonly RecyclableMemoryStreamManager StreamManager = new();

    private readonly RequestDelegate _next;
    private readonly HttpLoggingOptions _options;
    private readonly ILogger _logger = Log.ForContext<HttpLoggingMiddleware>();

    public HttpLoggingMiddleware(RequestDelegate next, HttpLoggingOptions options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ── 1. Enable request body buffering (allows re-reading) ──
        context.Request.EnableBuffering();

        string? requestBody = null;
        string? responseBody = null;

        // ── 2. Read request body ──────────────────────────────────
        if (_options.LogRequestBody && IsLoggableContentType(context.Request.ContentType))
        {
            requestBody = await ReadRequestBodyAsync(context.Request);
        }

        // ── 3. Swap response stream so we can intercept the body ──
        var originalResponseStream = context.Response.Body;
        await using var bufferedResponseStream = StreamManager.GetStream();
        context.Response.Body = bufferedResponseStream;

        try
        {
            await _next(context);
        }
        finally
        {
            // ── 4. Read response body ─────────────────────────────
            if (_options.LogResponseBody && IsLoggableContentType(context.Response.ContentType))
            {
                responseBody = await ReadResponseBodyAsync(context.Response);
            }

            // ── 5. Copy intercepted response back to original stream ──
            bufferedResponseStream.Position = 0;
            await bufferedResponseStream.CopyToAsync(originalResponseStream);
            context.Response.Body = originalResponseStream;

            // ── 6. Decide whether to log ──────────────────────────
            if (!_options.LogOnlyErrors && !(context.Response.StatusCode < 400))
               LogRequest(context, requestBody, responseBody);

            
            
        }
    }

    // ─────────────────────────────────────────────────────────────
    private void LogRequest(
        HttpContext context,
        string? requestBody,
        string? responseBody)
    {
        var request = context.Request;
        var response = context.Response;

        // Request headers — redact sensitive ones
        var requestHeaders = _options.LogRequestHeaders
            ? SanitizeHeaders(request.Headers)
            : null;

        // Response headers
        var responseHeaders = _options.LogResponseHeaders
            ? SanitizeHeaders(response.Headers)
            : null;

        // Query string
        var queryString = _options.LogQueryString && request.QueryString.HasValue
            ? ParseQueryString(request.QueryString.Value)
            : null;

        // Route values
        var routeValues = context.Request.RouteValues.Count > 0
            ? context.Request.RouteValues
                .ToDictionary(kv => kv.Key, kv => kv.Value?.ToString())
            : null;

        using (LogContext.PushProperty("Http.RequestMethod", request.Method))
        using (LogContext.PushProperty("Http.RequestPath", request.Path.Value))
        using (LogContext.PushProperty("Http.QueryString", queryString))
        using (LogContext.PushProperty("Http.RequestHeaders", requestHeaders))
        using (LogContext.PushProperty("Http.RequestBody", Truncate(requestBody)))
        using (LogContext.PushProperty("Http.RouteValues", routeValues))
        using (LogContext.PushProperty("Http.StatusCode", response.StatusCode))
        using (LogContext.PushProperty("Http.ResponseHeaders", responseHeaders))
        using (LogContext.PushProperty("Http.ResponseBody", Truncate(responseBody)))
        using (LogContext.PushProperty("Http.ContentType", request.ContentType))
        using (LogContext.PushProperty("Http.Protocol", request.Protocol))
        {
            var level = response.StatusCode >= 500 ? Serilog.Events.LogEventLevel.Error
                      : response.StatusCode >= 400 ? Serilog.Events.LogEventLevel.Warning
                      : Serilog.Events.LogEventLevel.Information;

            _logger.Write(level,
                "HTTP {Http.RequestMethod} {Http.RequestPath} → {Http.StatusCode}",
                request.Method,
                request.Path.Value,
                response.StatusCode);
        }
    }

    // ─────────────────────────────────────────────────────────────
    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == 0)
            return null;

        request.Body.Position = 0;
        using var reader = new StreamReader(
            request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;   // rewind so the actual handler can read it

        return body;
    }

    private static async Task<string?> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Position = 0;
        using var reader = new StreamReader(
            response.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        response.Body.Position = 0;  // rewind so it can be copied back to the client

        return body;
    }

    private Dictionary<string, string> SanitizeHeaders(IHeaderDictionary headers)
    {
        return headers
            .Where(h => !_options.RedactedHeaders.Contains(
                h.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(
                h => h.Key,
                h => h.Value.ToString());
    }

    private static Dictionary<string, string> ParseQueryString(string? queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
            return [];

        return Microsoft.AspNetCore.WebUtilities.QueryHelpers
            .ParseQuery(queryString)
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
    }

    private string? Truncate(string? value)
    {
        if (value is null) return null;
        return value.Length <= _options.MaxBodySizeBytes
            ? value
            : value[.._options.MaxBodySizeBytes] + $"... [TRUNCATED — original size: {value.Length} bytes]";
    }

    private bool IsLoggableContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return _options.LoggableContentTypes.Any(loggable =>
            contentType.Contains(loggable, StringComparison.OrdinalIgnoreCase));
    }
}