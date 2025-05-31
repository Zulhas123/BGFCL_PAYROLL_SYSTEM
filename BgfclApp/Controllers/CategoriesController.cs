using Microsoft.AspNetCore.Mvc;
using Entities;
using Contracts;


namespace BgfclApp.Controllers
{
    [Route("[controller]/[action]")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryContract _categoryContract;
        public CategoriesController(ICategoryContract categoryContract)
        {

            _categoryContract = categoryContract;

        }
        public IActionResult Index()
        {
            //var categories = await _categoryContract.GetCategories();
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult CreateCategory()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryName")] Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategories = await _categoryContract.GetCategories();
                    var existingCategory = existingCategories.Where(c => c.CategoryName == category.CategoryName).SingleOrDefault();
                    if (existingCategory == null)
                    {
                        category.CreatedBy = "";
                        category.CreatedDate = DateTime.Now;
                        category.IsActive = true;
                        int result = await _categoryContract.CreateCategory(category);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(category);
                    }

                }
                else
                {
                    return View(category);
                }

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }

        }

        public async Task<IActionResult> EditCategory(int id)
        {
            try
            {
                Category category = await _categoryContract.GetCategory(id);
                if (category != null)
                {
                    return View(category);
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {
            try
            {
                Category _category = await _categoryContract.GetCategory(category.Id);
                if (_category != null)
                {
                    _category.CategoryName = category.CategoryName;
                    _category.UpdatedBy = "";
                    _category.UpdatedDate = DateTime.Now;
                    int result = await _categoryContract.UpdateCategory(_category);
                    return RedirectToAction("Index");

                }
                else
                {
                    return View(_category);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<IActionResult> RemoveCategory(int id)
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            try
            {
                Category category = await _categoryContract.GetCategory(id);
                if (category != null)
                {
                    await _categoryContract.RemoveCategory(category.Id);
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }


    }
}
