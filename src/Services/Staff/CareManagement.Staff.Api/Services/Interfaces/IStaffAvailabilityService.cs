using CareManagement.Staff.Api.DTOs;

namespace CareManagement.Staff.Api.Services.Interfaces;

public interface IStaffAvailabilityService
{
    Task<List<StaffAvailabilityDto>> GetStaffAvailabilityAsync(int staffId);
    Task<StaffAvailabilityDto> CreateStaffAvailabilityAsync(int staffId, CreateStaffAvailabilityRequest request);
    Task<StaffAvailabilityDto?> UpdateStaffAvailabilityAsync(int staffId, int id, UpdateStaffAvailabilityRequest request);
    Task<bool> DeleteStaffAvailabilityAsync(int staffId, int id);
}
