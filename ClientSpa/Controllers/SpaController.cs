using Microsoft.AspNetCore.Mvc;

namespace SpaClient.Controllers
{
    public class SpaController : Controller
    {
        public IActionResult Index() => View();
    }
}
