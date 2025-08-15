using CareManagement.Auth.Application.DTOs;
using CareManagement.Auth.Application.Mappings;
using CareManagement.Auth.Application.Services;
using CareManagement.Auth.Core.Repositories;

namespace CareManagement.Auth.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserRepository _userRepository;

    public UserApplicationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        var roles = await _userRepository.GetRolesAsync(user);
        return user.ToDto(roles);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var roles = await _userRepository.GetRolesAsync(user);
        return user.ToDto(roles);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userRepository.GetRolesAsync(user);
            userDtos.Add(user.ToDto(roles));
        }

        return userDtos;
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetActiveUsersAsync();
        return users.Select(u => u.ToDto());
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequestDto request)
    {
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        var user = request.ToEntity();
        var createdUser = await _userRepository.CreateAsync(user, request.Password);

        // TODO: Add roles to user based on request.Roles

        return createdUser.ToDto();
    }

    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        user.UpdateFromDto(request);
        var updatedUser = await _userRepository.UpdateAsync(user);

        // TODO: Update user roles based on request.Roles

        return updatedUser.ToDto();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Deactivate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> ActivateUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Activate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return false;
        }

        user.Deactivate();
        await _userRepository.UpdateAsync(user);
        return true;
    }
}
