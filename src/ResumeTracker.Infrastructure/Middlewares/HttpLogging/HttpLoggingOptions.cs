namespace ResumeTracker.Infrastructure.Middlewares.HttpLogging;

public sealed record HttpLoggingOptions
{
    /// <summary>Log request headers (excludes sensitive ones by default).</summary>
    public bool LogRequestHeaders { get; init; } = true;

    /// <summary>Log request body (JSON, form data).</summary>
    public bool LogRequestBody { get; init; } = true;

    /// <summary>Log query string parameters.</summary>
    public bool LogQueryString { get; init; } = true;

    /// <summary>Log response headers.</summary>
    public bool LogResponseHeaders { get; init; } = false;

    /// <summary>Log response body.</summary>
    public bool LogResponseBody { get; init; } = true;

    /// <summary>Max body size to log in bytes. Larger bodies are truncated. Default 32 KB.</summary>
    public int MaxBodySizeBytes { get; init; } = 32 * 1024;

    /// <summary>
    /// Content types whose body will be logged.
    /// Binary, multipart, and file uploads are always skipped.
    /// </summary>
    public string[] LoggableContentTypes { get; init; } =
    [
        "application/json",
        "application/x-www-form-urlencoded",
        "text/plain",
        "text/xml",
        "application/xml"
    ];

    /// <summary>
    /// Headers that will NEVER be logged — security-sensitive values.
    /// </summary>
    public string[] RedactedHeaders { get; init; } =
    [
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key",
        "X-Auth-Token",
        "Proxy-Authorization"
    ];

    /// <summary>
    /// When true, only log requests that resulted in 4xx or 5xx responses.
    /// Useful in production to reduce noise.
    /// </summary>
    public bool LogOnlyErrors { get; init; } = false;
}