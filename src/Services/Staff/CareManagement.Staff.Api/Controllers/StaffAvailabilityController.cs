using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff/{staffId}/availability")]
[Authorize]
public class StaffAvailabilityController : ControllerBase
{
    private readonly IStaffAvailabilityService _staffAvailabilityService;
    private readonly ILogger<StaffAvailabilityController> _logger;

    public StaffAvailabilityController(IStaffAvailabilityService staffAvailabilityService, ILogger<StaffAvailabilityController> logger)
    {
        _staffAvailabilityService = staffAvailabilityService;
        _logger = logger;
    }

    // GET: api/staff/{staffId}/availability
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StaffAvailabilityDto>>>> GetStaffAvailability(int staffId)
    {
        try
        {
            var availabilities = await _staffAvailabilityService.GetStaffAvailabilityAsync(staffId);
            return Ok(ApiResponse<List<StaffAvailabilityDto>>.SuccessResult(availabilities));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse<List<StaffAvailabilityDto>>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff availability for staff {StaffId}", staffId);
            return StatusCode(500, ApiResponse<List<StaffAvailabilityDto>>.ErrorResult("An error occurred"));
        }
    }

    // POST: api/staff/{staffId}/availability
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StaffAvailabilityDto>>> CreateStaffAvailability(
        int staffId,
        [FromBody] CreateStaffAvailabilityRequest request)
    {
        try
        {
            var availabilityDto = await _staffAvailabilityService.CreateStaffAvailabilityAsync(staffId, request);

            return CreatedAtAction(nameof(GetStaffAvailability), new { staffId = staffId },
                ApiResponse<StaffAvailabilityDto>.SuccessResult(availabilityDto, "Availability created successfully"));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse<StaffAvailabilityDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff availability");
            return StatusCode(500, ApiResponse<StaffAvailabilityDto>.ErrorResult("An error occurred"));
        }
    }

    // PUT: api/staff/{staffId}/availability/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<StaffAvailabilityDto>>> UpdateStaffAvailability(
        int staffId,
        int id,
        [FromBody] UpdateStaffAvailabilityRequest request)
    {
        try
        {
            var availabilityDto = await _staffAvailabilityService.UpdateStaffAvailabilityAsync(staffId, id, request);

            if (availabilityDto == null)
            {
                return NotFound(ApiResponse<StaffAvailabilityDto>.ErrorResult("Availability not found"));
            }

            return Ok(ApiResponse<StaffAvailabilityDto>.SuccessResult(availabilityDto, "Availability updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff availability");
            return StatusCode(500, ApiResponse<StaffAvailabilityDto>.ErrorResult("An error occurred"));
        }
    }

    // DELETE: api/staff/{staffId}/availability/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStaffAvailability(int staffId, int id)
    {
        try
        {
            var result = await _staffAvailabilityService.DeleteStaffAvailabilityAsync(staffId, id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Availability not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(new { }, "Availability deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff availability");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }
}
