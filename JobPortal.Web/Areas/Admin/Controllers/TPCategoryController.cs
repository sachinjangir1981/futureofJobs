using Microsoft.AspNetCore.Mvc;
using JobPortal.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyModel;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TPCategoryController : Controller
    {
        private readonly ITimePassCategory _timePassCategory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TPCategoryController(ITimePassCategory timePassCategory, IWebHostEnvironment webHostEnvironment)
        {
            _timePassCategory = timePassCategory;
            _webHostEnvironment = webHostEnvironment;
        }
       
       

        public IActionResult Index()
        {
            var lst = _timePassCategory.GetAllCaseCade();
            return View(lst);
        }


        public IActionResult ManageCategory(int id = 0)
        {
            if (id.Equals(0))
            {
                ViewBag.Title = "Add Category";
                ViewBag.btnText = "Save";
                ViewBag.msg = "";


                var parent = new TimePassCategories() { PKID = 0, TPCategory = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _timePassCategory.GetAllCaseCade();
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;
                return View(new TimePassCategories());
            }
            else
            {
                ViewBag.Title = "Edit Category";
                ViewBag.btnText = "Update";
                ViewBag.msg = "";
                var parent = new TimePassCategories() { PKID = 0, TPCategory = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _timePassCategory.GetAllCaseCade();
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;
                var category = _timePassCategory.GetById(id);
                return View(category);
            }

        }

        [HttpPost]
        public IActionResult ManageCategory(TimePassCategories library)
        {
            try
            {

                if (library.PKID == 0)
                {

                    int i = _timePassCategory.Add(library);
                    if (i > 0)
                    {
                        ViewBag.msg = "Category added successfully";
                        return RedirectToAction("index", "TPCategory");
                    }
                    else
                    {
                        ViewBag.msg = "\"category couldn't be added successfully.";
                    }
                }
                else
                {
                    int i = _timePassCategory.Update(library);
                    if (i > 0)
                    {
                        ViewBag.msg = "Category updated successfully";
                        return RedirectToAction("index", "TPCategory");
                    }
                    else
                    {
                        ViewBag.msg = "\"Category couldn't be updated successfully.";
                    }
                }

            }
            catch (Exception ex)
            {

                //CardCommon.WriteErrorLog(ex.Message);
            }
            return View(library);
        }


        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {

            var result = _timePassCategory.Delete(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Deleted Successfully" });
            }

            return Json(new { success = false, message = "Can't deleted." });
        }

         
    }
}
