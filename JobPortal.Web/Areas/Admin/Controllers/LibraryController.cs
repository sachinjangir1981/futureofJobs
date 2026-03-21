using Microsoft.AspNetCore.Mvc;
using JobPortal.Web.Models;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LibraryController : Controller
    {
        private readonly ILibrary _library;
        private readonly ILibraryCategory _libraryCategory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LibraryController(ILibrary library, ILibraryCategory libraryCategory, IWebHostEnvironment webHostEnvironment)
        {

            _library = library;
            _libraryCategory = libraryCategory;
            _webHostEnvironment = webHostEnvironment;

        }



        public IActionResult Index(int? id)
        {
            int categoryId = 0;
            if (id.HasValue)
            {
                categoryId = id.Value;
            }

            //var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };

            //var allcategory = _libraryCategory.GetAllByParentId(0);
            //allcategory.Insert(0, parent);
            //ViewBag.ParentCategories = allcategory;

            var lst = _library.GetAllCategoryforLibrary(categoryId);
            return View(lst);
        }


        public IActionResult Create(int? id)
        {
            var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
            var allcategory = _libraryCategory.GetAllCaseCade();
            allcategory.Insert(0, parent);
            ViewBag.ParentCategories = allcategory;
            var model = new LibraryModel { Level1 = 0, Level2 = 0, Level3 = 0, DisplayOrder = 0, CategoryId = id.HasValue == true ? id.Value : 0 };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(LibraryModel library, IFormFile flImageUrl)
        {
            try
            {

                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/library/");

                string paperImage = library.ImageUrl == null ? "" : library.ImageUrl;

                if (flImageUrl != null && flImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrl.CopyTo(stream);
                        }

                        library.ImageUrl = newimagename;

                    }


                }
                else
                {
                    library.ImageUrl = paperImage;
                }

                int i = _library.AddToLibrary(library);
                if (i > 0)
                {
                    ViewBag.msg = "library added successfully";
                    return RedirectToAction("index");
                }
                else
                {
                    var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
                    var allcategory = _libraryCategory.GetAllCaseCade();
                    allcategory.Insert(0, parent);
                    ViewBag.ParentCategories = allcategory;
                    ViewBag.msg = "\"library couldn't be added successfully.";
                }

            }
            catch (Exception ex)
            {

                //CardCommon.WriteErrorLog(ex.Message);
            }
            return View(library);
        }

        public IActionResult Edit(int? id)
        {
            ViewBag.msg = "";


            if (id.HasValue)
            {
                var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _libraryCategory.GetAllCaseCade();
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;

                var library = _library.GetById(id.Value);

                return View(library);
            }
            else
            {
                return View("Index");
            }
        }


        [HttpPost]
        public IActionResult Edit(LibraryModel library, IFormFile flImageUrl)
        {
            try
            {

                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/library/");

                string paperImage = library.ImageUrl == null ? "" : library.ImageUrl;

                if (flImageUrl != null && flImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrl.CopyTo(stream);
                        }

                        library.ImageUrl = newimagename;

                    }


                }
                else
                {
                    library.ImageUrl = paperImage;
                }


                int i = _library.UpdateToLibrary(library);
                if (i > 0)
                {
                    ViewBag.msg = "Item updated successfully";
                    return RedirectToAction("index", "library");
                }
                else
                {
                    var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
                    var allcategory = _libraryCategory.GetAllCaseCade();
                    allcategory.Insert(0, parent);
                    ViewBag.ParentCategories = allcategory;
                    ViewBag.msg = "Item couldn't be updated.";
                }

            }
            catch (Exception ex)
            {

                // CardCommon.WriteErrorLog(ex.Message);
            }
            return View(library);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {

            var result = _library.DeleteLibrary(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Deleted Successfully" });
            }

            return Json(new { success = false, message = "Can't deleted." });
        }

        [HttpPost]
        public IActionResult Duplicate(int id)
        {

            var result = _library.Duplicate(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Record created successfully" });
            }

            return Json(new { success = false, message = "Error! can't created." });
        }


        public IActionResult Categories()
        {
            var lst = _libraryCategory.GetAllCaseCade();
            return View(lst);
        }


        public IActionResult ManageCategory(int id = 0)
        {
            if (id.Equals(0))
            {
                ViewBag.Title = "Add Category";
                ViewBag.btnText = "Save";
                ViewBag.msg = "";


                var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _libraryCategory.GetAllCaseCade();
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;
                return View(new LibraryCategoryModel());
            }
            else
            {
                ViewBag.Title = "Edit Category";
                ViewBag.btnText = "Update";
                ViewBag.msg = "";
                var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _libraryCategory.GetAllCaseCade();
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;
                var category = _libraryCategory.GetById(id);
                return View(category);
            }

        }

        [HttpPost]
        public IActionResult ManageCategory(LibraryCategoryModel library)
        {
            try
            {

                if (library.PKID == 0)
                {

                    int i = _libraryCategory.Add(library);
                    if (i > 0)
                    {
                        ViewBag.msg = "library added successfully";
                        return Redirect("Categories");
                    }
                    else
                    {
                        ViewBag.msg = "\"library couldn't be added successfully.";
                    }
                }
                else
                {
                    int i = _libraryCategory.Update(library);
                    if (i > 0)
                    {
                        ViewBag.msg = "library updated successfully";
                        return RedirectToAction("Categories", "Library");
                    }
                    else
                    {
                        ViewBag.msg = "\"library couldn't be updated successfully.";
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

            var result = _libraryCategory.Delete(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Deleted Successfully" });
            }

            return Json(new { success = false, message = "Can't deleted." });
        }

        [HttpPost]
        public IActionResult UpdateCategory(int id, int categoryId)
        {

            var result = _library.UpdateCategory(id, categoryId);
            if (result > 0)
            {

                return Json(new { success = true, message = "Updated Successfully" });
            }

            return Json(new { success = false, message = "Can't update." });
        }


        public IActionResult ProductList(int? id, string CatSNO)
        {
            ViewBag.CatSNO = CatSNO + ".";
            //int categoryId = 0;
            //if (id.HasValue)
            //{
            //    categoryId = id.Value;
            //}
            ////var lst = _library.GetAll();
            //var parent = new LibraryCategoryModel() { PKID = 0, Title = "Select", ParentId = 0, Path = "Level (0)" };
            //var allcategory = _libraryCategory.GetAllCaseCade();
            //allcategory.Insert(0, parent);
            //ViewBag.ParentCategories = allcategory;

            //var lst = _library.GetAllLibraryAndCategory(categoryId);
            var lst = _library.GetAllLibraryItemByCategory(id.Value);
            return PartialView(lst);
        }

        [HttpPost]
        public IActionResult UpdateLevel(int id, int displayOrder, int level1, int level2, int level3, int level4)
        {

            var result = _library.UpdateLevel(id, displayOrder, level1, level2, level3);

            int categoryId = 0;
            if (level4 > 0)
            {
                categoryId = level4;
            }
            else
           if (level3 > 0)
            {
                categoryId = level3;
            }
            else if (level2 > 0)
            {
                categoryId = level2;
            }
            else if (level1 > 0)
            {
                categoryId = level1;
            }

            result = _library.UpdateCategory(id, categoryId);
            if (result > 0)
            {

                return Json(new { success = true, message = "Updated Successfully" });
            }

            return Json(new { success = false, message = "Can't update." });
        }


        [HttpGet]
        public IActionResult GetCategoriesByParent(int id)
        {
            var lst = _libraryCategory.GetAllByParentId(id);

            return Ok(lst);
        }

        [HttpGet]
        public IActionResult GetAllLibraryItemsByCategoryId(int id)
        {
            var lst = _library.GetAllLibraryItemByCategory(id);

            return PartialView("_LibraryItemByCategoryId", lst);
        }

        [HttpPost]
        public IActionResult DuplicateCategory(int id)
        {

            var result = _library.DuplicateCategoryWithAllLevels(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Record created successfully" });
            }

            return Json(new { success = false, message = "Error! can't created." });
        }

    }
}

