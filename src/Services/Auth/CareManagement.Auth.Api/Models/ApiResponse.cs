namespace CareManagement.Auth.Api.Models;

/// <summary>
/// Standard API response wrapper for consistent response format across all endpoints
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public ApiResponse()
    {
    }

    public ApiResponse(T data, string message = "")
    {
        Success = true;
        Data = data;
        Message = message;
    }

    public ApiResponse(string message, List<string>? errors = null)
    {
        Success = false;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static ApiResponse<T> SuccessResult(T data, string message = "")
    {
        return new ApiResponse<T>(data, message);
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>(message, errors);
    }
}

/// <summary>
/// Non-generic API response for operations that don't return data
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public ApiResponse()
    {
    }

    public ApiResponse(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }

    public ApiResponse(string message, List<string>? errors = null)
    {
        Success = false;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static ApiResponse SuccessResult(string message = "")
    {
        return new ApiResponse(true, message);
    }

    public static ApiResponse ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse(message, errors);
    }
}
