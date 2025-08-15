using CareManagement.Auth.Application.DTOs;
using CareManagement.Auth.Application.Mappings;
using CareManagement.Auth.Core.Entities;
using CareManagement.Auth.Core.Services;
using CareManagement.Auth.Core.Repositories;

namespace CareManagement.Auth.Application.Services;

public class AuthApplicationService : IAuthApplicationService
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthApplicationService(IAuthService authService, ITokenService tokenService, IUserRepository userRepository)
    {
        _authService = authService;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var token = await _authService.AuthenticateAsync(request.Email, request.Password);
        var userId = _tokenService.GetUserIdFromToken(token);
        var expiresAt = _tokenService.GetTokenExpiry(token);

        // Get the full user data including roles
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidOperationException("User not found after authentication");
        }

        var roles = await _userRepository.GetRolesAsync(user);
        var userDto = user.ToDto(roles);

        var refreshToken = _tokenService.GenerateRefreshToken();

        return new LoginResponseDto
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = userDto
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        return user.ToDto();
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var newToken = await _authService.RefreshTokenAsync(request.RefreshToken);
        var userId = _tokenService.GetUserIdFromToken(newToken);
        var expiresAt = _tokenService.GetTokenExpiry(newToken);
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new LoginResponseDto
        {
            AccessToken = newToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto { Id = userId }
        };
    }

    public async Task<bool> LogoutAsync(string token)
    {
        await _authService.LogoutAsync(token);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        // This would need to be implemented in the core service
        // For now, return true as placeholder
        return true;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var result = await _authService.ResetPasswordAsync(user.Email, newPassword);
        return result;
    }
}
