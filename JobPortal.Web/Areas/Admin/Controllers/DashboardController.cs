using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Areas.Admin.Controllers;
[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        ViewBag.TotalJobs = _db.Jobs.Count();
        ViewBag.TotalEmployers = _db.Users.Count(u => _db.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId != Guid.Empty));
        ViewBag.TotalSeekers = _db.JobSeekerProfiles.Count();
        ViewBag.TotalApplications = _db.JobApplications.Count();
        return View();
    }
}
