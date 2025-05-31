using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers
{
    public class ReligionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
