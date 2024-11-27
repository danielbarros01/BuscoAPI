using Microsoft.AspNetCore.Mvc;

namespace BuscoAPI.Controllers
{
    [ApiController]
    [Route("/api/health")]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
        }

        [HttpGet("check")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
