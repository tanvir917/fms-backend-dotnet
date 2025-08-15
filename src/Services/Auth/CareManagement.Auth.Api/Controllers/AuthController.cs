using CareManagement.Auth.Application.DTOs;
using CareManagement.Auth.Application.Services;
using CareManagement.Auth.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareManagement.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authService;
    private readonly IUserApplicationService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthApplicationService authService,
        IUserApplicationService userService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult("Invalid credentials"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResult("An error occurred during login"));
        }
    }

    [HttpPost("register")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id },
                ApiResponse<UserDto>.SuccessResult(user, "User registered successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
            return BadRequest(ApiResponse<UserDto>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred during registration"));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(response, "Token refreshed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Invalid refresh token attempt");
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResult("Invalid refresh token"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResult("An error occurred during token refresh"));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        try
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ApiResponse.ErrorResult("Token is required"));
            }

            await _authService.LogoutAsync(token);
            return Ok(ApiResponse.SuccessResult("Logged out successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred during logout"));
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ApiResponse.ErrorResult("User not found"));
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result)
            {
                return Ok(ApiResponse.SuccessResult("Password changed successfully"));
            }
            else
            {
                return BadRequest(ApiResponse.ErrorResult("Failed to change password"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while changing password"));
        }
    }

    [HttpPost("users/{id}/reset-password")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ApiResponse>> ResetPassword(int id, [FromBody] ResetPasswordRequestDto request)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(id, request.NewPassword);
            if (result)
            {
                return Ok(ApiResponse.SuccessResult("Password reset successfully"));
            }
            else
            {
                return BadRequest(ApiResponse.ErrorResult("Failed to reset password"));
            }
        }
        catch (ArgumentException ex)
        {
            return NotFound(ApiResponse.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {UserId}", id);
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while resetting password"));
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ApiResponse<UserDto>.ErrorResult("User not found"));
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResult(user, "Profile retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred while retrieving profile"));
        }
    }

    [HttpGet("users")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResult("An error occurred while retrieving users"));
        }
    }

    [HttpGet("users/{id}")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult("User not found"));
            }

            return Ok(ApiResponse<UserDto>.SuccessResult(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred while retrieving user"));
        }
    }

    [HttpPut("users/{id}")]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
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
            return StatusCode(500, ApiResponse<UserDto>.ErrorResult("An error occurred while updating user"));
        }
    }

    [HttpDelete("users/{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                return Ok(ApiResponse.SuccessResult("User deactivated successfully"));
            }
            else
            {
                return NotFound(ApiResponse.ErrorResult("User not found"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return StatusCode(500, ApiResponse.ErrorResult("An error occurred while deleting user"));
        }
    }
}
