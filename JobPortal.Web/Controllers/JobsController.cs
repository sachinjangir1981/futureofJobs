using Microsoft.AspNetCore.Mvc;
using JobPortal.Infrastructure.Services;

namespace JobPortal.Web.Controllers;
public class JobsController : Controller
{
    private readonly IJobService _jobs;
    public JobsController(IJobService jobs) => _jobs = jobs;

    public IActionResult Index(string q, int page = 1)
    {
        var results = _jobs.Search(q ?? string.Empty, page, 20);
        return View(results);
    }

    public IActionResult Details(int id)
    {
        var job = _jobs.GetById(id);
        if (job == null) return NotFound();
        return View(job);
    }
}
