namespace ResumeTracker.Application.Abstractions.Auth;

/// <summary>
/// Carries only the data needed to generate a JWT.
/// Keeps Application layer free of Persistence types.
/// </summary>
public sealed record TokenUserInfo(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles
);