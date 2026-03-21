using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    public class QuestionMasterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
