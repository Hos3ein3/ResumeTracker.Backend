


using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using ResumeTracker.Application.Abstractions.Persistence;
using ResumeTracker.Application.Features.Auth.Register;
using ResumeTracker.Domain.Common;
using ResumeTracker.Domain.Documents;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Infrastructure.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IUserPreferencesRepository preferencesRepository,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _preferencesRepository = preferencesRepository;
        _logger = logger;
    }


    public async Task<OperationResult<RegisterResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // ── 1. Check if email already taken ───────────────────────
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            _logger.LogWarning("Registration attempt with existing email {Email}", request.Email);
            return OperationResult<RegisterResponse>.Conflict(
                $"An account with email '{request.Email}' already exists.");
        }

        // ── 2. Create Identity user ───────────────────────────────
        var user = new ApplicationUser
        {
            // Id is set to Guid v7 by ApplicationUserManager.CreateAsync
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email   // Identity requires UserName
        };

        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors
                .Select(e => e.Description)
                .ToList();

            _logger.LogWarning(
                "Registration failed for {Email}. Errors: {Errors}",
                request.Email, string.Join(", ", errors));

            return OperationResult<RegisterResponse>.ValidationFailure(
                "Registration failed due to validation errors.",
                errors);
        }

        // ── 3. Create default UserPreferences ─────────────────────
        // Runs AFTER user creation — if this fails, log it but don't
        // fail the registration (preferences can be created later)
        try
        {
            var preferences = new ResumeTracker.Domain.Documents.UserPreferences
            {
                UserId = user.Id,
                Language = request.PreferredLanguage ?? "en",
                TimeZone = request.TimeZone ?? "UTC",
                UseRtlLayout = request.PreferredLanguage == "fa"
            };

            await _preferencesRepository.CreateAsync(preferences, cancellationToken);

            _logger.LogInformation(
                "User {UserId} registered and preferences created.", user.Id);
        }
        catch (Exception ex)
        {
            // Non-fatal — user is created, preferences will fallback to defaults
            _logger.LogError(ex,
                "Failed to create preferences for user {UserId}. " +
                "User was registered successfully.", user.Id);
        }

        // ── 4. Return success ──────────────────────────────────────
        return OperationResult<RegisterResponse>.Created(
            new RegisterResponse(
                UserId: user.Id,
                Email: user.Email!,
                FullName: $"{user.FirstName} {user.LastName}"),
            message: "Account created successfully. Welcome to ResumeTracker!");
    }
}
