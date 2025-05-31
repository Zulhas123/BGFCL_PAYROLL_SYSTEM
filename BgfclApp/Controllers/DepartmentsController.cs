using Microsoft.AspNetCore.Mvc;
using Entities;
using Contracts;
using Repositories;

namespace BgfclApp.Controllers
{
    public class DepartmentsController : Controller
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
