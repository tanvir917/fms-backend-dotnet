using CareManagement.Auth.Application.DTOs;
using CareManagement.Auth.Core.Entities;

namespace CareManagement.Auth.Application.Mappings;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user, IList<string>? roles = null)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive,
            Roles = roles?.ToList() ?? new List<string>()
        };
    }

    public static User ToEntity(this CreateUserRequestDto dto)
    {
        return new User
        {
            Email = dto.Email,
            UserName = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static void UpdateFromDto(this User user, UpdateUserRequestDto dto)
    {
        user.UpdateProfile(dto.FirstName, dto.LastName, dto.Email);
        user.IsActive = dto.IsActive;
    }
}
