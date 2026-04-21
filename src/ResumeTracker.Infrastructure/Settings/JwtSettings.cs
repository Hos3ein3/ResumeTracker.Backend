namespace ResumeTracker.Infrastructure.Settings;


public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 15;
    public int RefreshTokenDays { get; init; } = 7;
}