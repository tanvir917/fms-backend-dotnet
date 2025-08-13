using CareManagement.Auth.Api.DTOs;

namespace CareManagement.Auth.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetProfileAsync(string userId);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<bool> AdminResetPasswordAsync(int userId, AdminChangePasswordRequest request, string adminUserId);
}
