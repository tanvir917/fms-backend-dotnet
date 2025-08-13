using CareManagement.Client.Api.DTOs;

namespace CareManagement.Client.Api.Services.Interfaces;

public interface IClientDocumentService
{
    Task<IEnumerable<ClientDocumentDto>> GetDocumentsByClientIdAsync(int clientId);
    Task<ClientDocumentDto?> GetDocumentByIdAsync(int id);
    Task<ClientDocumentDto> CreateDocumentAsync(DocumentUploadDto uploadDto, string? createdBy = null);
    Task<ClientDocumentDto?> UpdateDocumentAsync(int id, UpdateClientDocumentDto updateDto, string? updatedBy = null);
    Task<bool> DeleteDocumentAsync(int id);
    Task<(byte[] Content, string ContentType, string FileName)?> DownloadDocumentAsync(int id);
}
