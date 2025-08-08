using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareManagement.Client.Api.Services;
using CareManagement.Client.Api.DTOs;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Client.Api.Controllers;

[ApiController]
[Route("api/clients/{clientId}/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly IClientNoteService _noteService;

    public NotesController(IClientNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ClientNoteDto>>>> GetNotes(int clientId, [FromQuery] bool includePrivate = false)
    {
        try
        {
            var notes = await _noteService.GetNotesByClientIdAsync(clientId, includePrivate);

            return Ok(new ApiResponse<List<ClientNoteDto>>
            {
                Success = true,
                Data = notes.ToList(),
                Message = "Notes retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<ClientNoteDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving notes",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClientNoteDto>>> GetNote(int clientId, int id)
    {
        try
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null || note.ClientId != clientId)
            {
                return NotFound(new ApiResponse<ClientNoteDto>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            return Ok(new ApiResponse<ClientNoteDto>
            {
                Success = true,
                Data = note,
                Message = "Note retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientNoteDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClientNoteDto>>> CreateNote(int clientId, [FromBody] CreateClientNoteDto createDto)
    {
        try
        {
            if (createDto.ClientId != clientId)
            {
                return BadRequest(new ApiResponse<ClientNoteDto>
                {
                    Success = false,
                    Message = "Client ID in URL does not match Client ID in request body"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var note = await _noteService.CreateNoteAsync(createDto, userId);

            return CreatedAtAction(nameof(GetNote), new { clientId, id = note.Id }, new ApiResponse<ClientNoteDto>
            {
                Success = true,
                Data = note,
                Message = "Note created successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientNoteDto>
            {
                Success = false,
                Message = "An error occurred while creating the note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClientNoteDto>>> UpdateNote(int clientId, int id, [FromBody] UpdateClientNoteDto updateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var note = await _noteService.UpdateNoteAsync(id, updateDto, userId);

            if (note == null || note.ClientId != clientId)
            {
                return NotFound(new ApiResponse<ClientNoteDto>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            return Ok(new ApiResponse<ClientNoteDto>
            {
                Success = true,
                Data = note,
                Message = "Note updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientNoteDto>
            {
                Success = false,
                Message = "An error occurred while updating the note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNote(int clientId, int id)
    {
        try
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null || note.ClientId != clientId)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            var success = await _noteService.DeleteNoteAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Note deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the note",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
