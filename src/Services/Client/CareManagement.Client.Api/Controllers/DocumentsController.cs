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
public class DocumentsController : ControllerBase
{
    private readonly IClientDocumentService _documentService;

    public DocumentsController(IClientDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ClientDocumentDto>>>> GetDocuments(int clientId)
    {
        try
        {
            var documents = await _documentService.GetDocumentsByClientIdAsync(clientId);

            return Ok(new ApiResponse<List<ClientDocumentDto>>
            {
                Success = true,
                Data = documents.ToList(),
                Message = "Documents retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<ClientDocumentDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving documents",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDocumentDto>>> GetDocument(int clientId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.ClientId != clientId)
            {
                return NotFound(new ApiResponse<ClientDocumentDto>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            return Ok(new ApiResponse<ClientDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDocumentDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<ClientDocumentDto>>> UploadDocument(int clientId, [FromForm] DocumentUploadDto uploadDto)
    {
        try
        {
            if (uploadDto.ClientId != clientId)
            {
                return BadRequest(new ApiResponse<ClientDocumentDto>
                {
                    Success = false,
                    Message = "Client ID in URL does not match Client ID in request body"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var document = await _documentService.CreateDocumentAsync(uploadDto, userId);

            return CreatedAtAction(nameof(GetDocument), new { clientId, id = document.Id }, new ApiResponse<ClientDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document uploaded successfully"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<ClientDocumentDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDocumentDto>
            {
                Success = false,
                Message = "An error occurred while uploading the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ClientDocumentDto>>> UpdateDocument(int clientId, int id, [FromBody] UpdateClientDocumentDto updateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var document = await _documentService.UpdateDocumentAsync(id, updateDto, userId);

            if (document == null || document.ClientId != clientId)
            {
                return NotFound(new ApiResponse<ClientDocumentDto>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            return Ok(new ApiResponse<ClientDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ClientDocumentDto>
            {
                Success = false,
                Message = "An error occurred while updating the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDocument(int clientId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.ClientId != clientId)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            var success = await _documentService.DeleteDocumentAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Document deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(int clientId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.ClientId != clientId)
            {
                return NotFound();
            }

            var downloadResult = await _documentService.DownloadDocumentAsync(id);
            if (downloadResult == null)
            {
                return NotFound();
            }

            var (content, contentType, fileName) = downloadResult.Value;
            return File(content, contentType, fileName);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while downloading the document");
        }
    }
}
