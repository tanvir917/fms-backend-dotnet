using System.ComponentModel.DataAnnotations;

namespace CareManagement.Staff.Api.DTOs;

public class StaffDto
{
    public int Id { get; set; }
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
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public List<StaffAvailabilityDto>? Availabilities { get; set; }
    public List<StaffLeaveRequestDto>? LeaveRequests { get; set; }
}

public class CreateStaffRequest
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required")]
    [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Employment type is required")]
    [StringLength(50, ErrorMessage = "Employment type cannot exceed 50 characters")]
    public string EmploymentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Hourly rate is required")]
    [Range(0.01, 999.99, ErrorMessage = "Hourly rate must be between 0.01 and 999.99")]
    public decimal HourlyRate { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Address line 1 is required")]
    [StringLength(200, ErrorMessage = "Address line 1 cannot exceed 200 characters")]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Address line 2 cannot exceed 200 characters")]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string PostalCode { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Qualifications cannot exceed 1000 characters")]
    public string? Qualifications { get; set; }

    [StringLength(1000, ErrorMessage = "Certifications cannot exceed 1000 characters")]
    public string? Certifications { get; set; }

    [Range(1, 168, ErrorMessage = "Preferred hours per week must be between 1 and 168")]
    public int? PreferredHoursPerWeek { get; set; }

    public bool AvailableWeekdays { get; set; } = true;
    public bool AvailableWeekends { get; set; } = false;
    public bool AvailableNights { get; set; } = false;

    [Required(ErrorMessage = "Emergency contact name is required")]
    [StringLength(100, ErrorMessage = "Emergency contact name cannot exceed 100 characters")]
    public string EmergencyContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency contact phone is required")]
    [Phone(ErrorMessage = "Invalid emergency contact phone format")]
    public string EmergencyContactPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Emergency contact relationship is required")]
    [StringLength(50, ErrorMessage = "Emergency contact relationship cannot exceed 50 characters")]
    public string EmergencyContactRelationship { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
}

public class UpdateStaffRequest
{
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
    public bool AvailableWeekdays { get; set; }
    public bool AvailableWeekends { get; set; }
    public bool AvailableNights { get; set; }
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string EmergencyContactRelationship { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class StaffLeaveRequestDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public StaffDto? Staff { get; set; }

    public int DurationDays => (EndDate - StartDate).Days + 1;
}

public class CreateLeaveRequestRequest
{
    public int StaffId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class UpdateLeaveRequestRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

public class ApproveRejectLeaveRequest
{
    public string Status { get; set; } = string.Empty; // "approved" or "rejected"
    public string? Comments { get; set; }
}

public class StaffAvailabilityDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAvailabilityRequest
{
    public int StaffId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateAvailabilityRequest
{
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
}

public class StaffStatsDto
{
    public int TotalStaff { get; set; }
    public int ActiveStaff { get; set; }
    public int OnLeave { get; set; }
    public int TotalDepartments { get; set; }
    public Dictionary<string, int> DepartmentBreakdown { get; set; } = new();
    public Dictionary<string, int> EmploymentTypeBreakdown { get; set; } = new();
    public Dictionary<string, int> PositionBreakdown { get; set; } = new();
}

public class StaffSearchRequest
{
    public string? Query { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? EmploymentType { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

// Staff Availability DTOs
public class CreateStaffAvailabilityRequest
{
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateStaffAvailabilityRequest
{
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string? Notes { get; set; }
}

// Staff Leave Request DTOs
public class CreateStaffLeaveRequestRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class UpdateStaffLeaveRequestRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class ApproveLeaveRequestRequest
{
    public string? Comments { get; set; }
}

public class RejectLeaveRequestRequest
{
    public string? Comments { get; set; }
}
