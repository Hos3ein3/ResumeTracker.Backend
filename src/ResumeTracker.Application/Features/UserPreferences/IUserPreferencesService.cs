

using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.UserPreferences;

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

public interface IUserPreferencesService
{
    Task<OperationResult<UserPreferencesResponse>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(
        Guid userId,
        UpdateUserPreferencesRequest request,
        CancellationToken cancellationToken = default);
}
