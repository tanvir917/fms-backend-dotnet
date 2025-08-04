using Microsoft.AspNetCore.Mvc;

namespace CareManagement.Staff.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "Staff API",
            Timestamp = DateTime.UtcNow
        });
    }
}
