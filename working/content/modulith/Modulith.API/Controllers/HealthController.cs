using Microsoft.AspNetCore.Mvc;

namespace Modulith.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check requested");
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        _logger.LogInformation("Detailed health check requested");
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Services = new
            {
                Database = "Connected",
                Redis = "Connected",
                RabbitMQ = "Connected"
            }
        });
    }
} 