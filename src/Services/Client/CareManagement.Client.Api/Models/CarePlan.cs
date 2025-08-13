using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareManagement.Client.Api.Models;

public enum CarePlanStatus
{
    Draft,
    Active,
    Inactive,
    Completed,
    Discontinued
}

public class CarePlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? Goals { get; set; }

    [MaxLength(1000)]
    public string? InterventionStrategies { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ReviewDate { get; set; }

    public CarePlanStatus Status { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    // Navigation properties
    [ForeignKey("ClientId")]
    [JsonIgnore]
    public virtual Client Client { get; set; } = null!;
}
