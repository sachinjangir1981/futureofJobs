using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Areas.Academics.Controllers
{
    [Area("Academics")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
