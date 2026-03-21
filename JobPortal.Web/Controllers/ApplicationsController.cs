using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Infrastructure.Services;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace JobPortal.Web.Controllers;
public class ApplicationsController : Controller
{
    private readonly IApplicationService _apps;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _um;

    public ApplicationsController(IApplicationService apps, AppDbContext db, UserManager<ApplicationUser> um)
    {
        _apps = apps; _db = db; _um = um;
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPost]
    public IActionResult Apply(int jobId)
    {
        var userId = Guid.Parse(_um.GetUserId(User));
        var profile = _db.JobSeekerProfiles.FirstOrDefault(p => p.UserId == userId);
        if (profile == null) return RedirectToAction("Edit", "Profile", new { area = "JobSeeker" });
        _apps.Apply(jobId, profile.Id);
        return RedirectToAction("Details", "Jobs", new { id = jobId });
    }

    [Authorize(Roles = "JobSeeker")]
    public IActionResult MyApplications()
    {
        var userId = Guid.Parse(_um.GetUserId(User));
        var profile = _db.JobSeekerProfiles.FirstOrDefault(p => p.UserId == userId);
        if (profile == null) return RedirectToAction("Edit", "Profile", new { area = "JobSeeker" });
        var list = _apps.GetForSeeker(profile.Id);
        return View(list);
    }
}
