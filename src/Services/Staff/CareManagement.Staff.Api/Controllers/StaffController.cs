using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Shared.DTOs;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly StaffDbContext _context;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<StaffController> _logger;

    public StaffController(StaffDbContext context, IMessageBus messageBus, ILogger<StaffController> logger)
    {
        _context = context;
        _messageBus = messageBus;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<StaffDto>>>> GetStaff(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? department = null,
        [FromQuery] bool? isActive = null)
    {
        try
        {
            var query = _context.StaffMembers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.FirstName.Contains(search) ||
                                       s.LastName.Contains(search) ||
                                       s.EmployeeId.Contains(search));
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(s => s.Department == department);
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var staff = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StaffDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    EmployeeId = s.EmployeeId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    Position = s.Position,
                    Department = s.Department,
                    HourlyRate = s.HourlyRate,
                    DateOfBirth = s.DateOfBirth,
                    Address = s.Address,
                    EmergencyContactName = s.EmergencyContactName,
                    EmergencyContactPhone = s.EmergencyContactPhone,
                    HireDate = s.HireDate,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedResponse<StaffDto>
            {
                Results = staff,
                Count = totalCount,
                Next = page * pageSize < totalCount ? $"?page={page + 1}&pageSize={pageSize}" : null,
                Previous = page > 1 ? $"?page={page - 1}&pageSize={pageSize}" : null
            };

            return Ok(ApiResponse<PaginatedResponse<StaffDto>>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff");
            return StatusCode(500, ApiResponse<PaginatedResponse<StaffDto>>.ErrorResult("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> GetStaff(int id)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null)
            {
                return NotFound(ApiResponse<StaffDto>.ErrorResult("Staff member not found"));
            }

            var staffDto = new StaffDto
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber,
                Position = staff.Position,
                Department = staff.Department,
                HourlyRate = staff.HourlyRate,
                DateOfBirth = staff.DateOfBirth,
                Address = staff.Address,
                EmergencyContactName = staff.EmergencyContactName,
                EmergencyContactPhone = staff.EmergencyContactPhone,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            };

            return Ok(ApiResponse<StaffDto>.SuccessResult(staffDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff member {StaffId}", id);
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Care_Coordinator")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> CreateStaff([FromBody] CreateStaffRequest request)
    {
        try
        {
            var existingStaff = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId || s.Email == request.Email);

            if (existingStaff != null)
            {
                return BadRequest(ApiResponse<StaffDto>.ErrorResult("Staff member with this employee ID or email already exists"));
            }

            var staff = new StaffMember
            {
                UserId = request.UserId,
                EmployeeId = request.EmployeeId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Position = request.Position,
                Department = request.Department,
                HourlyRate = request.HourlyRate,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                HireDate = request.HireDate
            };

            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();

            var staffDto = new StaffDto
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber,
                Position = staff.Position,
                Department = staff.Department,
                HourlyRate = staff.HourlyRate,
                DateOfBirth = staff.DateOfBirth,
                Address = staff.Address,
                EmergencyContactName = staff.EmergencyContactName,
                EmergencyContactPhone = staff.EmergencyContactPhone,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            };

            // Publish staff created event
            await _messageBus.PublishAsync("staff_created", new StaffCreatedEvent
            {
                StaffId = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Position = staff.Position,
                CreatedAt = staff.CreatedAt
            });

            return CreatedAtAction(nameof(GetStaff), new { id = staff.Id },
                ApiResponse<StaffDto>.SuccessResult(staffDto, "Staff member created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Care_Coordinator")]
    public async Task<ActionResult<ApiResponse<StaffDto>>> UpdateStaff(int id, [FromBody] UpdateStaffRequest request)
    {
        try
        {
            var staff = await _context.StaffMembers.FindAsync(id);
            if (staff == null)
            {
                return NotFound(ApiResponse<StaffDto>.ErrorResult("Staff member not found"));
            }

            staff.FirstName = request.FirstName;
            staff.LastName = request.LastName;
            staff.Email = request.Email;
            staff.PhoneNumber = request.PhoneNumber;
            staff.Position = request.Position;
            staff.Department = request.Department;
            staff.HourlyRate = request.HourlyRate;
            staff.DateOfBirth = request.DateOfBirth;
            staff.Address = request.Address;
            staff.EmergencyContactName = request.EmergencyContactName;
            staff.EmergencyContactPhone = request.EmergencyContactPhone;
            staff.HireDate = request.HireDate;
            staff.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            var staffDto = new StaffDto
            {
                Id = staff.Id,
                UserId = staff.UserId,
                EmployeeId = staff.EmployeeId,
                FirstName = staff.FirstName,
                LastName = staff.LastName,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber,
                Position = staff.Position,
                Department = staff.Department,
                HourlyRate = staff.HourlyRate,
                DateOfBirth = staff.DateOfBirth,
                Address = staff.Address,
                EmergencyContactName = staff.EmergencyContactName,
                EmergencyContactPhone = staff.EmergencyContactPhone,
                HireDate = staff.HireDate,
                IsActive = staff.IsActive,
                CreatedAt = staff.CreatedAt,
                UpdatedAt = staff.UpdatedAt
            };

            return Ok(ApiResponse<StaffDto>.SuccessResult(staffDto, "Staff member updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff member {StaffId}", id);
            return StatusCode(500, ApiResponse<StaffDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpGet("stats")]
    [Authorize(Roles = "Admin,Care_Coordinator")]
    public async Task<ActionResult<ApiResponse<StaffStatsDto>>> GetStaffStats()
    {
        try
        {
            var totalStaff = await _context.StaffMembers.CountAsync();
            var activeStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);

            var today = DateTime.UtcNow.Date;
            var onLeave = await _context.StaffLeaveRequests
                .CountAsync(lr => lr.Status == "Approved" &&
                                lr.StartDate <= today &&
                                lr.EndDate >= today);

            var staffByDepartment = await _context.StaffMembers
                .Where(s => s.IsActive)
                .GroupBy(s => s.Department)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var stats = new StaffStatsDto
            {
                TotalStaff = totalStaff,
                ActiveStaff = activeStaff,
                OnLeave = onLeave,
                StaffByDepartment = staffByDepartment
            };

            return Ok(ApiResponse<StaffStatsDto>.SuccessResult(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff stats");
            return StatusCode(500, ApiResponse<StaffStatsDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpGet("leave-requests")]
    public async Task<ActionResult<ApiResponse<List<StaffLeaveRequestDto>>>> GetLeaveRequests(
        [FromQuery] int? staffId = null,
        [FromQuery] string? status = null)
    {
        try
        {
            var query = _context.StaffLeaveRequests
                .Include(lr => lr.Staff)
                .AsQueryable();

            if (staffId.HasValue)
            {
                query = query.Where(lr => lr.StaffId == staffId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(lr => lr.Status == status);
            }

            var leaveRequests = await query
                .Select(lr => new StaffLeaveRequestDto
                {
                    Id = lr.Id,
                    StaffId = lr.StaffId,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    LeaveType = lr.LeaveType,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    ApproverComments = lr.ApproverComments,
                    ApprovedBy = lr.ApprovedBy,
                    ApprovedAt = lr.ApprovedAt,
                    CreatedAt = lr.CreatedAt,
                    Staff = new StaffDto
                    {
                        Id = lr.Staff.Id,
                        FirstName = lr.Staff.FirstName,
                        LastName = lr.Staff.LastName,
                        EmployeeId = lr.Staff.EmployeeId
                    }
                })
                .ToListAsync();

            return Ok(ApiResponse<List<StaffLeaveRequestDto>>.SuccessResult(leaveRequests));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting leave requests");
            return StatusCode(500, ApiResponse<List<StaffLeaveRequestDto>>.ErrorResult("An error occurred"));
        }
    }
}
