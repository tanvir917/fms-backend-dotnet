using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareManagement.Client.Api.Models;

public enum DocumentType
{
    Assessment,
    CarePlan,
    MedicalRecord,
    Insurance,
    Legal,
    Photo,
    Other
}

public class ClientDocument
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DocumentType DocumentType { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? FileSize { get; set; }

    [MaxLength(50)]
    public string? ContentType { get; set; }

    public DateTime UploadDate { get; set; }

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
