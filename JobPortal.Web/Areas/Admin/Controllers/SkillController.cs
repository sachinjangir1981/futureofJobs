using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
  //s  [Authorize(Roles = "Admin")]
    public class SkillController : Controller
    {
        private readonly AppDbContext _db;

        public SkillController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var skills = await _db.Skills
     .Join(_db.SkillCategories,
           skill => skill.SkillCategoryId,
           cat => cat.Id,
           (skill, cat) => new { Skill = skill, Category = cat })
     .OrderBy(x => x.Category.DisplayOrder)
     .ThenBy(x => x.Skill.Name)
     .Select(x => x.Skill)
     .Include(s => s.SkillCategory)
     .ToListAsync();
            return View(skills);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_db.SkillCategories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _db.Skills.Add(skill);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_db.SkillCategories, "Id", "Name", skill.SkillCategoryId);
            return View(skill);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var skill = await _db.Skills.FindAsync(id);
            if (skill == null) return NotFound();

            ViewBag.Categories = new SelectList(_db.SkillCategories, "Id", "Name", skill.SkillCategoryId);
            return View(skill);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Skill skill)
        {
            if (ModelState.IsValid)
            {
                _db.Skills.Update(skill);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_db.SkillCategories, "Id", "Name", skill.SkillCategoryId);
            return View(skill);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var skill = await _db.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
