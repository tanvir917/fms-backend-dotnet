using CareManagement.Auth.Application.DTOs;

namespace CareManagement.Auth.Application.Services;

public interface IUserApplicationService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetActiveUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserRequestDto request);
    Task<UserDto> UpdateUserAsync(int id, UpdateUserRequestDto request);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ActivateUserAsync(int id);
    Task<bool> DeactivateUserAsync(int id);
}
