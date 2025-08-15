using CareManagement.Auth.Core.Entities;

namespace CareManagement.Auth.Core.Services;

public interface IAuthService
{
    Task<string> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(string email, string password, string firstName, string lastName);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string token);
    Task<bool> ResetPasswordAsync(string email, string newPassword);
}
