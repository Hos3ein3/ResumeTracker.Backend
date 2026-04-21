using Microsoft.AspNetCore.Identity;

using ResumeTracker.Persistence.Identity;

namespace ResumeTracker.Persistence.Seeds;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    // ─────────────────────────────────────────────
    // Roles
    // ─────────────────────────────────────────────
    private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
        var roles = new[]
        {
            new ApplicationRole
            {
                Id = SeedConstants.Roles.AdminId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Priority = 1
            },
            new ApplicationRole
            {
                Id = SeedConstants.Roles.DeveloperId,
                Name = "Developer",
                NormalizedName = "DEVELOPER",
                Priority = 2
            },
            new ApplicationRole
            {
                Id = SeedConstants.Roles.SystemId,
                Name = "System",
                NormalizedName = "SYSTEM",
                Priority = 3
            }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    // ─────────────────────────────────────────────
    // Users
    // ─────────────────────────────────────────────
    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new[]
        {
            new
            {
                User = new ApplicationUser
                {
                    Id            = SeedConstants.Users.AdminId,
                    UserName      = "admin@resumetracker.io",
                    NormalizedUserName = "ADMIN@RESUMETRACKER.IO",
                    Email         = "admin@resumetracker.io",
                    NormalizedEmail = "ADMIN@RESUMETRACKER.IO",
                    EmailConfirmed = true,
                    FirstName     = "System",
                    LastName      = "Admin"
                },
                Password = "Admin@123456",
                Role     = "Admin"
            },
            new
            {
                User = new ApplicationUser
                {
                    Id            = SeedConstants.Users.DeveloperId,
                    UserName      = "developer@resumetracker.io",
                    NormalizedUserName = "DEVELOPER@RESUMETRACKER.IO",
                    Email         = "developer@resumetracker.io",
                    NormalizedEmail = "DEVELOPER@RESUMETRACKER.IO",
                    EmailConfirmed = true,
                    FirstName     = "System",
                    LastName      = "Developer"
                },
                Password = "Developer@123456",
                Role     = "Developer"
            },
            new
            {
                User = new ApplicationUser
                {
                    Id            = SeedConstants.Users.SystemId,
                    UserName      = "system@resumetracker.io",
                    NormalizedUserName = "SYSTEM@RESUMETRACKER.IO",
                    Email         = "system@resumetracker.io",
                    NormalizedEmail = "SYSTEM@RESUMETRACKER.IO",
                    EmailConfirmed = true,
                    FirstName     = "System",
                    LastName      = "Service"
                },
                Password = "System@123456",
                Role     = "System"
            }
        };

        foreach (var entry in users)
        {
            var existing = await userManager.FindByEmailAsync(entry.User.Email!);
            if (existing is not null)
                continue;

            var result = await userManager.CreateAsync(entry.User, entry.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(entry.User, entry.Role);
            }
        }
    }
}