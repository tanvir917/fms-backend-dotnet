using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareManagement.Staff.Api.Services;
using CareManagement.Staff.Api.DTOs;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/staff/{staffId}/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IStaffDocumentService _documentService;

    public DocumentsController(IStaffDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<StaffDocumentDto>>>> GetDocuments(int staffId)
    {
        try
        {
            var documents = await _documentService.GetDocumentsByStaffIdAsync(staffId);

            return Ok(new ApiResponse<List<StaffDocumentDto>>
            {
                Success = true,
                Data = documents.ToList(),
                Message = "Documents retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<StaffDocumentDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving documents",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<StaffDocumentDto>>> GetDocument(int staffId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.StaffId != staffId)
            {
                return NotFound(new ApiResponse<StaffDocumentDto>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            return Ok(new ApiResponse<StaffDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<StaffDocumentDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("upload")]
    public async Task<ActionResult<ApiResponse<StaffDocumentDto>>> UploadDocument(int staffId, [FromForm] StaffDocumentUploadDto uploadDto)
    {
        try
        {
            if (uploadDto.StaffId != staffId)
            {
                return BadRequest(new ApiResponse<StaffDocumentDto>
                {
                    Success = false,
                    Message = "Staff ID in URL does not match Staff ID in request body"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var document = await _documentService.CreateDocumentAsync(uploadDto, userId);

            return CreatedAtAction(nameof(GetDocument), new { staffId, id = document.Id }, new ApiResponse<StaffDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document uploaded successfully"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<StaffDocumentDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<StaffDocumentDto>
            {
                Success = false,
                Message = "An error occurred while uploading the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<StaffDocumentDto>>> UpdateDocument(int staffId, int id, [FromBody] UpdateStaffDocumentDto updateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var document = await _documentService.UpdateDocumentAsync(id, updateDto, userId);

            if (document == null || document.StaffId != staffId)
            {
                return NotFound(new ApiResponse<StaffDocumentDto>
                {
                    Success = false,
                    Message = "Document not found"
                });
            }

            return Ok(new ApiResponse<StaffDocumentDto>
            {
                Success = true,
                Data = document,
                Message = "Document updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<StaffDocumentDto>
            {
                Success = false,
                Message = "An error occurred while updating the document",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDocument(int staffId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.StaffId != staffId)
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
    public async Task<IActionResult> DownloadDocument(int staffId, int id)
    {
        try
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null || document.StaffId != staffId)
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
