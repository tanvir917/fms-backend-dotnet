using CareManagement.Auth.Core.Entities;
using CareManagement.Auth.Core.Repositories;
using CareManagement.Auth.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace CareManagement.Auth.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
    }

    public async Task<User> RegisterAsync(string email, string password, string firstName, string lastName)
    {
        if (await _userRepository.EmailExistsAsync(email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Email = email,
            UserName = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true // For simplicity, set as confirmed
        };

        return await _userRepository.CreateAsync(user, password);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        return _tokenService.ValidateToken(token);
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        // In a real implementation, you would validate the refresh token
        // and retrieve the user associated with it. For now, this is a placeholder.
        throw new NotImplementedException("Refresh token functionality not yet implemented");
    }

    public async Task LogoutAsync(string token)
    {
        // In a real implementation, you would blacklist the token
        // For now, this is a placeholder
        await Task.CompletedTask;
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        // Remove current password and set new one
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        return result.Succeeded;
    }
}
