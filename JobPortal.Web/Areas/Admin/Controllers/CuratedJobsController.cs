using JobPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CuratedJobsController : Controller
    {
        private readonly ICuratedJobs _curatedJobs;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CuratedJobsController(ICuratedJobs curatedJobs, IWebHostEnvironment webHostEnvironment)
        {
            _curatedJobs = curatedJobs;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var lst = _curatedJobs.GetAll(true);
            return View(lst);
        }

        public IActionResult ManageJobs(int id = 0)
        {
            if (id.Equals(0))
            {
                ViewBag.Title = "Add Job";
                ViewBag.btnText = "Save";
                ViewBag.msg = "";
 
                return View(new CuratedJob());
            }
            else
            {
                ViewBag.Title = "Edit Job";
                ViewBag.btnText = "Update";
                ViewBag.msg = "";

                var job = _curatedJobs.GetById(id);
                job.PostedDate2 = DateOnly.Parse(job.PostedDate.ToShortDateString()) ;
                return View(job);
            }
        }


        [HttpPost]
        public IActionResult ManageJobs(CuratedJob job)
        {
            try
            {
                if(job.PostedDate2.Equals(null))
                {
                    ModelState.AddModelError("PostedDate2", "Posted Date is required.");
                    return View(job);
                }
                job.PostedDate = DateTime.Parse(job.PostedDate2.ToString());
                if (job.PKID == 0)
                {

                    int i = _curatedJobs.Add(job);
                    if (i > 0)
                    {
                        ViewBag.msg = "Job added successfully";
                        return RedirectToAction("index", "CuratedJobs");
                    }
                    else
                    {
                        ViewBag.msg = "Job couldn't be added successfully.";
                    }
                }
                else
                {
                    int i = _curatedJobs.Update(job);
                    if (i > 0)
                    {
                        ViewBag.msg = "Job updated successfully";
                        return RedirectToAction("index", "CuratedJobs");
                    }
                    else
                    {
                        ViewBag.msg = "Job couldn't be updated successfully.";
                    }
                }

            }
            catch (Exception ex)
            {

                //CardCommon.WriteErrorLog(ex.Message);
            }
            return View(job);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {

            var result = _curatedJobs.Delete(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Deleted Successfully" });
            }

            return Json(new { success = false, message = "Can't deleted." });
        }
    }
}
