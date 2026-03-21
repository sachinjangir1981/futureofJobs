
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal.Domain.Models;
using Microsoft.AspNetCore.Authorization;
namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
  //  [Authorize(Roles = "Admin")]
    public class SkillCategoryController : Controller
    {
        private readonly AppDbContext _db;

        public SkillCategoryController(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _db.SkillCategories
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
            return View(categories);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SkillCategory category)
        {
            if (ModelState.IsValid)
            {
                _db.SkillCategories.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.SkillCategories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SkillCategory category)
        {
            if (ModelState.IsValid)
            {
                _db.SkillCategories.Update(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.SkillCategories.FindAsync(id);
            if (category == null) return NotFound();
            _db.SkillCategories.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

