
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
public sealed record CreateUserPreferencesRequest(
    string? Language,
    string? Theme = null,
    string? TimeZone = null,
    string? DateFormat = "DD/MM/YYYY",
    bool? UseRtlLayout = null,
    bool? EmailNotifications = null,
    bool? PushNotifications = null,
    bool? ResumeViewAlerts = null,
    int? DefaultPageSize = 10,
    string? DefaultSortBy = "Id",
    string? DefaultSortOrder = "asc"
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
    string DefaultSortOrder
);
