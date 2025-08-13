using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CareManagement.Auth.Api.Data;
using CareManagement.Auth.Api.DTOs;
using CareManagement.Auth.Api.Services.Interfaces;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;

namespace CareManagement.Auth.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMessageBus _messageBus;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IMessageBus messageBus,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _messageBus = messageBus;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !user.IsActive)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };

            return new LoginResponse
            {
                Token = token,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            throw;
        }
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var user = _mapper.Map<ApplicationUser>(request);

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            // Create role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(request.Role));
            }

            // Assign role to user
            await _userManager.AddToRoleAsync(user, request.Role);

            // Publish user created event
            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _messageBus.PublishAsync("user_created", userCreatedEvent);

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", request.Username);
            throw;
        }
    }

    public async Task<UserDto?> GetProfileAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> AdminResetPasswordAsync(int userId, AdminChangePasswordRequest request, string adminUserId)
    {
        try
        {
            var targetUser = await _userManager.FindByIdAsync(userId.ToString());
            if (targetUser == null)
            {
                return false;
            }

            // Generate a password reset token and use it to reset the password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(targetUser);
            var result = await _userManager.ResetPasswordAsync(targetUser, resetToken, request.NewPassword);

            if (result.Succeeded)
            {
                // Update the user's updated timestamp
                targetUser.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(targetUser);

                _logger.LogInformation("Admin {AdminId} reset password for user {UserId}", adminUserId, userId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", userId);
            throw;
        }
    }
}
