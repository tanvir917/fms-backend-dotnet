using CareManagement.Auth.Api.Data;

namespace CareManagement.Auth.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
