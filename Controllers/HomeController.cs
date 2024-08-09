using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult Get([FromServices] IConfiguration config)
        {
            var environment = config.GetValue<string>("Env");
            return Ok(new { message = "Estou vivo! 🤖", environment });
        }
    }
}
