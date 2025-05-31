using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers
{
    public class PayrollDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
