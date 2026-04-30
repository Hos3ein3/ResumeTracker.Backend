using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ResumeTracker.Domain.Common;

namespace ResumeTracker.Persistence.Identity;

/// <summary>
/// Custom UserManager that ensures every new user gets a GUID v7 as Id
/// before Identity touches the entity. Overrides both CreateAsync overloads.
/// </summary>
public sealed class ApplicationUserManager : UserManager<ApplicationUser>
{
    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger)
        : base(store, optionsAccessor, passwordHasher,
            userValidators, passwordValidators,
            keyNormalizer, errors, services, logger)
    {
    }

    public async Task<ApplicationUser?> FindByPhoneAsync(string phoneNumber)
        => await Users.FirstOrDefaultAsync(x => x.PhoneNumber.Equals(phoneNumber));

    public override Task<IdentityResult> CreateAsync(ApplicationUser user)
    {
        AssignGuidV7IfEmpty(user);
        return base.CreateAsync(user);
    }

    // Called when creating a user WITH a password (e.g. registration)
    public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        AssignGuidV7IfEmpty(user);
        return base.CreateAsync(user, password);
    }

    private static void AssignGuidV7IfEmpty(ApplicationUser user)
    {
        if (user.Id == Guid.Empty)
            user.Id = NewId.Next();
    }
}