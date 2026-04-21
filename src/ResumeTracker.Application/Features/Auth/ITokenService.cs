
using ResumeTracker.Application.Abstractions.Auth;

namespace ResumeTracker.Application.Features.Auth;

public interface ITokenService
{
    string GenerateAccessToken(TokenUserInfo user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromExpiredToken(string accessToken);
}