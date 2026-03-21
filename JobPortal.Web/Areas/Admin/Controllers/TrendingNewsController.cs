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
    public class TrendingNewsController : Controller
    {

        private readonly IWpPostModel _wpPost;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TrendingNewsController(IWpPostModel wpPost, IWebHostEnvironment webHostEnvironment)
        {
            _wpPost = wpPost;
            _webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index(int id)
        {
            int catId = id; // Default to 167 if no category is specified
            var catlist = _wpPost.GetAllCategory(0);
            ViewBag.CategoryList = catlist;
            if(catId ==0)
            {
                catId = catlist[0].PKID;
            }
            ViewBag.SelectedCategory = catId;
            var lst = _wpPost.GetAllPosts(categoryId: catId);
            return View(lst);
        }

        public ActionResult Create()
        {
            var catlist = _wpPost.GetAllCategory(0);
            ViewBag.CategoryList = catlist;
            return View(new WpPost()); ;
        }

        [HttpPost]
        public ActionResult Create(WpPost post, IFormFile fCoverPhoto)
        {

            try
            {
                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
                string coverphoto = post.CoverImage == null ? "" : post.CoverImage;

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

                        post.CoverImage = newimagename;
                    }


                }
                else
                {
                    post.CoverImage = coverphoto;
                }

                post.IsHKContent = false;
                _wpPost.AddPost(post);
                return RedirectToAction("Index");
            }
            catch 
            {
               
                return View();

            }

        }

        public ActionResult Edit(int id)
        {
            var post = _wpPost.SelectById(id);
            var catlist = _wpPost.GetAllCategory(0);
            ViewBag.CategoryList = catlist;
            return View(post); 
        }

        [HttpPost]
        public ActionResult Edit(WpPost post, IFormFile fCoverPhoto)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/snippet/");
            string coverphoto = post.CoverImage == null ? "" : post.CoverImage;

            if (fCoverPhoto != null && fCoverPhoto.Length > 0)
            {
                var ext = Path.GetExtension(fCoverPhoto.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext))  
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fCoverPhoto.CopyTo(stream);
                    }

                    post.CoverImage = newimagename;
                }


            }
            else
            {
                post.CoverImage = coverphoto;
            }


            post.IsHKContent = false;
            _wpPost.UpdatePost(post);
            return RedirectToAction("Index");
        }

       [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _wpPost.Delete(id);
                return Json(new { msg = "success" });
            }
            catch (Exception)
            {

                return Json(new { msg = "error" });
            }
        }
    }
}
