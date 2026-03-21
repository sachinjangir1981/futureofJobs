using JobPortal.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OneMinuteController : Controller
    {
        private readonly IOneMinute _oneMinute;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public OneMinuteController(IOneMinute oneMinute, IWebHostEnvironment webHostEnvironment)
        {
            _oneMinute = oneMinute;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {

            try
            {

                var lst = _oneMinute.GetAllOneMinuteItem(2);
                return View(lst);

            }
            catch
            {
                return View(null);

            }


        }

        public ActionResult Create()
        {
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

            _oneMinute.Add(media, 2);
            return RedirectToAction("index");



        }

        public ActionResult Edit(int id)
        {

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

            _oneMinute.Update(media,2);

            return View(media);
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
