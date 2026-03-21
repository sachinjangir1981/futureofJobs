using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Infrastructure.Services;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobPortal.Domain.Models;

namespace JobPortal.Web.Areas.Employer.Controllers;
[Area("Employer")]
//[Authorize(Roles = "Employer")]
public class JobsController : Controller
{
    private readonly IJobService _jobs;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _um;
    private readonly IJLocationServices _location;
    public JobsController(IJobService jobs, AppDbContext db, UserManager<ApplicationUser> um, IJLocationServices location)
    {
        _jobs = jobs;
        _db = db;
        _um = um;
        _location = location;

    }

    public async Task<IActionResult> Index()
    {
        var userId = _um.GetUserId(User);
        var guid = Guid.Parse(userId);
       
        var jobs = await _db.Jobs
        .Where(j => j.CompanyId == guid)
        .Include(j => j.JobSkills)
            .ThenInclude(js => js.Skill)
        .Include(j => j.State)
        .Include(j => j.City)
        .ToListAsync();
        return View(jobs);
    }

    public async Task<IActionResult> Create()
    {
        var userId = _um.GetUserId(User);
       
        

        ViewBag.States = await _db.States.ToListAsync();
         
        ViewBag.Skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();

        
        return View(new Job { Status = JobStatus.Published });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Job model, int[] selectedSkills)
    {
        try
        {

       
        var userId = _um.GetUserId(User);
        if (ModelState.IsValid)
        {
            model.CompanyId = Guid.Parse(userId);
                model.Slug = _jobs.GenerateSlug(model.Title);
                model.PublishedAt = DateTimeOffset.UtcNow;
                model.Status  = JobStatus.Published;
                model.NumberOfOpenings = model.Positions;
                _db.Jobs.Add(model);
            await _db.SaveChangesAsync();
            foreach (var skillId in selectedSkills)
            {
                _db.JobSkills.Add(new JobSkill
                {
                    JobId = model.Id,
                    SkillId = skillId
                });
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        ViewBag.States = await _db.States.OrderBy(s => s.Name).ToListAsync();
        ViewBag.Skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();
        return View(model);
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            
            ViewBag.States = await _db.States.ToListAsync();

            ViewBag.Skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();


            return View(new Job { Status = JobStatus.Published });

        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _um.GetUserId(User);
        var job = await _db.Jobs
            .Include(j => j.JobSkills)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

        if (job == null)
        {
            return NotFound();
        }

        ViewBag.States = await _db.States.ToListAsync();
        ViewBag.Districts = await _db.Cities.Where(d => d.StateId == job.StateId).ToListAsync();
        ViewBag.Skills = await _db.Skills.OrderBy(s => s.Name).ToListAsync();

        return View(job);

    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Job model, int[] selectedSkills)
    {
        var userId = _um.GetUserId(User);

        var job = await _db.Jobs
            .Include(j => j.JobSkills)
            .FirstOrDefaultAsync(j => j.Id == id && j.UserId == userId);

        if (job == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // Update scalar fields
            job.Title = model.Title;
            job.Description = model.Description;
            job.JobType = model.JobType;
            job.MinSalary = model.MinSalary;
            job.MaxSalary = model.MaxSalary;
            job.Positions = model.Positions;
            job.ExpiryAt = model.ExpiryAt;
            job.StateId = model.StateId;
            job.CityId = model.CityId;

            // Update skills
            job.JobSkills.Clear();
            foreach (var skillId in selectedSkills)
            {
                job.JobSkills.Add(new JobSkill
                {
                    JobId = job.Id,
                    SkillId = skillId
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);

    }

    public IActionResult Applicants(int id)
    {
        var apps = _db.JobApplications.Where(a => a.JobId == id).ToList();
        return View(apps);
    }

    [HttpPost]
    public IActionResult UpdateApplicationStatus(int applicationId, string status)
    {
        var app = _db.JobApplications.Find(applicationId);
        if (app != null)
        {
            if (System.Enum.TryParse<JobPortal.Domain.Models.ApplicationStatus>(status, out var s))
            {
                app.Status = s;
                _db.JobApplications.Update(app);
                _db.SaveChanges();
            }
        }
        return RedirectToAction("Applicants", new { id = app.JobId });
    }
}
