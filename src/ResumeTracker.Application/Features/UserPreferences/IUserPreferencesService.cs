

using ResumeTracker.Application.DTOs;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.UserPreferences;

public interface IUserPreferencesService
{
    Task<OperationResult<UserPreferencesResponse>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(
        Guid userId,
        UpdateUserPreferencesRequest request,
        CancellationToken cancellationToken = default);

    Task<OperationResult>  CreateAsync(
        Guid userId,
CreateUserPreferencesRequest request,
        CancellationToken cancellationToken = default);

    CreateUserPreferencesRequest CreateObject(string? preferredLanguage,
    string? timeZone);
}
