using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CareManagement.Client.Api.Data;
using CareManagement.Client.Api.DTOs;
using CareManagement.Client.Api.Models;

namespace CareManagement.Client.Api.Services;

public class ClientNoteService : IClientNoteService
{
    private readonly ClientDbContext _context;
    private readonly IMapper _mapper;

    public ClientNoteService(ClientDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClientNoteDto>> GetNotesByClientIdAsync(int clientId, bool includePrivate = false)
    {
        var query = _context.ClientNotes
            .Include(n => n.Client)
            .Where(n => n.ClientId == clientId);

        if (!includePrivate)
        {
            query = query.Where(n => !n.IsPrivate);
        }

        var notes = await query
            .OrderByDescending(n => n.NoteDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ClientNoteDto>>(notes);
    }

    public async Task<ClientNoteDto?> GetNoteByIdAsync(int id)
    {
        var note = await _context.ClientNotes
            .Include(n => n.Client)
            .FirstOrDefaultAsync(n => n.Id == id);

        return note == null ? null : _mapper.Map<ClientNoteDto>(note);
    }

    public async Task<ClientNoteDto> CreateNoteAsync(CreateClientNoteDto createDto, string? createdBy = null)
    {
        var note = _mapper.Map<ClientNote>(createDto);
        note.CreatedBy = createdBy;
        note.UpdatedBy = createdBy;

        if (!createDto.NoteDate.HasValue)
        {
            note.NoteDate = DateTime.UtcNow;
        }

        _context.ClientNotes.Add(note);
        await _context.SaveChangesAsync();

        // Reload with client data
        await _context.Entry(note)
            .Reference(n => n.Client)
            .LoadAsync();

        return _mapper.Map<ClientNoteDto>(note);
    }

    public async Task<ClientNoteDto?> UpdateNoteAsync(int id, UpdateClientNoteDto updateDto, string? updatedBy = null)
    {
        var note = await _context.ClientNotes
            .Include(n => n.Client)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null)
        {
            return null;
        }

        _mapper.Map(updateDto, note);
        note.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();

        return _mapper.Map<ClientNoteDto>(note);
    }

    public async Task<bool> DeleteNoteAsync(int id)
    {
        var note = await _context.ClientNotes.FindAsync(id);
        if (note == null)
        {
            return false;
        }

        _context.ClientNotes.Remove(note);
        await _context.SaveChangesAsync();

        return true;
    }
}
