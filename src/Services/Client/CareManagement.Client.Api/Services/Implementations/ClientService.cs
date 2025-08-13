using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CareManagement.Client.Api.Data;
using CareManagement.Client.Api.DTOs;
using CareManagement.Client.Api.Models;
using CareManagement.Client.Api.Services.Interfaces;
using CareManagement.Shared.Messaging;
using CareManagement.Shared.Events;

namespace CareManagement.Client.Api.Services.Implementations;

public class ClientService : IClientService
{
    private readonly ClientDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ClientService> _logger;

    public ClientService(ClientDbContext context, IMapper mapper, IMessageBus messageBus, ILogger<ClientService> logger)
    {
        _context = context;
        _mapper = mapper;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<(IEnumerable<ClientDto> Clients, int TotalCount)> GetClientsAsync(ClientSearchDto searchDto)
    {
        var query = _context.Clients
            .Include(c => c.CarePlans)
            .Include(c => c.Documents)
            .Include(c => c.ClientNotes)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(searchDto.Name))
        {
            var nameFilter = searchDto.Name.ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(nameFilter) ||
                c.LastName.ToLower().Contains(nameFilter) ||
                (c.PreferredName != null && c.PreferredName.ToLower().Contains(nameFilter)));
        }

        if (!string.IsNullOrEmpty(searchDto.Email))
        {
            query = query.Where(c => c.Email != null && c.Email.ToLower().Contains(searchDto.Email.ToLower()));
        }

