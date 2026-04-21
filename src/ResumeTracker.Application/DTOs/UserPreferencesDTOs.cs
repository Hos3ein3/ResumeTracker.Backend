
namespace ResumeTracker.Application.DTOs;


public sealed record UpdateUserPreferencesRequest(
    string? Language,
    string? Theme,
    string? TimeZone,
    string? DateFormat,
    bool? UseRtlLayout,
    bool? EmailNotifications,
    bool? PushNotifications,
    bool? ResumeViewAlerts,
    int? DefaultPageSize,
    string? DefaultSortBy,
    string? DefaultSortOrder
);
public sealed record UserPreferencesResponse(
    Guid UserId,
    string Language,
    string Theme,
    string TimeZone,
    string DateFormat,
    bool UseRtlLayout,
    bool EmailNotifications,
    bool PushNotifications,
    bool ResumeViewAlerts,
    int DefaultPageSize,
    string DefaultSortBy,
    string DefaultSortOrder,
    DateTime UpdatedAtUtc
);
