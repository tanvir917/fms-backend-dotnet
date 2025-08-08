using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? PreferredName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? MedicaidNumber { get; set; }
    public string? MedicareNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public CareLevel CareLevel { get; set; }
    public ClientStatus Status { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Medications { get; set; }
    public string? Allergies { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public List<CarePlanDto> CarePlans { get; set; } = new();
    public List<ClientDocumentDto> Documents { get; set; } = new();
    public List<ClientNoteDto> ClientNotes { get; set; } = new();
}

public class CreateClientDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? PreferredName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? MedicaidNumber { get; set; }
    public string? MedicareNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public CareLevel CareLevel { get; set; }
    public ClientStatus Status { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Medications { get; set; }
    public string? Allergies { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
}

public class UpdateClientDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? PreferredName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public string? MedicaidNumber { get; set; }
    public string? MedicareNumber { get; set; }
    public string? SocialSecurityNumber { get; set; }
    public CareLevel? CareLevel { get; set; }
    public ClientStatus? Status { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? MedicalConditions { get; set; }
    public string? Medications { get; set; }
    public string? Allergies { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? Notes { get; set; }
}

public class ClientSearchDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public CareLevel? CareLevel { get; set; }
    public ClientStatus? Status { get; set; }
    public DateTime? AdmissionDateFrom { get; set; }
    public DateTime? AdmissionDateTo { get; set; }
    public int? AgeFrom { get; set; }
    public int? AgeTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}

public class ClientStatsDto
{
    public int TotalClients { get; set; }
    public int ActiveClients { get; set; }
    public int InactiveClients { get; set; }
    public int DischargedClients { get; set; }
    public Dictionary<CareLevel, int> ClientsByCareLevel { get; set; } = new();
    public Dictionary<ClientStatus, int> ClientsByStatus { get; set; } = new();
    public int NewClientsThisMonth { get; set; }
    public int DischargedThisMonth { get; set; }
}
