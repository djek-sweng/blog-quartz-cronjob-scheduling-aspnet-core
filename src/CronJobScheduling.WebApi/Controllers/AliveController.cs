namespace CronJobScheduling.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AliveController : ControllerBase
{
    public AliveController()
    {
    }

    [HttpGet("HeartBeat")]
    public IActionResult GetHeartBeat()
    {
        return Ok($"Bumm, bumm. {DateTime.UtcNow}");
    }
}
