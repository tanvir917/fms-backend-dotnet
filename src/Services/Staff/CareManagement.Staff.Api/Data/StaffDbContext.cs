using Microsoft.EntityFrameworkCore;
using CareManagement.Shared.Data;

namespace CareManagement.Staff.Api.Data;

public class StaffMember : BaseEntity
{
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? Qualifications { get; set; }
    public string? Certifications { get; set; }
    public int? PreferredHoursPerWeek { get; set; }
    public bool AvailableWeekdays { get; set; } = true;
    public bool AvailableWeekends { get; set; } = false;
    public bool AvailableNights { get; set; } = false;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public List<StaffLeaveRequest> LeaveRequests { get; set; } = new();
    public List<StaffAvailability> Availabilities { get; set; } = new();
}

public class StaffLeaveRequest : BaseEntity
{
    public int StaffId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty; // annual, sick, personal, maternity, emergency, unpaid
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "pending"; // pending, approved, rejected, cancelled
    public string? Comments { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation properties
    public StaffMember Staff { get; set; } = null!;
}

public class StaffAvailability : BaseEntity
{
    public int StaffId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty; // monday, tuesday, wednesday, thursday, friday, saturday, sunday
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation properties
    public StaffMember Staff { get; set; } = null!;
}

public class StaffDbContext : BaseDbContext
{
    public StaffDbContext(DbContextOptions<StaffDbContext> options) : base(options)
    {
    }

    public DbSet<StaffMember> StaffMembers { get; set; }
    public DbSet<StaffLeaveRequest> StaffLeaveRequests { get; set; }
    public DbSet<StaffAvailability> StaffAvailabilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StaffMember>(entity =>
        {
            entity.HasIndex(e => e.EmployeeId).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.HourlyRate).HasPrecision(10, 2);
        });

        modelBuilder.Entity<StaffLeaveRequest>(entity =>
        {
            entity.HasOne(e => e.Staff)
                  .WithMany(s => s.LeaveRequests)
                  .HasForeignKey(e => e.StaffId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StaffAvailability>(entity =>
        {
            entity.HasOne(e => e.Staff)
                  .WithMany(s => s.Availabilities)
                  .HasForeignKey(e => e.StaffId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Ensure unique combination of staff, day, and start time
            entity.HasIndex(e => new { e.StaffId, e.DayOfWeek, e.StartTime }).IsUnique();
        });
    }
}
