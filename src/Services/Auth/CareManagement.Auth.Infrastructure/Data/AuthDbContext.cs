using CareManagement.Auth.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareManagement.Auth.Infrastructure.Data;

public class AuthDbContext : IdentityDbContext<User, Role, int>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User entity
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Configure Role entity
        builder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Rename Identity tables
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        // Seed default roles
        SeedRoles(builder);

        // Seed default users
        SeedUsers(builder);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        var now = DateTime.UtcNow;

        builder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
                Description = "System administrator with full access",
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            },
            new Role
            {
                Id = 2,
                Name = "Manager",
                NormalizedName = "MANAGER",
                Description = "Care manager with management privileges",
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            },
            new Role
            {
                Id = 3,
                Name = "Staff",
                NormalizedName = "STAFF",
                Description = "Care staff member",
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            },
            new Role
            {
                Id = 4,
                Name = "Supervisor",
                NormalizedName = "SUPERVISOR",
                Description = "Supervisor with oversight responsibilities",
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            }
        );
    }

    private static void SeedUsers(ModelBuilder builder)
    {
        var now = DateTime.UtcNow;
        var hasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@caremanagement.com",
            NormalizedEmail = "ADMIN@CAREMANAGEMENT.COM",
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

        builder.Entity<User>().HasData(adminUser);

        // Assign admin user to Administrator role
        builder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 1 // Administrator role
            }
        );
    }
}
