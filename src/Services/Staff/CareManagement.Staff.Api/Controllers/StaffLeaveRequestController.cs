using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff/{staffId}/leave-requests")]
[Authorize]
public class StaffLeaveRequestController : ControllerBase
{
    private readonly StaffDbContext _context;
    private readonly ILogger<StaffLeaveRequestController> _logger;

    public StaffLeaveRequestController(StaffDbContext context, ILogger<StaffLeaveRequestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/staff/{staffId}/leave-requests
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StaffLeaveRequestDto>>>> GetStaffLeaveRequests(int staffId)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(staffId);
            if (staff == null)
            {
                return NotFound(ApiResponse<List<StaffLeaveRequestDto>>.ErrorResult("Staff member not found"));
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

            return Ok(ApiResponse<List<StaffLeaveRequestDto>>.SuccessResult(leaveRequests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leave requests for staff {StaffId}", staffId);
            return StatusCode(500, ApiResponse<List<StaffLeaveRequestDto>>.ErrorResult("An error occurred"));
        }
    }

    // POST: api/staff/{staffId}/leave-requests
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StaffLeaveRequestDto>>> CreateLeaveRequest(
        int staffId,
        [FromBody] CreateStaffLeaveRequestRequest request)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(staffId);
            if (staff == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Staff member not found"));
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

            var leaveRequestDto = new StaffLeaveRequestDto
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

            return CreatedAtAction(nameof(GetStaffLeaveRequests), new { staffId = staffId },
                ApiResponse<StaffLeaveRequestDto>.SuccessResult(leaveRequestDto, "Leave request created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating leave request");
            return StatusCode(500, ApiResponse<StaffLeaveRequestDto>.ErrorResult("An error occurred"));
        }
    }

    // PUT: api/staff/{staffId}/leave-requests/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<StaffLeaveRequestDto>>> UpdateLeaveRequest(
        int staffId,
        int id,
        [FromBody] UpdateStaffLeaveRequestRequest request)
    {
        try
        {
            var leaveRequest = await _context.StaffLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.StaffId == staffId);

            if (leaveRequest == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

            leaveRequest.StartDate = request.StartDate;
            leaveRequest.EndDate = request.EndDate;
            leaveRequest.LeaveType = request.LeaveType;
            leaveRequest.Reason = request.Reason;

            await _context.SaveChangesAsync();

            var leaveRequestDto = new StaffLeaveRequestDto
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

            return Ok(ApiResponse<StaffLeaveRequestDto>.SuccessResult(leaveRequestDto, "Leave request updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating leave request");
            return StatusCode(500, ApiResponse<StaffLeaveRequestDto>.ErrorResult("An error occurred"));
        }
    }

    // PUT: api/staff/{staffId}/leave-requests/{id}/approve
    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StaffLeaveRequestDto>>> ApproveLeaveRequest(
        int staffId,
        int id,
        [FromBody] ApproveLeaveRequestRequest request)
    {
        try
        {
            var leaveRequest = await _context.StaffLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.StaffId == staffId);

            if (leaveRequest == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            leaveRequest.Status = "approved";
            leaveRequest.Comments = request.Comments;
            leaveRequest.ApprovedBy = userId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var leaveRequestDto = new StaffLeaveRequestDto
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

            return Ok(ApiResponse<StaffLeaveRequestDto>.SuccessResult(leaveRequestDto, "Leave request approved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving leave request");
            return StatusCode(500, ApiResponse<StaffLeaveRequestDto>.ErrorResult("An error occurred"));
        }
    }

    // PUT: api/staff/{staffId}/leave-requests/{id}/reject
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StaffLeaveRequestDto>>> RejectLeaveRequest(
        int staffId,
        int id,
        [FromBody] RejectLeaveRequestRequest request)
    {
        try
        {
            var leaveRequest = await _context.StaffLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.StaffId == staffId);

            if (leaveRequest == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            leaveRequest.Status = "rejected";
            leaveRequest.Comments = request.Comments;
            leaveRequest.ApprovedBy = userId;
            leaveRequest.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var leaveRequestDto = new StaffLeaveRequestDto
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

            return Ok(ApiResponse<StaffLeaveRequestDto>.SuccessResult(leaveRequestDto, "Leave request rejected successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting leave request");
            return StatusCode(500, ApiResponse<StaffLeaveRequestDto>.ErrorResult("An error occurred"));
        }
    }

    // DELETE: api/staff/{staffId}/leave-requests/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaveRequest(int staffId, int id)
    {
        try
        {
            var leaveRequest = await _context.StaffLeaveRequests
                .FirstOrDefaultAsync(lr => lr.Id == id && lr.StaffId == staffId);

            if (leaveRequest == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Leave request not found"));
            }

            _context.StaffLeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { }, "Leave request deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting leave request");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }
}
