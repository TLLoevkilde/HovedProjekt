using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using ResourceApi.Data;
using ResourceApi.Models;
using System.Security.Claims;

namespace ResourceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
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
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            if (!User.HasScope("note_api"))
            {
                return Forbid(authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "note_api",
                        [OpenIddictValidationAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InsufficientScope,
                        [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                            "The 'note_api' scope is required to access notes."
                    }));
            }

            var notes = await _db.Notes
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Id)
                .Select(n => n.Text)
                .ToListAsync();

            return Ok(notes);
        }

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    // Scope-tjek
        //    if (!User.HasScope("note_api"))
        //    {
        //        return Forbid(authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        //            properties: new AuthenticationProperties(new Dictionary<string, string>
        //            {
        //                [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "note_api",
        //                [OpenIddictValidationAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InsufficientScope,
        //                [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
        //                    "The 'note_api' scope is required to access notes."
        //            }));
        //    }

        //    var notes = await _db.Notes
        //        .OrderByDescending(n => n.Id)
        //        .Select(n => n.Text)
        //        .ToListAsync();

        //    return Ok(notes);
        //}


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string noteText)
        {
            var userId = User.FindFirst("sub")?.Value;
            if (userId == null)
                return Unauthorized();

            if (!User.HasScope("note_api"))
            {
                return Forbid(authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "note_api",
                        [OpenIddictValidationAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InsufficientScope,
                        [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                            "The 'note_api' scope is required to create notes."
                    }));
            }

            if (string.IsNullOrWhiteSpace(noteText))
                return BadRequest("Note må ikke være tom.");

            var note = new Note { Text = noteText, UserId = userId };
            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            return Ok();
        }



        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] string noteText)
        //{
        //    if (!User.HasScope("note_api"))
        //    {
        //        return Forbid(authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        //            properties: new AuthenticationProperties(new Dictionary<string, string>
        //            {
        //                [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "note_api",
        //                [OpenIddictValidationAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InsufficientScope,
        //                [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
        //                    "The 'note_api' scope is required to create notes."
        //            }));
        //    }

        //    if (string.IsNullOrWhiteSpace(noteText))
        //        return BadRequest("Note må ikke være tom.");

        //    var note = new Note { Text = noteText };
        //    _db.Notes.Add(note);
        //    await _db.SaveChangesAsync();

        //    return Ok();
        //}
    }
}














//using Microsoft.AspNetCore.Mvc;
//using ResourceApi.Data;
//using ResourceApi.Models;
//using Microsoft.EntityFrameworkCore;

//namespace ResourceApi.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class NoteController : ControllerBase
//    {
//        private readonly NotesDbContext _db;

//        public NoteController(NotesDbContext db)
//        {
//            _db = db;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Get()
//        {
//            var notes = await _db.Notes
//                .OrderByDescending(n => n.Id)
//                .Select(n => n.Text)
//                .ToListAsync();

//            return Ok(notes);
//        }

//        [HttpPost]
//        public async Task<IActionResult> Post([FromBody] string noteText)
//        {
//            if (string.IsNullOrWhiteSpace(noteText))
//                return BadRequest("Note må ikke være tom.");

//            var note = new Note { Text = noteText };
//            _db.Notes.Add(note);
//            await _db.SaveChangesAsync();

//            return Ok();
//        }
//    }
//}
