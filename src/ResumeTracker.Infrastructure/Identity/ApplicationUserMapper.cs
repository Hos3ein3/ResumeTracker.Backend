

using ResumeTracker.Domain.Entities.Users;
using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Infrastructure.Identity;

public static class ApplicationUserMapper
{
    // Domain User → ApplicationUser (for creation)
    public static ApplicationUser ToApplicationUser(this User user)
        => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            CreatedAtUtc = user.CreatedAtUtc,
            SecurityStamp = user.SecurityStamp,
        };

    // ApplicationUser → Domain User (for queries)
    public static User ToDomainUser(this ApplicationUser user)
       => new(user.Id)
       {
           UserName = user.UserName,
           Email = user.Email ?? "",
           PhoneNumber = user.PhoneNumber ?? "",
           FirstName = user.FirstName ?? string.Empty,
           LastName = user.LastName ?? string.Empty,
           SecurityStamp = user.SecurityStamp,
       };
}
