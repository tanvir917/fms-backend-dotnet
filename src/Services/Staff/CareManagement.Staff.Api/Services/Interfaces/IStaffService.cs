using CareManagement.Staff.Api.DTOs;
using CareManagement.Shared.DTOs;

namespace CareManagement.Staff.Api.Services.Interfaces;

public interface IStaffService
{
    Task<PaginatedResponse<StaffDto>> GetStaffAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? department = null,
        string? position = null,
        string? employmentType = null,
        bool? isActive = null);

    Task<StaffDto?> GetStaffMemberAsync(int id);
    Task<StaffDto> CreateStaffAsync(CreateStaffRequest request);
    Task<StaffDto?> UpdateStaffAsync(int id, UpdateStaffRequest request);
    Task<bool> DeleteStaffAsync(int id);
    Task<StaffStatsDto> GetStaffStatsAsync();
    Task<List<StaffDto>> SearchStaffAsync(
        string? query = null,
        string? department = null,
        string? position = null,
        string? employmentType = null,
        bool? isActive = null);
    Task<string> GenerateEmployeeIdAsync();
}
