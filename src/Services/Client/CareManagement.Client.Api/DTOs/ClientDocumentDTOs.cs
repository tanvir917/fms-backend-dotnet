using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.DTOs;

public class ClientDocumentDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? FileSize { get; set; }
    public string? ContentType { get; set; }
    public DateTime UploadDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public ClientDto? Client { get; set; }
}

public class CreateClientDocumentDto
{
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
}

public class UpdateClientDocumentDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DocumentType? DocumentType { get; set; }
}

public class DocumentUploadDto
{
    public int ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
    public IFormFile File { get; set; } = null!;
}
