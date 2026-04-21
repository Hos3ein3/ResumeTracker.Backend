

using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;

namespace ResumeTracker.Application.Features.Auth;





public interface IAuthService
{
    Task<OperationResult<AuthResponse>> RegisterAsync(
        RegisterRequest request, string ipAddress, CancellationToken ct = default);

    Task<OperationResult<AuthResponse>> LoginAsync(
        LoginRequest request, string ipAddress, CancellationToken ct = default);

    Task<OperationResult<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request, string ipAddress, CancellationToken ct = default);

    Task<OperationResult> LogoutAsync(
        LogoutRequest request, string ipAddress, CancellationToken ct = default);

}
