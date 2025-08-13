using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;
using CareManagement.Shared.DTOs;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, IMessageBus messageBus, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _messageBus = messageBus;
        _logger = logger;
    }

    // Generate unique employee ID
    private async Task<string> GenerateEmployeeIdAsync()
    {
        return await _staffService.GenerateEmployeeIdAsync();
    }

    // GET: api/staff
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<StaffDto>>>> GetStaff(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? department = null,
        [FromQuery] string? position = null,
        [FromQuery] string? employmentType = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var response = await _staffService.GetStaffAsync(page, pageSize, search, department, position, employmentType, isActive);
            return Ok(ApiResponse<PaginatedResponse<StaffDto>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff");
            return StatusCode(500, ApiResponse<PaginatedResponse<StaffDto>>.ErrorResult("An error occurred"));
        }
    }

    // GET: api/staff/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> GetStaffMember(int id)
    {
        try
        {
            var staffDto = await _staffService.GetStaffMemberAsync(id);

            if (staffDto == null)
            {
                return NotFound(ApiResponse<StaffDto>.ErrorResult("Staff member not found"));
            }

            return Ok(ApiResponse<StaffDto>.SuccessResult(staffDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff member {Id}", id);
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    // POST: api/staff
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> CreateStaff([FromBody] CreateStaffRequest request)
    {
        try
        {
            _logger.LogInformation("CreateStaff called with request: {@Request}", request);
            _logger.LogInformation("Request details - UserId: {UserId}, FirstName: {FirstName}, LastName: {LastName}, Email: {Email}, Position: {Position}, Department: {Department}",
                request.UserId, request.FirstName, request.LastName, request.Email, request.Position, request.Department);

            // Validate model state
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var modelStateEntry in ModelState)
                {
                    var key = modelStateEntry.Key;
                    var modelErrors = modelStateEntry.Value.Errors;
                    foreach (var error in modelErrors)
                    {
                        var errorMessage = $"{key}: {error.ErrorMessage}";
                        errors.Add(errorMessage);
                        _logger.LogError("Validation error for {Key}: {ErrorMessage}", key, error.ErrorMessage);
                    }
                }
                _logger.LogError("Model validation failed with {ErrorCount} errors: {Errors}", errors.Count, string.Join(", ", errors));
                return BadRequest(ApiResponse<StaffDto>.ErrorResult("Validation failed", errors));
            }

            var staffDto = await _staffService.CreateStaffAsync(request);

            return CreatedAtAction(nameof(GetStaffMember), new { id = staffDto.Id },
                ApiResponse<StaffDto>.SuccessResult(staffDto, "Staff member created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation creating staff member");
            return BadRequest(ApiResponse<StaffDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    // PUT: api/staff/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
    {
        try
        {
            var staffDto = await _staffService.UpdateStaffAsync(id, request);
            if (staffDto == null)
            {
                return NotFound(ApiResponse<StaffDto>.ErrorResult("Staff member not found"));
            }

            return Ok(ApiResponse<StaffDto>.SuccessResult(staffDto, "Staff member updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff member");
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    // DELETE: api/staff/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStaff(int id)
    {
        try
        {
            var result = await _staffService.DeleteStaffAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResult("Staff member not found"));
            }

            return Ok(ApiResponse<object>.SuccessResult(new { }, "Staff member deactivated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }

    // GET: api/staff/stats
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<StaffStatsDto>>> GetStaffStats()
    {
        try
        {
            var stats = await _staffService.GetStaffStatsAsync();
            return Ok(ApiResponse<StaffStatsDto>.SuccessResult(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff statistics");
            return StatusCode(500, ApiResponse<StaffStatsDto>.ErrorResult("An error occurred"));
        }
    }

    // GET: api/staff/search
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<StaffDto>>>> SearchStaff(
        [FromQuery] string? query = null,
        [FromQuery] string? department = null,
        [FromQuery] string? position = null,
        [FromQuery] string? employmentType = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var staff = await _staffService.SearchStaffAsync(query, department, position, employmentType, isActive);
            return Ok(ApiResponse<List<StaffDto>>.SuccessResult(staff));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff");
            return StatusCode(500, ApiResponse<List<StaffDto>>.ErrorResult("An error occurred"));
        }
    }
}
