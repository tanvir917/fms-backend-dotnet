using CareManagement.Client.Api.DTOs;
using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.Services;

public interface IClientService
{
    Task<(IEnumerable<ClientDto> Clients, int TotalCount)> GetClientsAsync(ClientSearchDto searchDto);
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto createDto, string? createdBy = null);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto updateDto, string? updatedBy = null);
    Task<bool> DeleteClientAsync(int id);
    Task<ClientStatsDto> GetClientStatsAsync();
}

public interface ICarePlanService
{
    Task<IEnumerable<CarePlanDto>> GetCarePlansByClientIdAsync(int clientId);
    Task<CarePlanDto?> GetCarePlanByIdAsync(int id);
    Task<CarePlanDto> CreateCarePlanAsync(CreateCarePlanDto createDto, string? createdBy = null);
    Task<CarePlanDto?> UpdateCarePlanAsync(int id, UpdateCarePlanDto updateDto, string? updatedBy = null);
    Task<bool> DeleteCarePlanAsync(int id);
    Task<CarePlanDto?> ActivateCarePlanAsync(ActivateCarePlanDto activateDto, string? updatedBy = null);
}

public interface IClientDocumentService
{
    Task<IEnumerable<ClientDocumentDto>> GetDocumentsByClientIdAsync(int clientId);
    Task<ClientDocumentDto?> GetDocumentByIdAsync(int id);
    Task<ClientDocumentDto> CreateDocumentAsync(DocumentUploadDto uploadDto, string? createdBy = null);
    Task<ClientDocumentDto?> UpdateDocumentAsync(int id, UpdateClientDocumentDto updateDto, string? updatedBy = null);
    Task<bool> DeleteDocumentAsync(int id);
    Task<(byte[] Content, string ContentType, string FileName)?> DownloadDocumentAsync(int id);
}

public interface IClientNoteService
{
    Task<IEnumerable<ClientNoteDto>> GetNotesByClientIdAsync(int clientId, bool includePrivate = false);
    Task<ClientNoteDto?> GetNoteByIdAsync(int id);
    Task<ClientNoteDto> CreateNoteAsync(CreateClientNoteDto createDto, string? createdBy = null);
    Task<ClientNoteDto?> UpdateNoteAsync(int id, UpdateClientNoteDto updateDto, string? updatedBy = null);
    Task<bool> DeleteNoteAsync(int id);
}
