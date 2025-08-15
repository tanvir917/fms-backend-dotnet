using CareManagement.Auth.Core.Entities;

namespace CareManagement.Auth.Core.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User> CreateAsync(User user, string password);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> ValidatePasswordAsync(User user, string password);
    Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IList<string>> GetRolesAsync(User user);
}
