using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobPortal.Web.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
  //  [Authorize(Roles = "JobSeeker")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db; 
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var answers = _db.FormAnswers
                .Include(a => a.Question)
                .ThenInclude(q => q.Section)
                .Where(a => a.UserId == userId)
                .ToList();

            // Group answers by section for easier rendering
            var groupedAnswers = answers
                .GroupBy(a => a.Question.Section.Title)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Check status
            bool hasDraft = answers.Any(a => a.IsDraft);
            bool hasFinal = answers.Any(a => !a.IsDraft);

            ViewBag.HasDraft = hasDraft;
            ViewBag.HasFinal = hasFinal;

            return View(groupedAnswers);
        }
    }
}
