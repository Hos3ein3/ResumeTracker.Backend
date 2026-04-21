using System.Net;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ResumeTracker.Application.Abstractions.Auth;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Infrastructure.Auth;
using ResumeTracker.Infrastructure.Settings;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Infrastructure.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IUnitOfWork _uow;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepo,
        IUnitOfWork uow,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepo = refreshTokenRepo;
        _uow = uow;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    // ── Register ──────────────────────────────────────────────────
    public async Task<OperationResult<AuthResponse>> RegisterAsync(
        RegisterRequest request, string ipAddress, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            return OperationResult<AuthResponse>.Conflict(
                $"An account with email '{request.Email}' already exists.");

        var user = new ApplicationUser     // ← Persistence type, fine in Infrastructure
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return OperationResult<AuthResponse>.ValidationFailure(
                "Registration failed.",
                result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "User");
        _logger.LogInformation("User {UserId} registered from {IP}", user.Id, ipAddress);

        return await IssueTokenPairAsync(user, ipAddress, ct);
    }

    // ── Login ─────────────────────────────────────────────────────
    public async Task<OperationResult<AuthResponse>> LoginAsync(
        LoginRequest request, string ipAddress, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Failed login for {Email} from {IP}", request.Email, ipAddress);
            return OperationResult<AuthResponse>.Failure(
                "Invalid email or password.",
                statusCode: HttpStatusCode.Unauthorized);
        }

        if (await _userManager.IsLockedOutAsync(user))
            return OperationResult<AuthResponse>.Failure(
                "Account is temporarily locked. Please try again later.",
                statusCode: HttpStatusCode.Forbidden);

        _logger.LogInformation("User {UserId} logged in from {IP}", user.Id, ipAddress);
        return await IssueTokenPairAsync(user, ipAddress, ct);
    }

    // ── Refresh Token ─────────────────────────────────────────────
    public async Task<OperationResult<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request, string ipAddress, CancellationToken ct = default)
    {
        var stored = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null)
            return OperationResult<AuthResponse>.Failure(
                "Refresh token is invalid.",
                statusCode: HttpStatusCode.Unauthorized);

        if (stored.IsRevoked)
        {
            // Reuse detected — nuke all sessions
            _logger.LogWarning(
                "Token reuse detected for user {UserId} from {IP}. Revoking all sessions.",
                stored.UserId, ipAddress);

            await _refreshTokenRepo.RevokeAllByUserIdAsync(stored.UserId, ipAddress, ct);
            await _uow.SaveChangesAsync(ct);

            return OperationResult<AuthResponse>.Failure(
                "Token reuse detected. All sessions have been revoked. Please log in again.",
                statusCode: HttpStatusCode.Unauthorized);
        }

        if (stored.IsExpired)
            return OperationResult<AuthResponse>.Failure(
                "Refresh token has expired. Please log in again.",
                statusCode: HttpStatusCode.Unauthorized);

        var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
        if (user is null)
            return OperationResult<AuthResponse>.NotFound("User not found.");

        // Rotate — revoke old token
        var newRawToken = _tokenService.GenerateRefreshToken();
        stored.RevokedAtUtc = DateTime.UtcNow;
        stored.RevokedByIp = ipAddress;
        stored.ReplacedByToken = newRawToken;
        await _refreshTokenRepo.UpdateAsync(stored, ct);

        _logger.LogInformation("Token rotated for user {UserId}", user.Id);
        return await IssueTokenPairAsync(user, ipAddress, ct, newRawToken);
    }

    // ── Logout ────────────────────────────────────────────────────
    public async Task<OperationResult> LogoutAsync(
        LogoutRequest request, string ipAddress, CancellationToken ct = default)
    {
        var stored = await _refreshTokenRepo.GetByTokenAsync(request.RefreshToken, ct);

        if (stored is null || !stored.IsActive)
            return OperationResult.Failure(
                "Refresh token is invalid or already revoked.",
                statusCode: HttpStatusCode.BadRequest);

        stored.RevokedAtUtc = DateTime.UtcNow;
        stored.RevokedByIp = ipAddress;
        await _refreshTokenRepo.UpdateAsync(stored, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} logged out from {IP}", stored.UserId, ipAddress);
        return OperationResult.Success("Logged out successfully.");
    }

    // ── Private helper ────────────────────────────────────────────
    private async Task<OperationResult<AuthResponse>> IssueTokenPairAsync(
        ApplicationUser user,
        string ipAddress,
        CancellationToken ct,
        string? preGeneratedRefreshToken = null)
    {
        var roles = await _userManager.GetRolesAsync(user);

        // Map Persistence entity → Application DTO before crossing the boundary
        var tokenUser = new TokenUserInfo(
            Id: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles.ToList());

        var accessToken = _tokenService.GenerateAccessToken(tokenUser);
        var refreshRaw = preGeneratedRefreshToken ?? _tokenService.GenerateRefreshToken();

        var refreshData = new RefreshTokenData
        {
            Id = Guid.NewGuid(),
            Token = refreshRaw,
            UserId = user.Id,
            CreatedByIp = ipAddress,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays)
        };

        await _refreshTokenRepo.AddAsync(refreshData, ct);
        await _uow.SaveChangesAsync(ct);

        return OperationResult<AuthResponse>.Success(new AuthResponse(
            UserId: user.Id,
            Email: user.Email!,
            FullName: $"{user.FirstName} {user.LastName}",
            Roles: roles.ToList(),
            AccessToken: accessToken,
            AccessTokenExpiresAtUtc: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
            RefreshToken: refreshRaw,
            RefreshTokenExpiresAtUtc: refreshData.ExpiresAtUtc));
    }


}