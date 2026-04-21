using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions.Localization;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.DTOs;
using ResumeTracker.Application.Features.UserPreferences;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Infrastructure.Services.UserPreferences;

public sealed class UserPreferencesService : IUserPreferencesService
{
    private readonly IUserPreferencesRepository _repository;
    private readonly IMessageLocalizer _localizer;
    private readonly ILogger<UserPreferencesService> _logger;

    public UserPreferencesService(
        IUserPreferencesRepository repository,
        IMessageLocalizer localizer,
        ILogger<UserPreferencesService> logger)
    {
        _repository = repository;
        _localizer = localizer;
        _logger = logger;
    }

    public async Task<OperationResult> CreateAsync(Guid userId, UpdateUserPreferencesRequest request, CancellationToken cancellationToken = default)
    {
        var preferences = await _repository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is not null)
        {
            _logger.LogWarning(
                "Preferences already exist for user {UserId}", userId);
            return OperationResult<UserPreferencesResponse>.Conflict(
                _localizer.UserPreferencesResources("Preferences.Conflict", userId));
        }

        preferences = new();


        if (request.Language is not null) preferences.Language = request.Language;
        if (request.Theme is not null) preferences.Theme = request.Theme;
        if (request.TimeZone is not null) preferences.TimeZone = request.TimeZone;
        if (request.DateFormat is not null) preferences.DateFormat = request.DateFormat;
        if (request.UseRtlLayout is not null) preferences.UseRtlLayout = request.UseRtlLayout.Value;
        if (request.EmailNotifications is not null) preferences.EmailNotifications = request.EmailNotifications.Value;
        if (request.PushNotifications is not null) preferences.PushNotifications = request.PushNotifications.Value;
        if (request.ResumeViewAlerts is not null) preferences.ResumeViewAlerts = request.ResumeViewAlerts.Value;
        if (request.DefaultPageSize is not null) preferences.DefaultPageSize = request.DefaultPageSize.Value;
        if (request.DefaultSortBy is not null) preferences.DefaultSortBy = request.DefaultSortBy;
        if (request.DefaultSortOrder is not null) preferences.DefaultSortOrder = request.DefaultSortOrder;

        // Auto-set RTL when language is Persian
        if (request.Language is not null)
            preferences.UseRtlLayout = request.Language == "fa";

        await _repository.CreateAsync(preferences, cancellationToken);

        return OperationResult<UserPreferencesResponse>.Success();
    }

    public async Task<OperationResult<UserPreferencesResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var preferences = await _repository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            _logger.LogWarning("Preferences not found for user {UserId}", userId);
            return OperationResult<UserPreferencesResponse>.NotFound(
                _localizer.UserPreferencesResources("Preferences.NotFound", userId));
        }

        var response = new UserPreferencesResponse(
            UserId: preferences.UserId,
            Language: preferences.Language,
            Theme: preferences.Theme,
            TimeZone: preferences.TimeZone,
            DateFormat: preferences.DateFormat,
            UseRtlLayout: preferences.UseRtlLayout,
            EmailNotifications: preferences.EmailNotifications,
            PushNotifications: preferences.PushNotifications,
            ResumeViewAlerts: preferences.ResumeViewAlerts,
            DefaultPageSize: preferences.DefaultPageSize,
            DefaultSortBy: preferences.DefaultSortBy,
            DefaultSortOrder: preferences.DefaultSortOrder,
            UpdatedAtUtc: preferences.UpdatedAtUtc);

        return OperationResult<UserPreferencesResponse>.Success(response);
    }

    public async Task<OperationResult> UpdateAsync(
        Guid userId,
        UpdateUserPreferencesRequest request,
        CancellationToken cancellationToken = default)
    {
        var preferences = await _repository.GetByUserIdAsync(userId, cancellationToken);

        if (preferences is null)
        {
            _logger.LogWarning(
                "Update preferences failed — not found for user {UserId}", userId);
            return OperationResult.NotFound(
                _localizer.UserPreferencesResources("Preferences.NotFound", userId));
        }

        // ── Apply only the fields that were provided (PATCH semantics) ──
        if (request.Language is not null) preferences.Language = request.Language;
        if (request.Theme is not null) preferences.Theme = request.Theme;
        if (request.TimeZone is not null) preferences.TimeZone = request.TimeZone;
        if (request.DateFormat is not null) preferences.DateFormat = request.DateFormat;
        if (request.UseRtlLayout is not null) preferences.UseRtlLayout = request.UseRtlLayout.Value;
        if (request.EmailNotifications is not null) preferences.EmailNotifications = request.EmailNotifications.Value;
        if (request.PushNotifications is not null) preferences.PushNotifications = request.PushNotifications.Value;
        if (request.ResumeViewAlerts is not null) preferences.ResumeViewAlerts = request.ResumeViewAlerts.Value;
        if (request.DefaultPageSize is not null) preferences.DefaultPageSize = request.DefaultPageSize.Value;
        if (request.DefaultSortBy is not null) preferences.DefaultSortBy = request.DefaultSortBy;
        if (request.DefaultSortOrder is not null) preferences.DefaultSortOrder = request.DefaultSortOrder;

        // Auto-set RTL when language is Persian
        if (request.Language is not null)
            preferences.UseRtlLayout = request.Language == "fa";

        await _repository.UpdateAsync(preferences, cancellationToken);

        _logger.LogInformation("Preferences updated for user {UserId}", userId);

        return OperationResult.Success(
            _localizer.UserPreferencesResources("Preferences.UpdatedSuccessfully"));
    }
}