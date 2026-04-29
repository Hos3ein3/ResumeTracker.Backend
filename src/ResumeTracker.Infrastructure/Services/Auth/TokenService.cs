using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using ResumeTracker.Application.Abstractions;
using ResumeTracker.Application.Abstractions.Auth;
using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.DTOs.Auth;
using ResumeTracker.Application.Features.Auth;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Entities.Users;
using ResumeTracker.Domain.Exceptions;
using ResumeTracker.Infrastructure.Identity;
using ResumeTracker.Infrastructure.Settings;

namespace ResumeTracker.Infrastructure.Auth;

public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly IIdentityService  _identityService;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtSettings> settings, IIdentityService identityService, IRefreshTokenRepository refreshTokenRepo, IUnitOfWork uow, ILogger<TokenService> logger)
    {
        _identityService = identityService;
        _refreshTokenRepo = refreshTokenRepo;
        _uow = uow;
        _logger = logger;
        _settings = settings.Value;
    }

    public string GenerateAccessToken(TokenUserInfo user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new("firstName", user.FirstName),
            new("lastName",  user.LastName)
        };

        claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public Guid? GetUserIdFromExpiredToken(string accessToken)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(_settings.SecretKey)),
            ValidateLifetime = false   // ← intentional for refresh flow
        };

        try
        {
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(accessToken, parameters, out var validated);

            if (validated is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(
                    SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                return null;

            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return sub is null ? null : Guid.Parse(sub);
        }
        catch { return null; }
    }
    
    public async Task<OperationResult<AuthResponse>> IssueTokenPairAsync(User user, CancellationToken ct, string? preGeneratedRefreshToken = null)
    {
        var roles = await _identityService.GetRolesAsync(user);

        if (roles.Data is null) return OperationResult<AuthResponse>.NotFound(Error.NotFound);
        
        var tokenUser = new TokenUserInfo(
            Id: user.Id,
            Email: user.Email!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles.Data.ToList());
        

        var accessToken = this.GenerateAccessToken(tokenUser);
        var refreshRaw = preGeneratedRefreshToken ?? this.GenerateRefreshToken();

        var refreshData = new RefreshTokenData
        {
            Id = Guid.NewGuid(),
            Token = refreshRaw,
            UserId = user.Id,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays)
        };

        await _refreshTokenRepo.AddAsync(refreshData, ct);
        await _uow.SaveChangesAsync(ct);

        return OperationResult<AuthResponse>.Success(new AuthResponse(
            UserId: user.Id,
            Username:user.UserName,
            PhoneNumber:user.PhoneNumber,
            Email: user.Email!,
            FullName: $"{user.FirstName} {user.LastName}",
            Roles: roles.Data.ToList(),
            AccessToken: accessToken,
            AccessTokenExpiresAtUtc: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            RefreshToken: refreshRaw,
            RefreshTokenExpiresAtUtc: refreshData.ExpiresAtUtc));
    }

}