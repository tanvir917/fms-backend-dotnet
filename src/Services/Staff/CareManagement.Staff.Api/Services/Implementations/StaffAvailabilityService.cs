using Microsoft.EntityFrameworkCore;
using CareManagement.Staff.Api.Data;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Staff.Api.Services.Interfaces;

namespace CareManagement.Staff.Api.Services.Implementations;

public class StaffAvailabilityService : IStaffAvailabilityService
{
    private readonly StaffDbContext _context;
    private readonly ILogger<StaffAvailabilityService> _logger;

    public StaffAvailabilityService(StaffDbContext context, ILogger<StaffAvailabilityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<StaffAvailabilityDto>> GetStaffAvailabilityAsync(int staffId)
    {
        var staff = await _context.StaffMembers.FindAsync(staffId);
        if (staff == null)
        {
            throw new ArgumentException("Staff member not found");
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

        return availabilities;
    }

    public async Task<StaffAvailabilityDto> CreateStaffAvailabilityAsync(int staffId, CreateStaffAvailabilityRequest request)
    {
        var staff = await _context.StaffMembers.FindAsync(staffId);
        if (staff == null)
        {
            throw new ArgumentException("Staff member not found");
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

        return new StaffAvailabilityDto
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
    }

    public async Task<StaffAvailabilityDto?> UpdateStaffAvailabilityAsync(int staffId, int id, UpdateStaffAvailabilityRequest request)
    {
        var availability = await _context.StaffAvailabilities
            .FirstOrDefaultAsync(a => a.Id == id && a.StaffId == staffId);

        if (availability == null)
            return null;

        availability.DayOfWeek = request.DayOfWeek;
        availability.StartTime = request.StartTime;
        availability.EndTime = request.EndTime;
        availability.IsAvailable = request.IsAvailable;
        availability.Notes = request.Notes;

        await _context.SaveChangesAsync();

        return new StaffAvailabilityDto
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
    }

    public async Task<bool> DeleteStaffAvailabilityAsync(int staffId, int id)
    {
        var availability = await _context.StaffAvailabilities
            .FirstOrDefaultAsync(a => a.Id == id && a.StaffId == staffId);

        if (availability == null)
            return false;

        _context.StaffAvailabilities.Remove(availability);
        await _context.SaveChangesAsync();

        return true;
    }
}
