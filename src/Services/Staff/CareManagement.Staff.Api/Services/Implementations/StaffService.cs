using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;
using CareManagement.Shared.DTOs;

namespace CareManagement.Staff.Api.Services.Implementations;

public class StaffService : IStaffService
{
    private readonly StaffDbContext _context;
    private readonly ILogger<StaffService> _logger;

    public StaffService(StaffDbContext context, ILogger<StaffService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<StaffDto>> GetStaffAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        string? department = null,
        string? position = null,
        string? employmentType = null,
        bool? isActive = null)
    {
        var query = _context.StaffMembers.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.FirstName.Contains(search) ||
                                   s.LastName.Contains(search) ||
                                   s.Email.Contains(search) ||
                                   s.EmployeeId.Contains(search));
        }

        if (!string.IsNullOrEmpty(department))
        {
            query = query.Where(s => s.Department == department);
        }

        if (!string.IsNullOrEmpty(position))
        {
            query = query.Where(s => s.Position == position);
        }

        if (!string.IsNullOrEmpty(employmentType))
        {
            query = query.Where(s => s.EmploymentType == employmentType);
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
                EmploymentType = s.EmploymentType,
                HourlyRate = s.HourlyRate,
                DateOfBirth = s.DateOfBirth,
                AddressLine1 = s.AddressLine1,
                AddressLine2 = s.AddressLine2,
                City = s.City,
                State = s.State,
                PostalCode = s.PostalCode,
                Qualifications = s.Qualifications,
                Certifications = s.Certifications,
                PreferredHoursPerWeek = s.PreferredHoursPerWeek,
                AvailableWeekdays = s.AvailableWeekdays,
                AvailableWeekends = s.AvailableWeekends,
                AvailableNights = s.AvailableNights,
                EmergencyContactName = s.EmergencyContactName,
                EmergencyContactPhone = s.EmergencyContactPhone,
                EmergencyContactRelationship = s.EmergencyContactRelationship,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();

        return new PaginatedResponse<StaffDto>
        {
            Results = staff,
            Count = totalCount,
            Next = page * pageSize < totalCount ? $"?page={page + 1}&pageSize={pageSize}" : null,
            Previous = page > 1 ? $"?page={page - 1}&pageSize={pageSize}" : null
        };
    }

    public async Task<StaffDto?> GetStaffMemberAsync(int id)
    {
        var staff = await _context.StaffMembers
            .Include(s => s.Availabilities)
            .Include(s => s.LeaveRequests)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff == null)
            return null;

        return new StaffDto
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
            EmploymentType = staff.EmploymentType,
            HourlyRate = staff.HourlyRate,
            DateOfBirth = staff.DateOfBirth,
            AddressLine1 = staff.AddressLine1,
            AddressLine2 = staff.AddressLine2,
            City = staff.City,
            State = staff.State,
            PostalCode = staff.PostalCode,
            Qualifications = staff.Qualifications,
            Certifications = staff.Certifications,
            PreferredHoursPerWeek = staff.PreferredHoursPerWeek,
            AvailableWeekdays = staff.AvailableWeekdays,
            AvailableWeekends = staff.AvailableWeekends,
            AvailableNights = staff.AvailableNights,
            EmergencyContactName = staff.EmergencyContactName,
            EmergencyContactPhone = staff.EmergencyContactPhone,
            EmergencyContactRelationship = staff.EmergencyContactRelationship,
            StartDate = staff.StartDate,
            EndDate = staff.EndDate,
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            UpdatedAt = staff.UpdatedAt,
            Availabilities = staff.Availabilities.Select(a => new StaffAvailabilityDto
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
            }).ToList(),
            LeaveRequests = staff.LeaveRequests.Select(lr => new StaffLeaveRequestDto
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
            }).ToList()
        };
    }

    public async Task<StaffDto> CreateStaffAsync(CreateStaffRequest request)
    {
        _logger.LogInformation("CreateStaff called with request: {@Request}", request);

        // Check if email already exists
        var existingStaff = await _context.StaffMembers
            .FirstOrDefaultAsync(s => s.Email == request.Email);

        if (existingStaff != null)
        {
            throw new InvalidOperationException("Staff member with this email already exists");
        }

        // Generate unique employee ID
        var employeeId = await GenerateEmployeeIdAsync();

        var staff = new StaffMember
        {
            UserId = request.UserId,
            EmployeeId = employeeId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Position = request.Position,
            Department = request.Department,
            EmploymentType = request.EmploymentType,
            HourlyRate = request.HourlyRate,
            DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc),
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Qualifications = request.Qualifications,
            Certifications = request.Certifications,
            PreferredHoursPerWeek = request.PreferredHoursPerWeek,
            AvailableWeekdays = request.AvailableWeekdays,
            AvailableWeekends = request.AvailableWeekends,
            AvailableNights = request.AvailableNights,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc)
        };

        _context.StaffMembers.Add(staff);
        await _context.SaveChangesAsync();

        return new StaffDto
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
            EmploymentType = staff.EmploymentType,
            HourlyRate = staff.HourlyRate,
            DateOfBirth = staff.DateOfBirth,
            AddressLine1 = staff.AddressLine1,
            AddressLine2 = staff.AddressLine2,
            City = staff.City,
            State = staff.State,
            PostalCode = staff.PostalCode,
            Qualifications = staff.Qualifications,
            Certifications = staff.Certifications,
            PreferredHoursPerWeek = staff.PreferredHoursPerWeek,
            AvailableWeekdays = staff.AvailableWeekdays,
            AvailableWeekends = staff.AvailableWeekends,
            AvailableNights = staff.AvailableNights,
            EmergencyContactName = staff.EmergencyContactName,
            EmergencyContactPhone = staff.EmergencyContactPhone,
            EmergencyContactRelationship = staff.EmergencyContactRelationship,
            StartDate = staff.StartDate,
            EndDate = staff.EndDate,
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            UpdatedAt = staff.UpdatedAt
        };
    }

    public async Task<StaffDto?> UpdateStaffAsync(int id, UpdateStaffRequest request)
    {
        var staff = await _context.StaffMembers.FindAsync(id);
        if (staff == null)
            return null;

        staff.FirstName = request.FirstName;
        staff.LastName = request.LastName;
        staff.Email = request.Email;
        staff.PhoneNumber = request.PhoneNumber;
        staff.Position = request.Position;
        staff.Department = request.Department;
        staff.EmploymentType = request.EmploymentType;
        staff.HourlyRate = request.HourlyRate;
        staff.DateOfBirth = DateTime.SpecifyKind(request.DateOfBirth, DateTimeKind.Utc);
        staff.AddressLine1 = request.AddressLine1;
        staff.AddressLine2 = request.AddressLine2;
        staff.City = request.City;
        staff.State = request.State;
        staff.PostalCode = request.PostalCode;
        staff.Qualifications = request.Qualifications;
        staff.Certifications = request.Certifications;
        staff.PreferredHoursPerWeek = request.PreferredHoursPerWeek;
        staff.AvailableWeekdays = request.AvailableWeekdays;
        staff.AvailableWeekends = request.AvailableWeekends;
        staff.AvailableNights = request.AvailableNights;
        staff.EmergencyContactName = request.EmergencyContactName;
        staff.EmergencyContactPhone = request.EmergencyContactPhone;
        staff.EmergencyContactRelationship = request.EmergencyContactRelationship;
        staff.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
        staff.EndDate = request.EndDate.HasValue ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc) : null;
        staff.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return new StaffDto
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
            EmploymentType = staff.EmploymentType,
            HourlyRate = staff.HourlyRate,
            DateOfBirth = staff.DateOfBirth,
            AddressLine1 = staff.AddressLine1,
            AddressLine2 = staff.AddressLine2,
            City = staff.City,
            State = staff.State,
            PostalCode = staff.PostalCode,
            Qualifications = staff.Qualifications,
            Certifications = staff.Certifications,
            PreferredHoursPerWeek = staff.PreferredHoursPerWeek,
            AvailableWeekdays = staff.AvailableWeekdays,
            AvailableWeekends = staff.AvailableWeekends,
            AvailableNights = staff.AvailableNights,
            EmergencyContactName = staff.EmergencyContactName,
            EmergencyContactPhone = staff.EmergencyContactPhone,
            EmergencyContactRelationship = staff.EmergencyContactRelationship,
            StartDate = staff.StartDate,
            EndDate = staff.EndDate,
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            UpdatedAt = staff.UpdatedAt
        };
    }

    public async Task<bool> DeleteStaffAsync(int id)
    {
        var staff = await _context.StaffMembers.FindAsync(id);
        if (staff == null)
            return false;

        // Soft delete - set IsActive to false
        staff.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<StaffStatsDto> GetStaffStatsAsync()
    {
        var totalStaff = await _context.StaffMembers.CountAsync(s => s.IsActive);
        var activeStaff = totalStaff; // Same as total for active staff

        // Staff currently on leave
        var onLeave = await _context.StaffLeaveRequests
            .CountAsync(lr => lr.Status == "approved" &&
                       lr.StartDate <= DateTime.Today &&
                       lr.EndDate >= DateTime.Today);

        // Department breakdown
        var departmentBreakdown = await _context.StaffMembers
            .Where(s => s.IsActive)
            .GroupBy(s => s.Department)
            .Select(g => new { Department = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Department, x => x.Count);

        // Employment type breakdown
        var employmentTypeBreakdown = await _context.StaffMembers
            .Where(s => s.IsActive)
            .GroupBy(s => s.EmploymentType)
            .Select(g => new { EmploymentType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EmploymentType, x => x.Count);

        // Position breakdown
        var positionBreakdown = await _context.StaffMembers
            .Where(s => s.IsActive)
            .GroupBy(s => s.Position)
            .Select(g => new { Position = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Position, x => x.Count);

        return new StaffStatsDto
        {
            TotalStaff = totalStaff,
            ActiveStaff = activeStaff,
            OnLeave = onLeave,
            TotalDepartments = departmentBreakdown.Count,
            DepartmentBreakdown = departmentBreakdown,
            EmploymentTypeBreakdown = employmentTypeBreakdown,
            PositionBreakdown = positionBreakdown
        };
    }

    public async Task<List<StaffDto>> SearchStaffAsync(
        string? query = null,
        string? department = null,
        string? position = null,
        string? employmentType = null,
        bool? isActive = null)
    {
        var staffQuery = _context.StaffMembers.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            staffQuery = staffQuery.Where(s =>
                s.FirstName.Contains(query) ||
                s.LastName.Contains(query) ||
                s.Email.Contains(query) ||
                s.EmployeeId.Contains(query));
        }

        if (!string.IsNullOrEmpty(department))
        {
            staffQuery = staffQuery.Where(s => s.Department == department);
        }

        if (!string.IsNullOrEmpty(position))
        {
            staffQuery = staffQuery.Where(s => s.Position == position);
        }

        if (!string.IsNullOrEmpty(employmentType))
        {
            staffQuery = staffQuery.Where(s => s.EmploymentType == employmentType);
        }

        if (isActive.HasValue)
        {
            staffQuery = staffQuery.Where(s => s.IsActive == isActive.Value);
        }

        return await staffQuery
            .Take(50) // Limit results for search
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
                EmploymentType = s.EmploymentType,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<string> GenerateEmployeeIdAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var prefix = $"EMP{currentYear}";

        // Get the highest existing employee ID for this year
        var lastEmployeeId = await _context.StaffMembers
            .Where(s => s.EmployeeId.StartsWith(prefix))
            .OrderByDescending(s => s.EmployeeId)
            .Select(s => s.EmployeeId)
            .FirstOrDefaultAsync();

        var nextNumber = 1;
        if (!string.IsNullOrEmpty(lastEmployeeId))
        {
            // Extract the number part and increment
            var numberPart = lastEmployeeId.Substring(prefix.Length);
            if (int.TryParse(numberPart, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D4}"; // e.g., EMP20250001
    }
}
