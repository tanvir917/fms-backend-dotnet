using CareManagement.Client.Api.DTOs;

namespace CareManagement.Client.Api.Services.Interfaces;

public interface IClientNoteService
{
    Task<IEnumerable<ClientNoteDto>> GetNotesByClientIdAsync(int clientId, bool includePrivate = false);
    Task<ClientNoteDto?> GetNoteByIdAsync(int id);
    Task<ClientNoteDto> CreateNoteAsync(CreateClientNoteDto createDto, string? createdBy = null);
    Task<ClientNoteDto?> UpdateNoteAsync(int id, UpdateClientNoteDto updateDto, string? updatedBy = null);
    Task<bool> DeleteNoteAsync(int id);
}
