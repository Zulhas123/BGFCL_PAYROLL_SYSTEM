using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers
{
    public class SchoolsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