        if (!string.IsNullOrEmpty(searchDto.PhoneNumber))
        {
            query = query.Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(searchDto.PhoneNumber));
        }

        if (searchDto.CareLevel.HasValue)
        {
            query = query.Where(c => c.CareLevel == searchDto.CareLevel.Value);
        }

        if (searchDto.Status.HasValue)
        {
            query = query.Where(c => c.Status == searchDto.Status.Value);
        }

        if (searchDto.AdmissionDateFrom.HasValue)
        {
            query = query.Where(c => c.AdmissionDate >= searchDto.AdmissionDateFrom.Value);
        }

        if (searchDto.AdmissionDateTo.HasValue)
        {
            query = query.Where(c => c.AdmissionDate <= searchDto.AdmissionDateTo.Value);
        }

        if (searchDto.AgeFrom.HasValue || searchDto.AgeTo.HasValue)
        {
            var today = DateTime.Today;
            if (searchDto.AgeFrom.HasValue)
            {
                var maxBirthDate = today.AddYears(-searchDto.AgeFrom.Value);
                query = query.Where(c => c.DateOfBirth <= maxBirthDate);
            }
            if (searchDto.AgeTo.HasValue)
            {
                var minBirthDate = today.AddYears(-searchDto.AgeTo.Value - 1);
                query = query.Where(c => c.DateOfBirth > minBirthDate);
            }
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        if (!string.IsNullOrEmpty(searchDto.SortBy))
        {
            query = searchDto.SortBy.ToLower() switch
            {
                "firstname" => searchDto.SortDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                "lastname" => searchDto.SortDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                "dateofbirth" => searchDto.SortDescending ? query.OrderByDescending(c => c.DateOfBirth) : query.OrderBy(c => c.DateOfBirth),
                "admissiondate" => searchDto.SortDescending ? query.OrderByDescending(c => c.AdmissionDate) : query.OrderBy(c => c.AdmissionDate),
                "carelevel" => searchDto.SortDescending ? query.OrderByDescending(c => c.CareLevel) : query.OrderBy(c => c.CareLevel),
                "status" => searchDto.SortDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
                _ => searchDto.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedAt);
        }

        // Apply pagination
        var clients = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .ToListAsync();

        var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);

        return (clientDtos, totalCount);
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var client = await _context.Clients
            .Include(c => c.CarePlans)
            .Include(c => c.Documents)
            .Include(c => c.ClientNotes)
            .FirstOrDefaultAsync(c => c.Id == id);

        return client == null ? null : _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto createDto, string? createdBy = null)
    {
        var client = _mapper.Map<Models.Client>(createDto);
        client.CreatedBy = createdBy;
        client.UpdatedBy = createdBy;

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Publish client created event
        try
        {
            var clientCreatedEvent = new ClientCreatedEvent
            {
                ClientId = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                DateOfBirth = client.DateOfBirth,
                Address = client.Address,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                CareLevel = client.CareLevel.ToString(),
                CreatedAt = client.CreatedAt,
                CreatedBy = createdBy
            };

            await _messageBus.PublishAsync("client.created", clientCreatedEvent);
            _logger.LogInformation("Published ClientCreatedEvent for client {ClientId}", client.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ClientCreatedEvent for client {ClientId}", client.Id);
            // Don't throw - the client was successfully created, messaging is secondary
        }

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<ClientDto?> UpdateClientAsync(int id, UpdateClientDto updateDto, string? updatedBy = null)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return null;
        }

        _mapper.Map(updateDto, client);
        client.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        // Publish client updated event
        try
        {
            var clientUpdatedEvent = new ClientUpdatedEvent
            {
                ClientId = client.Id,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                PhoneNumber = client.PhoneNumber,
                CareLevel = client.CareLevel.ToString(),
                UpdatedAt = client.UpdatedAt,
                UpdatedBy = updatedBy
            };

            await _messageBus.PublishAsync("client.updated", clientUpdatedEvent);
            _logger.LogInformation("Published ClientUpdatedEvent for client {ClientId}", client.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ClientUpdatedEvent for client {ClientId}", client.Id);
            // Don't throw - the client was successfully updated, messaging is secondary
        }

        return _mapper.Map<ClientDto>(client);
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return false;
        }

        // Store client info for event before deletion
        var clientInfo = new
        {
            client.Id,
            client.FirstName,
            client.LastName
        };

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        // Publish client deleted event
        try
        {
            var clientDeletedEvent = new ClientDeletedEvent
            {
                ClientId = clientInfo.Id,
                FirstName = clientInfo.FirstName,
                LastName = clientInfo.LastName,
                DeletedAt = DateTime.UtcNow,
                DeletedBy = null // Could be enhanced to include the user who deleted
            };

            await _messageBus.PublishAsync("client.deleted", clientDeletedEvent);
            _logger.LogInformation("Published ClientDeletedEvent for client {ClientId}", clientInfo.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ClientDeletedEvent for client {ClientId}", clientInfo.Id);
            // Don't throw - the client was successfully deleted, messaging is secondary
        }

        return true;
    }

    public async Task<ClientStatsDto> GetClientStatsAsync()
    {
        var totalClients = await _context.Clients.CountAsync();
        var activeClients = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Active);
        var inactiveClients = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Inactive);
        var dischargedClients = await _context.Clients.CountAsync(c => c.Status == ClientStatus.Discharged);

        var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var nextMonth = currentMonth.AddMonths(1);

        var newClientsThisMonth = await _context.Clients
            .CountAsync(c => c.AdmissionDate >= currentMonth && c.AdmissionDate < nextMonth);

        var dischargedThisMonth = await _context.Clients
            .CountAsync(c => c.DischargeDate >= currentMonth && c.DischargeDate < nextMonth);

        var clientsByCareLevel = await _context.Clients
            .GroupBy(c => c.CareLevel)
            .Select(g => new { CareLevel = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CareLevel, x => x.Count);

        var clientsByStatus = await _context.Clients
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        return new ClientStatsDto
        {
            TotalClients = totalClients,
            ActiveClients = activeClients,
            InactiveClients = inactiveClients,
            DischargedClients = dischargedClients,
            ClientsByCareLevel = clientsByCareLevel,
            ClientsByStatus = clientsByStatus,
            NewClientsThisMonth = newClientsThisMonth,
            DischargedThisMonth = dischargedThisMonth
        };
    }
}
