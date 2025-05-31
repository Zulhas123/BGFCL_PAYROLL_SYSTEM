using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers
{
    public class DesignationsController : Controller
    {
        public IActionResult Index()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
    }
}
