using Microsoft.AspNetCore.Mvc;

namespace SpaClient.Controllers
{
    public class SpaController : Controller
    {
        // Viser standardforsiden
        public IActionResult Index() => View();

        // Håndterer redirect fra authorization server efter login (fx med code)
        [HttpGet("callback")]
        public IActionResult Callback()
        {
            // Genbruger Index.cshtml, så JavaScript kan læse ?code=... fra URL'en
            return View("Index");
        }
    }
}
