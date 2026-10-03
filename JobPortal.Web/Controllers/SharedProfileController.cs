using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Web.AdminForms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Controllers
{
    // Public, unauthenticated viewer for admin-generated share links (see
    // AdminFormSubmissionsController.GenerateShareLink). Deliberately outside the Admin area
    // and carries no [Authorize] - anyone holding a valid, unexpired token can view it.
    public class SharedProfileController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public SharedProfileController(AppDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return NotFound();

            var link = await _db.AdminShareLinks.FirstOrDefaultAsync(l => l.Token == token);
            if (link == null || link.RevokedAt != null || link.ExpiresAt <= DateTime.UtcNow)
                return View("Expired");

            var formIds = string.IsNullOrWhiteSpace(link.FormIdsCsv)
                ? new List<int>()
                : link.FormIdsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

            var vm = await FormSubmissionBuilder.BuildAsync(_db, _um, link.UserId, formIds);
            if (vm == null)
                return View("Expired");

            return View(vm);
        }
    }
}
