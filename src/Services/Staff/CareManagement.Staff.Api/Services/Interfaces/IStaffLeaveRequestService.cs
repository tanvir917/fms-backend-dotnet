using CareManagement.Staff.Api.DTOs;

namespace CareManagement.Staff.Api.Services.Interfaces;

public interface IStaffLeaveRequestService
{
    Task<List<StaffLeaveRequestDto>> GetLeaveRequestsByStaffIdAsync(int staffId);
    Task<StaffLeaveRequestDto?> GetLeaveRequestByIdAsync(int id);
    Task<StaffLeaveRequestDto> CreateLeaveRequestAsync(int staffId, CreateStaffLeaveRequestRequest request);
    Task<StaffLeaveRequestDto?> UpdateLeaveRequestAsync(int id, UpdateStaffLeaveRequestRequest request);
    Task<bool> DeleteLeaveRequestAsync(int id);
    Task<StaffLeaveRequestDto?> ApproveLeaveRequestAsync(int id, int approvedBy, string? comments = null);
    Task<StaffLeaveRequestDto?> RejectLeaveRequestAsync(int id, int rejectedBy, string? comments = null);
}
