using JobPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TimePass5LineController : Controller
    {
        private readonly IOneMinute _oneMinute;
        private readonly ITimePassCategory _timePassCategory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TimePass5LineController(ITimePassCategory timePassCategory, IOneMinute oneMinute, IWebHostEnvironment webHostEnvironment)
        {
            _oneMinute = oneMinute;
            _webHostEnvironment = webHostEnvironment;
            _timePassCategory = timePassCategory;
        }
        public IActionResult Index(int? id)
        {

            try
            {
                int LanguageId = 0;
                if (id == null)
                {
                    LanguageId = 0;
                }
                else
                {
                    LanguageId = id.Value;
                }
                ViewBag.LanguageId = LanguageId;
                var lst = _oneMinute.GetAllOneMinuteItem(LanguageId);
                var parent = new TimePassCategories() { PKID = 0, TPCategory = "Select", ParentId = 0, Path = "Level (0)" };
                var allcategory = _timePassCategory.GetAllCaseCade(false, true);
                allcategory.Insert(0, parent);
                ViewBag.ParentCategories = allcategory;
                return View(lst);

            }
            catch
            {
                return View(null);

            }
        }

        public ActionResult Create()
        {
            var allcategory = _timePassCategory.GetAllCaseCade(false, true);
            ViewBag.ParentCategories = allcategory;
            return View(new OneMinuteItem());
        }

        [HttpPost]

        public ActionResult Create(OneMinuteItem media, IFormFile fCoverPhoto)
        {

            var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
            string coverphoto = media.CoverPhoto == null ? "" : media.CoverPhoto;

            if (fCoverPhoto != null && fCoverPhoto.Length > 0)
            {
                var ext = Path.GetExtension(fCoverPhoto.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fCoverPhoto.CopyTo(stream);
                    }

                    media.CoverPhoto = newimagename;
                }


            }
            else
            {
                media.CoverPhoto = coverphoto;
            }
            
            _oneMinute.Add(media, media.LanguageId);
            return RedirectToAction("index");



        }

        public ActionResult Edit(int id)
        {
            var allcategory = _timePassCategory.GetAllCaseCade(false, true);
            ViewBag.ParentCategories = allcategory;
            var cust = _oneMinute.GetOneMinuteItemById(id);
            return View(cust);

        }

        [HttpPost]

        public ActionResult Edit(OneMinuteItem media, IFormFile fCoverPhoto)
        {


            var allowedExtensions = new[] { ".Jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
            string coverphoto = media.CoverPhoto == null ? "" : media.CoverPhoto;

            if (fCoverPhoto != null && fCoverPhoto.Length > 0)
            {
                var ext = Path.GetExtension(fCoverPhoto.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fCoverPhoto.CopyTo(stream);
                    }

                    media.CoverPhoto = newimagename;
                }


            }
            else
            {
                media.CoverPhoto = coverphoto;
            }
            
            _oneMinute.Update(media, media.LanguageId);
            return RedirectToAction("index");
            // return View(media);
        }

        public ActionResult Card(int? id)
        {


            if (id == null)
            {

                return RedirectToAction("index");
            }
            else
            {

                try
                {

                    var itm = _oneMinute.GetOneMinuteItemById(id.Value);
                    return View(itm);
                }
                catch
                {
                    return RedirectToAction("index");

                }
            }

        }

        public ActionResult Delete(int id)
        {
            try
            {

                _oneMinute.DeleteById(id);
                return Json(new { status = true, msg = "News deleted successfully" });
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
