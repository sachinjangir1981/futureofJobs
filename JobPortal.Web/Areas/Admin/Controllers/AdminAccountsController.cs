using JobPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly IFormFeeRepository _formFee;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminAccountsController(IFormFeeRepository formFee, IWebHostEnvironment webHostEnvironment)
        {
            _formFee = formFee;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int id = 1)
        {
            ViewBag.RoleId = id;
            var lst = _formFee.GetAllFormFeesByRoleId(id);
            return View(lst);
        }

        public IActionResult UpdateFormFee(FormFee model)
        {
            int i = _formFee.AddUpdateFormFee(model);
            if (i > 0)
                return Json(new { success = true, message = "Updated successfully." });
            else
                return Json(new { success = false, message = "Can't deleted." });

        }

        public IActionResult GetFeeChangeHistory(FormFee model)
        {
            var lst = _formFee.GetFeeHistoryByRoleIdFormId(model.RoleId, model.Id);

            return Json(new { success = true,data= lst, message = lst.Count.ToString() +" records fetched" });

        }


    }
}
