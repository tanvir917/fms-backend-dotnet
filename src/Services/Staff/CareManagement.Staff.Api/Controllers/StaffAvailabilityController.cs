using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff/{staffId}/availability")]
[Authorize]
public class StaffAvailabilityController : ControllerBase
{
    private readonly StaffDbContext _context;
    private readonly ILogger<StaffAvailabilityController> _logger;

    public StaffAvailabilityController(StaffDbContext context, ILogger<StaffAvailabilityController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/staff/{staffId}/availability
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StaffAvailabilityDto>>>> GetStaffAvailability(int staffId)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(staffId);
            if (staff == null)
            {
                return NotFound(ApiResponse<List<StaffAvailabilityDto>>.ErrorResult("Staff member not found"));
            }

            var availabilities = await _context.StaffAvailabilities
                .Where(a => a.StaffId == staffId)
                .Select(a => new StaffAvailabilityDto
                {
                    Id = a.Id,
                    StaffId = a.StaffId,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsAvailable = a.IsAvailable,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<StaffAvailabilityDto>>.SuccessResult(availabilities));
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
            var staff = await _context.StaffMembers.FindAsync(staffId);
            if (staff == null)
            {
                return NotFound(ApiResponse<StaffAvailabilityDto>.ErrorResult("Staff member not found"));
            }

            var availability = new StaffAvailability
            {
                StaffId = staffId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsAvailable = request.IsAvailable,
                Notes = request.Notes
            };

            _context.StaffAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            var availabilityDto = new StaffAvailabilityDto
            {
                Id = availability.Id,
                StaffId = availability.StaffId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsAvailable = availability.IsAvailable,
                Notes = availability.Notes,
                CreatedAt = availability.CreatedAt,
                UpdatedAt = availability.UpdatedAt
            };

            return CreatedAtAction(nameof(GetStaffAvailability), new { staffId = staffId },
                ApiResponse<StaffAvailabilityDto>.SuccessResult(availabilityDto, "Availability created successfully"));
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
            var availability = await _context.StaffAvailabilities
                .FirstOrDefaultAsync(a => a.Id == id && a.StaffId == staffId);

            if (availability == null)
            {
                return NotFound(ApiResponse<StaffAvailabilityDto>.ErrorResult("Availability not found"));
            }

            availability.DayOfWeek = request.DayOfWeek;
            availability.StartTime = request.StartTime;
            availability.EndTime = request.EndTime;
            availability.IsAvailable = request.IsAvailable;
            availability.Notes = request.Notes;

            await _context.SaveChangesAsync();

            var availabilityDto = new StaffAvailabilityDto
            {
                Id = availability.Id,
                StaffId = availability.StaffId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsAvailable = availability.IsAvailable,
                Notes = availability.Notes,
                CreatedAt = availability.CreatedAt,
                UpdatedAt = availability.UpdatedAt
            };

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
            var availability = await _context.StaffAvailabilities
                .FirstOrDefaultAsync(a => a.Id == id && a.StaffId == staffId);

            if (availability == null)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Availability not found"));
            }

            _context.StaffAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResult(new { }, "Availability deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff availability");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }
}
