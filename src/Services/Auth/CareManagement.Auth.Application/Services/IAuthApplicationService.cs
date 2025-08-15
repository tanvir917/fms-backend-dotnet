using CareManagement.Auth.Application.DTOs;

namespace CareManagement.Auth.Application.Services;

public interface IAuthApplicationService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<bool> LogoutAsync(string token);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
    Task<bool> ResetPasswordAsync(int userId, string newPassword);
}
