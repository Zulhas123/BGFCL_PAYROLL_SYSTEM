using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers
{
	public class BanksController : Controller
	{
		public IActionResult Tags()
		{
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
		}
        public IActionResult Banks()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult BankAccounts()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult BankTypes()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult Branches()
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
