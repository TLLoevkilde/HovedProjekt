using Microsoft.AspNetCore.Mvc;

namespace ResourceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Hello from the Resource API!" });
        }
    }
}
