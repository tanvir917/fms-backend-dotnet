using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareManagement.Client.Api.Services;
using CareManagement.Client.Api.DTOs;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Client.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ClientDto>>>> GetClients([FromQuery] ClientSearchDto searchDto)
    {
        try
        {
            var (clients, totalCount) = await _clientService.GetClientsAsync(searchDto);

            var pagedResponse = new PaginatedResponse<ClientDto>
            {
                Results = clients.ToList(),
                Count = totalCount
            };

            return Ok(new ApiResponse<PaginatedResponse<ClientDto>>
            {
                Success = true,
                Message = "Clients retrieved successfully",
                Data = pagedResponse,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<PaginatedResponse<ClientDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving clients",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> GetClient(int id)
    {
        try
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound(new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "Client not found"
                });
            }

            return Ok(new ApiResponse<ClientDto>
            {
                Success = true,
                Data = client,
                Message = "Client retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the client",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ClientDto>>> CreateClient([FromBody] CreateClientDto createDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = await _clientService.CreateClientAsync(createDto, userId);

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, new ApiResponse<ClientDto>
            {
                Success = true,
                Data = client,
                Message = "Client created successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDto>
            {
                Success = false,
                Message = "An error occurred while creating the client",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDto>>> UpdateClient(int id, [FromBody] UpdateClientDto updateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var client = await _clientService.UpdateClientAsync(id, updateDto, userId);

            if (client == null)
            {
                return NotFound(new ApiResponse<ClientDto>
                {
                    Success = false,
                    Message = "Client not found"
                });
            }

            return Ok(new ApiResponse<ClientDto>
            {
                Success = true,
                Data = client,
                Message = "Client updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDto>
            {
                Success = false,
                Message = "An error occurred while updating the client",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteClient(int id)
    {
        try
        {
            var success = await _clientService.DeleteClientAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Client not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Client deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the client",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<ClientStatsDto>>> GetClientStats()
    {
        try
        {
            var stats = await _clientService.GetClientStatsAsync();

            return Ok(new ApiResponse<ClientStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "Client statistics retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientStatsDto>
            {
                Success = false,
                Message = "An error occurred while retrieving client statistics",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
