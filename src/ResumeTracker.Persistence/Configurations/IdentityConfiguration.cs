using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ResumeTracker.Persistence.Configurations;

public static class IdentityConfiguration
{
    public static ModelBuilder buildIdentityConfiguration(this ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<Guid>>()
                .ToTable("UserRoles", DatabaseSchemas.Identity);

        builder.Entity<IdentityUserClaim<Guid>>()
                .ToTable("UserClaims", DatabaseSchemas.Identity);

        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("UserLogins", DatabaseSchemas.Identity);

        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable("UserTokens", DatabaseSchemas.Identity);

        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("RoleClaims", DatabaseSchemas.Identity);

        return builder;
    }
}
