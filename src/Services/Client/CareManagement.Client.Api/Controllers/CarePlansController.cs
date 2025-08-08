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
public class CarePlansController : ControllerBase
{
    private readonly ICarePlanService _carePlanService;

    public CarePlansController(ICarePlanService carePlanService)
    {
        _carePlanService = carePlanService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CarePlanDto>>>> GetCarePlans(int clientId)
    {
        try
        {
            var carePlans = await _carePlanService.GetCarePlansByClientIdAsync(clientId);

            return Ok(new ApiResponse<List<CarePlanDto>>
            {
                Success = true,
                Data = carePlans.ToList(),
                Message = "Care plans retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<CarePlanDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving care plans",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CarePlanDto>>> GetCarePlan(int clientId, int id)
    {
        try
        {
            var carePlan = await _carePlanService.GetCarePlanByIdAsync(id);
            if (carePlan == null || carePlan.ClientId != clientId)
            {
                return NotFound(new ApiResponse<CarePlanDto>
                {
                    Success = false,
                    Message = "Care plan not found"
                });
            }

            return Ok(new ApiResponse<CarePlanDto>
            {
                Success = true,
                Data = carePlan,
                Message = "Care plan retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CarePlanDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the care plan",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CarePlanDto>>> CreateCarePlan(int clientId, [FromBody] CreateCarePlanDto createDto)
    {
        try
        {
            if (createDto.ClientId != clientId)
            {
                return BadRequest(new ApiResponse<CarePlanDto>
                {
                    Success = false,
                    Message = "Client ID in URL does not match Client ID in request body"
                });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var carePlan = await _carePlanService.CreateCarePlanAsync(createDto, userId);

            return CreatedAtAction(nameof(GetCarePlan), new { clientId, id = carePlan.Id }, new ApiResponse<CarePlanDto>
            {
                Success = true,
                Data = carePlan,
                Message = "Care plan created successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CarePlanDto>
            {
                Success = false,
                Message = "An error occurred while creating the care plan",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CarePlanDto>>> UpdateCarePlan(int clientId, int id, [FromBody] UpdateCarePlanDto updateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var carePlan = await _carePlanService.UpdateCarePlanAsync(id, updateDto, userId);

            if (carePlan == null || carePlan.ClientId != clientId)
            {
                return NotFound(new ApiResponse<CarePlanDto>
                {
                    Success = false,
                    Message = "Care plan not found"
                });
            }

            return Ok(new ApiResponse<CarePlanDto>
            {
                Success = true,
                Data = carePlan,
                Message = "Care plan updated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CarePlanDto>
            {
                Success = false,
                Message = "An error occurred while updating the care plan",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCarePlan(int clientId, int id)
    {
        try
        {
            var carePlan = await _carePlanService.GetCarePlanByIdAsync(id);
            if (carePlan == null || carePlan.ClientId != clientId)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Care plan not found"
                });
            }

            var success = await _carePlanService.DeleteCarePlanAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Care plan not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Care plan deleted successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the care plan",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("activate")]
    public async Task<ActionResult<ApiResponse<CarePlanDto>>> ActivateCarePlan(int clientId, [FromBody] ActivateCarePlanDto activateDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var carePlan = await _carePlanService.ActivateCarePlanAsync(activateDto, userId);

            if (carePlan == null || carePlan.ClientId != clientId)
            {
                return NotFound(new ApiResponse<CarePlanDto>
                {
                    Success = false,
                    Message = "Care plan not found"
                });
            }

            return Ok(new ApiResponse<CarePlanDto>
            {
                Success = true,
                Data = carePlan,
                Message = "Care plan activated successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CarePlanDto>
            {
                Success = false,
                Message = "An error occurred while activating the care plan",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
