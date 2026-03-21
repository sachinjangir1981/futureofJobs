using JobPortal.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionCategoryController : Controller
    {
        
        private readonly IServey _servey;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public QuestionCategoryController(IServey servey, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _servey = servey;
        }
        public IActionResult Index()
        {
            var lst = _servey.GetAllServeyHeader(IsAdmin: true);
            return View(lst);
        }

        public IActionResult Create()
        {
            return View(new Questionnaires());
        }

        [HttpPost]
        public IActionResult Create(Questionnaires questionnaires, IFormFile flImageUrl, IFormFile flBgImageUrl)
        {
            try
            {


                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg", ".webp" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/polls/");

                string paperImage = questionnaires.ImageUrl == null ? "" : questionnaires.ImageUrl;

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

                        questionnaires.ImageUrl = newimagename;

                    }


                }
                else
                {
                    questionnaires.ImageUrl = paperImage;
                }



                string bgImage = questionnaires.BGImage == null ? "" : questionnaires.BGImage;

                if (flBgImageUrl != null && flBgImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flBgImageUrl.FileName).ToLower();
                    var newbgimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newbgimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flBgImageUrl.CopyTo(stream);
                        }

                        questionnaires.BGImage = newbgimagename;

                    }


                }
                else
                {
                    questionnaires.BGImage = bgImage;
                }


                int i = _servey.AddServeyHeader(questionnaires);
                if (i > 0)
                {
                    ViewBag.msg = "Category added successfully";
                    return Redirect("index");
                }
                else
                {
                    ViewBag.msg = "\"Category couldn't be added successfully.";
                }

            }
            catch (Exception ex)
            {

                //CardCommon.WriteErrorLog(ex.Message);
            }
            return View(questionnaires);
        }

        public IActionResult Edit(int? id)
        {
            ViewBag.msg = "";


            if (id.HasValue)
            {
                var costing = _servey.GetServeyHeaderById(id.Value);

                return View(costing);
            }
            else
            {
                return View("Index");
            }
        }


        [HttpPost]
        public IActionResult Edit(Questionnaires questionnaires, IFormFile flImageUrl, IFormFile flBgImageUrl)
        {
            try
            {


                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg", ".webp" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/polls/");

                string paperImage = questionnaires.ImageUrl == null ? "" : questionnaires.ImageUrl;

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

                        questionnaires.ImageUrl = newimagename;

                    }


                }
               


                string bgImage = questionnaires.BGImage == null ? "" : questionnaires.BGImage;

                if (flBgImageUrl != null && flBgImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flBgImageUrl.FileName).ToLower();
                    var newbgimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newbgimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flBgImageUrl.CopyTo(stream);
                        }

                        questionnaires.BGImage = newbgimagename;

                    }


                }
                else
                {
                    questionnaires.BGImage = bgImage;
                }



                int i = _servey.UpdateServeyHeader(questionnaires);
                if (i > 0)
                {
                    ViewBag.msg = "Category updated successfully";
                    return RedirectToAction("index", "QuestionCategory");
                }
                else
                {
                    ViewBag.msg = "Category couldn't be updated.";
                }

            }
            catch (Exception ex)
            {

                // CardCommon.WriteErrorLog(ex.Message);
            }
            return View(questionnaires);
        }


        public IActionResult Delete(int id)
        {
            JsonResultResponse jsonResultResponse = new JsonResultResponse();
            if (id > 0)
            {
                int sheetDetailId = _servey.DeleteServeyHeader(id);
                jsonResultResponse.Message = "Category deleted successfully";
                jsonResultResponse.Status = true;
            }
            else
            {
                jsonResultResponse.Message = "Category couldn't be deleted successfully";
                jsonResultResponse.Status = false;
            }

            return Json(jsonResultResponse); // Send data back as JSON
        }
    }
}
