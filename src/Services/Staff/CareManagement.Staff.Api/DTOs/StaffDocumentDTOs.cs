using System.ComponentModel.DataAnnotations;
using CareManagement.Staff.Api.Data;

namespace CareManagement.Staff.Api.DTOs;

public class StaffDocumentDto
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public StaffDocumentType DocumentType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FormattedFileSize { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public int UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateStaffDocumentDto
{
    [Required]
    public int StaffId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public StaffDocumentType DocumentType { get; set; }
}

public class UpdateStaffDocumentDto
{
    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public StaffDocumentType? DocumentType { get; set; }
}

public class StaffDocumentUploadDto
{
    [Required]
    public int StaffId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public StaffDocumentType DocumentType { get; set; }

    [Required]
    public IFormFile File { get; set; } = null!;
}
