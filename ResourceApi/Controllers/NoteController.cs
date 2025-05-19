using Microsoft.AspNetCore.Mvc;

namespace ResourceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private static readonly List<string> Notes = new();

        [HttpGet]
        public IActionResult Get() => Ok(Notes);

        [HttpPost]
        public IActionResult Post([FromBody] string note)
        {
            if (string.IsNullOrWhiteSpace(note))
                return BadRequest("Note må ikke være tom.");

            Notes.Add(note);
            return Ok();
        }
    }
}
