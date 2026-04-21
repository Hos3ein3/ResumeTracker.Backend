

using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.Auth.Register;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PreferredLanguage,
    string? TimeZone);

public record RegisterResponse(
    Guid UserId,
    string Email,
    string FullName
);

public interface IAuthService
{
    Task<OperationResult<RegisterResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default);
}
