using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareManagement.Client.Api.Models;

public enum CareLevel
{
    Low,
    Medium,
    High,
    Respite,
    Palliative
}

public enum ClientStatus
{
    Active,
    Inactive,
    Discharged,
    Deceased,
    OnHold
}

public class Client
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? MiddleName { get; set; }

    [MaxLength(20)]
    public string? PreferredName { get; set; }

    public DateTime DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? ZipCode { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [MaxLength(100)]
    public string? EmergencyContactRelationship { get; set; }

    [MaxLength(50)]
    public string? MedicaidNumber { get; set; }

    [MaxLength(50)]
    public string? MedicareNumber { get; set; }

    [MaxLength(50)]
    public string? SocialSecurityNumber { get; set; }

    public CareLevel CareLevel { get; set; }

    public ClientStatus Status { get; set; }

    public DateTime? AdmissionDate { get; set; }

    public DateTime? DischargeDate { get; set; }

    [MaxLength(1000)]
    public string? MedicalConditions { get; set; }

    [MaxLength(1000)]
    public string? Medications { get; set; }

    [MaxLength(1000)]
    public string? Allergies { get; set; }

    [MaxLength(1000)]
    public string? SpecialInstructions { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public virtual ICollection<CarePlan> CarePlans { get; set; } = new List<CarePlan>();
    public virtual ICollection<ClientDocument> Documents { get; set; } = new List<ClientDocument>();
    public virtual ICollection<ClientNote> ClientNotes { get; set; } = new List<ClientNote>();

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public int Age => DateTime.Today.Year - DateOfBirth.Year - (DateTime.Today.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
}
