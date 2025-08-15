namespace CareManagement.Auth.Core.Services;

public interface ITokenService
{
    string GenerateAccessToken(int userId, string email, IEnumerable<string> roles);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    int GetUserIdFromToken(string token);
    DateTime GetTokenExpiry(string token);
}
