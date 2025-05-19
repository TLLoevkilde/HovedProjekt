using Microsoft.AspNetCore.Mvc;
using ResourceApi.Data;
using ResourceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ResourceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly NotesDbContext _db;

        public NoteController(NotesDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var notes = await _db.Notes
                .OrderByDescending(n => n.Id)
                .Select(n => n.Text)
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string noteText)
        {
            if (string.IsNullOrWhiteSpace(noteText))
                return BadRequest("Note må ikke være tom.");

            var note = new Note { Text = noteText };
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
