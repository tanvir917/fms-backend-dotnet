using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareManagement.Auth.Api.DTOs;
using CareManagement.Auth.Api.Services.Interfaces;
using CareManagement.Shared.DTOs;
using System.Security.Claims;

namespace CareManagement.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", request.Username);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            var response = await _authService.GetProfileAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile");
            return StatusCode(500, new { message = "An error occurred while retrieving profile" });
        }
    }
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            await _authService.ChangePasswordAsync(userId, request);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    [HttpPost("admin/change-password/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminChangePassword(int id, [FromBody] AdminChangePasswordRequest request)
    {
        try
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminUserId))
            {
                return Unauthorized(new { message = "Admin user not found" });
            }

            var result = await _authService.AdminResetPasswordAsync(id, request, adminUserId);
            if (result)
            {
                return Ok(new { message = "Password changed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to change password" });
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }
    [HttpGet("users")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetUsersAsync();
            return Ok(ApiResponse<List<UserDto>>.SuccessResult(users.ToList()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, ApiResponse<List<UserDto>>.ErrorResult("An error occurred"));
        }
    }

    [HttpGet("users/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResult(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpPut("users/{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var userDto = await _userService.UpdateUserAsync(id, request);
            return Ok(ApiResponse<UserDto>.SuccessResult(userDto, "User updated successfully"));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred"));
        }
    }

    [HttpDelete("users/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                return Ok(ApiResponse<object>.SuccessResult(new { }, "User deactivated successfully"));
            }
            else
            {
                return NotFound(ApiResponse<object>.ErrorResult("User not found"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred"));
        }
    }
}
