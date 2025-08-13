using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;

namespace CareManagement.Staff.Api.Services.Implementations;

public class StaffLeaveRequestService : IStaffLeaveRequestService
{
    private readonly StaffDbContext _context;
    private readonly ILogger<StaffLeaveRequestService> _logger;

    public StaffLeaveRequestService(StaffDbContext context, ILogger<StaffLeaveRequestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<StaffLeaveRequestDto>> GetLeaveRequestsByStaffIdAsync(int staffId)
    {
        var staff = await _context.StaffMembers.FindAsync(staffId);
        if (staff == null)
        {
            throw new ArgumentException("Staff member not found");
        }

        var leaveRequests = await _context.StaffLeaveRequests
            .Where(lr => lr.StaffId == staffId)
            .Select(lr => new StaffLeaveRequestDto
            {
                Id = lr.Id,
                StaffId = lr.StaffId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                LeaveType = lr.LeaveType,
                Reason = lr.Reason,
                Status = lr.Status,
                Comments = lr.Comments,
                ApprovedBy = lr.ApprovedBy,
                ApprovedAt = lr.ApprovedAt,
                CreatedAt = lr.CreatedAt,
                UpdatedAt = lr.UpdatedAt
            })
            .ToListAsync();

        return leaveRequests;
    }

    public async Task<StaffLeaveRequestDto?> GetLeaveRequestByIdAsync(int id)
    {
        var leaveRequest = await _context.StaffLeaveRequests.FindAsync(id);
        if (leaveRequest == null)
            return null;

        return new StaffLeaveRequestDto
        {
            Id = leaveRequest.Id,
            StaffId = leaveRequest.StaffId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            LeaveType = leaveRequest.LeaveType,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            Comments = leaveRequest.Comments,
            ApprovedBy = leaveRequest.ApprovedBy,
            ApprovedAt = leaveRequest.ApprovedAt,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }

    public async Task<StaffLeaveRequestDto> CreateLeaveRequestAsync(int staffId, CreateStaffLeaveRequestRequest request)
    {
        var staff = await _context.StaffMembers.FindAsync(staffId);
        if (staff == null)
        {
            throw new ArgumentException("Staff member not found");
        }

        var leaveRequest = new StaffLeaveRequest
        {
            StaffId = staffId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            LeaveType = request.LeaveType,
            Reason = request.Reason,
            Status = "pending"
        };

        _context.StaffLeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();

        return new StaffLeaveRequestDto
        {
            Id = leaveRequest.Id,
            StaffId = leaveRequest.StaffId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            LeaveType = leaveRequest.LeaveType,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            Comments = leaveRequest.Comments,
            ApprovedBy = leaveRequest.ApprovedBy,
            ApprovedAt = leaveRequest.ApprovedAt,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }

    public async Task<StaffLeaveRequestDto?> UpdateLeaveRequestAsync(int id, UpdateStaffLeaveRequestRequest request)
    {
        var leaveRequest = await _context.StaffLeaveRequests.FindAsync(id);
        if (leaveRequest == null)
            return null;

        leaveRequest.StartDate = request.StartDate;
        leaveRequest.EndDate = request.EndDate;
        leaveRequest.LeaveType = request.LeaveType;
        leaveRequest.Reason = request.Reason;

        await _context.SaveChangesAsync();

        return new StaffLeaveRequestDto
        {
            Id = leaveRequest.Id,
            StaffId = leaveRequest.StaffId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            LeaveType = leaveRequest.LeaveType,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            Comments = leaveRequest.Comments,
            ApprovedBy = leaveRequest.ApprovedBy,
            ApprovedAt = leaveRequest.ApprovedAt,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }

    public async Task<bool> DeleteLeaveRequestAsync(int id)
    {
        var leaveRequest = await _context.StaffLeaveRequests.FindAsync(id);
        if (leaveRequest == null)
            return false;

        _context.StaffLeaveRequests.Remove(leaveRequest);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<StaffLeaveRequestDto?> ApproveLeaveRequestAsync(int id, int approvedBy, string? comments = null)
    {
        var leaveRequest = await _context.StaffLeaveRequests.FindAsync(id);
        if (leaveRequest == null)
            return null;

        leaveRequest.Status = "approved";
        leaveRequest.Comments = comments;
        leaveRequest.ApprovedBy = approvedBy;
        leaveRequest.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new StaffLeaveRequestDto
        {
            Id = leaveRequest.Id,
            StaffId = leaveRequest.StaffId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            LeaveType = leaveRequest.LeaveType,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            Comments = leaveRequest.Comments,
            ApprovedBy = leaveRequest.ApprovedBy,
            ApprovedAt = leaveRequest.ApprovedAt,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }

    public async Task<StaffLeaveRequestDto?> RejectLeaveRequestAsync(int id, int rejectedBy, string? comments = null)
    {
        var leaveRequest = await _context.StaffLeaveRequests.FindAsync(id);
        if (leaveRequest == null)
            return null;

        leaveRequest.Status = "rejected";
        leaveRequest.Comments = comments;
        leaveRequest.ApprovedBy = rejectedBy;
        leaveRequest.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new StaffLeaveRequestDto
        {
            Id = leaveRequest.Id,
            StaffId = leaveRequest.StaffId,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            LeaveType = leaveRequest.LeaveType,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            Comments = leaveRequest.Comments,
            ApprovedBy = leaveRequest.ApprovedBy,
            ApprovedAt = leaveRequest.ApprovedAt,
            CreatedAt = leaveRequest.CreatedAt,
            UpdatedAt = leaveRequest.UpdatedAt
        };
    }
}
