using JobPortal.Models;
using JobPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using System.Text;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TNCategoryController : Controller
    {
        private readonly IWpPostModel _wpPost;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TNCategoryController(IWpPostModel wpPost, IWebHostEnvironment webHostEnvironment)
        {
            _wpPost = wpPost;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            var lst = _wpPost.GetAllCategory(0);
            return View(lst);
        }

        public ActionResult Create()
        {
            
             
            return View(new WpPostCategory()); ;
        }

        [HttpPost]
        public ActionResult Create(WpPostCategory post, IFormFile fImageUrl, IFormFile fBgImageUrl)
        {

            try
            {
                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
                string coverphoto = post.ImageUrl == null ? "" : post.ImageUrl;

                if (fImageUrl != null && fImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(fImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(coverphotoBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            fImageUrl.CopyTo(stream);
                        }

                        post.ImageUrl = newimagename;
                    }


                }
                else
                {
                    post.ImageUrl = coverphoto;
                }



                string bgphoto = post.BgImageUrl == null ? "" : post.BgImageUrl;
                
                if (fBgImageUrl != null && fBgImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(fBgImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(coverphotoBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            fBgImageUrl.CopyTo(stream);
                        }

                        post.BgImageUrl = newimagename;
                    }


                }
                else
                {
                    post.BgImageUrl = bgphoto;
                }


                _wpPost.AddCategory(post);
                return RedirectToAction("Index");
            }
            catch
            {

                return View();

            }

        }

        public ActionResult Edit(int id)
        {

            var post = _wpPost.SelectCategoryById(id);
            
            return View(post); ;
        }

        [HttpPost]
        public ActionResult Edit(WpPostCategory post, IFormFile fImageUrl, IFormFile fBgImageUrl)
        {
            

            var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
            string coverphoto = post.ImageUrl == null ? "" : post.ImageUrl;

            if (fImageUrl != null && fImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fImageUrl.CopyTo(stream);
                    }

                    post.ImageUrl = newimagename;
                }


            }
            else
            {
                post.ImageUrl = coverphoto;
            }



            string bgphoto = post.BgImageUrl == null ? "" : post.BgImageUrl;

            if (fBgImageUrl != null && fBgImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fBgImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext)) //check what type of extension  
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fBgImageUrl.CopyTo(stream);
                    }

                    post.BgImageUrl = newimagename;
                }


            }
            else
            {
                post.BgImageUrl = bgphoto;
            }


           
            _wpPost.UpdateCategory(post);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _wpPost.DeleteCategory(id);
                return Json(new { msg = "success" });
            }
            catch (Exception)
            {

                return Json(new { msg = "error" });
            }
        }
    }
}
