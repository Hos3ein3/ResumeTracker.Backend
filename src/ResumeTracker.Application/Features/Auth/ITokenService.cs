
using ResumeTracker.Application.Abstractions.Auth;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;

namespace ResumeTracker.Application.Features.Auth;

public interface ITokenService
{
    string GenerateAccessToken(TokenUserInfo user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromExpiredToken(string accessToken);

    Task<OperationResult<AuthResponse>> IssueTokenPairAsync(
        User user,
        CancellationToken ct,
        string? preGeneratedRefreshToken = null);
}