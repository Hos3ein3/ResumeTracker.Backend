

namespace ResumeTracker.Application.DTOs.Auth;



public record RegisterResponse(
Guid UserId,
string Email,
string FullName
);

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PreferredLanguage,
    string? TimeZone);

public sealed record LoginRequest(string Email, string Password);

public sealed record RefreshTokenRequest(string RefreshToken);


public record LogoutRequest(string RefreshToken);

public sealed record AuthResponse(
    Guid UserId,
    string Username,
    string PhoneNumber,
    string Email,
    string FullName,
    IReadOnlyList<string> Roles,
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc
);
