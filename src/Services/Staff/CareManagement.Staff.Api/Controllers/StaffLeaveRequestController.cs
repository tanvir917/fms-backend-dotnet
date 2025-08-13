using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff/{staffId}/leave-requests")]
[Authorize]
public class StaffLeaveRequestController : ControllerBase
{
    private readonly IStaffLeaveRequestService _staffLeaveRequestService;
    private readonly ILogger<StaffLeaveRequestController> _logger;

    public StaffLeaveRequestController(IStaffLeaveRequestService staffLeaveRequestService, ILogger<StaffLeaveRequestController> logger)
    {
        _staffLeaveRequestService = staffLeaveRequestService;
        _logger = logger;
    }

    // GET: api/staff/{staffId}/leave-requests
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StaffLeaveRequestDto>>>> GetStaffLeaveRequests(int staffId)
    {
        try
        {
            var leaveRequests = await _staffLeaveRequestService.GetLeaveRequestsByStaffIdAsync(staffId);
            return Ok(ApiResponse<List<StaffLeaveRequestDto>>.SuccessResult(leaveRequests));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse<List<StaffLeaveRequestDto>>.ErrorResult(ex.Message));
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
            var leaveRequestDto = await _staffLeaveRequestService.CreateLeaveRequestAsync(staffId, request);

            return CreatedAtAction(nameof(GetStaffLeaveRequests), new { staffId = staffId },
                ApiResponse<StaffLeaveRequestDto>.SuccessResult(leaveRequestDto, "Leave request created successfully"));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult(ex.Message));
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
            var leaveRequestDto = await _staffLeaveRequestService.UpdateLeaveRequestAsync(id, request);

            if (leaveRequestDto == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

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
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var leaveRequestDto = await _staffLeaveRequestService.ApproveLeaveRequestAsync(id, userId ?? 0, request.Comments);

            if (leaveRequestDto == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

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
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdString, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var leaveRequestDto = await _staffLeaveRequestService.RejectLeaveRequestAsync(id, userId ?? 0, request.Comments);

            if (leaveRequestDto == null)
            {
                return NotFound(ApiResponse<StaffLeaveRequestDto>.ErrorResult("Leave request not found"));
            }

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
            var result = await _staffLeaveRequestService.DeleteLeaveRequestAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Leave request not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(new { }, "Leave request deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting leave request");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }
}
