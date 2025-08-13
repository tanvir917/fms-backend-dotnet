using CareManagement.Client.Api.DTOs;

namespace CareManagement.Client.Api.Services.Interfaces;

public interface IClientService
{
    Task<(IEnumerable<ClientDto> Clients, int TotalCount)> GetClientsAsync(ClientSearchDto searchDto);
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto createDto, string? createdBy = null);
    Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto updateDto, string? updatedBy = null);
    Task<bool> DeleteClientAsync(int id);
    Task<ClientStatsDto> GetClientStatsAsync();
}
