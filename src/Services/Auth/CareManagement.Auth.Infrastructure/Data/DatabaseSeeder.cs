using CareManagement.Auth.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareManagement.Auth.Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly AuthDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        AuthDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Seed roles if they don't exist
            await SeedRolesAsync();

            // Seed admin user if it doesn't exist
            await SeedAdminUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new { Name = "Administrator", Description = "System administrator with full access" },
            new { Name = "Manager", Description = "Care manager with management privileges" },
            new { Name = "Staff", Description = "Care staff member" },
            new { Name = "Supervisor", Description = "Supervisor with oversight responsibilities" }
        };

        foreach (var roleInfo in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleInfo.Name))
            {
                var role = new Role
                {
                    Name = roleInfo.Name,
                    Description = roleInfo.Description,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {RoleName}", roleInfo.Name);
                }
                else
                {
                    _logger.LogError("Failed to create role: {RoleName}. Errors: {Errors}",
                        roleInfo.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        const string adminEmail = "admin@caremanagement.com";
        const string adminPassword = "Admin123!";

        var existingUser = await _userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Add to Administrator role
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Administrator");
                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Created admin user: {Email}", adminEmail);
                    _logger.LogInformation("Admin user credentials - Email: {Email}, Password: {Password}",
                        adminEmail, adminPassword);
                }
                else
                {
                    _logger.LogError("Failed to assign Administrator role to admin user. Errors: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogError("Failed to create admin user. Errors: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            _logger.LogInformation("Admin user already exists: {Email}", adminEmail);
        }
    }
}
