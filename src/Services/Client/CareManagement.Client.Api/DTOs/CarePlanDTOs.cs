using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.DTOs;

public class CarePlanDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Goals { get; set; }
    public string? InterventionStrategies { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public CarePlanStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public ClientDto? Client { get; set; }
}

public class CreateCarePlanDto
{
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Goals { get; set; }
    public string? InterventionStrategies { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public CarePlanStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCarePlanDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Goals { get; set; }
    public string? InterventionStrategies { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public CarePlanStatus? Status { get; set; }
    public string? Notes { get; set; }
}

public class ActivateCarePlanDto
{
    public int CarePlanId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? ReviewDate { get; set; }
}
