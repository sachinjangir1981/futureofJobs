using System.Security.Cryptography;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Web.AdminForms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminFormSubmissionsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public AdminFormSubmissionsController(AppDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index(int[]? formId, string? userId, string? search)
        {
            bool hasFiltered = Request.Query.ContainsKey("filtered");

            var raw = await _db.FormAnswers
                .Where(a => !a.IsDraft)
                .Select(a => new
                {
                    a.UserId,
                    a.SubmittedAt,
                    a.FormSessionEntryId,
                    a.QuestionId,
                    HasAnswer = (a.AnswerText != null && a.AnswerText != "") || (a.FilePath != null && a.FilePath != "") || a.AnswerId != 0,
                    FormId = a.Question.Section.FormTypeCategoryId
                })
                .ToListAsync();

            var totalQuestions = await _db.FormQuestions
                .Where(q => q.Section.IsActive)
                .GroupBy(q => q.Section.FormTypeCategoryId)
                .Select(g => new { FormId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.FormId, x => x.Count);

            var formNames = await _db.FormTypeCategory.ToDictionaryAsync(f => f.Id, f => f.FormCategory);

            var grouped = raw
                .GroupBy(a => a.UserId)
                .Select(g =>
                {
                    var byForm = g.GroupBy(a => a.FormId).ToList();
                    // Answered = distinct questions with an answer in the user's latest version of each form.
                    var answeredByForm = byForm.ToDictionary(f => f.Key, f =>
                    {
                        var latestVersion = f.Max(a => a.FormSessionEntryId);
                        return f.Where(a => a.FormSessionEntryId == latestVersion && a.HasAnswer)
                                .Select(a => a.QuestionId).Distinct().Count();
                    });
                    return new AdminFormSubmissionListItem
                    {
                        UserId = g.Key,
                        FormIds = byForm.Select(f => f.Key).ToList(),
                        FormNames = byForm
                            .Select(f => formNames.TryGetValue(f.Key, out var name) ? name : $"Form #{f.Key}")
                            .ToList(),
                        FormAnswerCounts = answeredByForm,
                        FormTotalQuestions = totalQuestions,
                        AnsweredQuestions = answeredByForm.Values.Sum(),
                        LastSubmittedAt = g.Max(a => a.SubmittedAt)
                    };
                })
                .ToList();

            var userIds = grouped.Select(g => g.UserId).Distinct().ToList();
            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id.ToString()))
                .ToDictionaryAsync(u => u.Id.ToString(), u => u);

            foreach (var item in grouped)
            {
                if (users.TryGetValue(item.UserId, out var u))
                {
                    item.UserName = u.UserName ?? "";
                    item.Email = u.Email ?? "";
                }
            }

            // Per-user filled-forms map, used client-side to populate the Forms dropdown the
            // moment a user is picked, without waiting for a Filter round-trip. Computed now,
            // before `result` below may mutate these same item objects for display purposes.
            ViewBag.UserFormsJson = grouped.ToDictionary(
                g => g.UserId,
                g => g.FormIds.Zip(g.FormNames, (id, name) => new { id, name }).ToList());

            // Scope the Forms dropdown to just the selected user's filled forms. Until a user is
            // picked there's nothing meaningful to scope to, so only "All Forms" is offered.
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userFormIds = grouped.FirstOrDefault(g => g.UserId == userId)?.FormIds ?? new List<int>();
                ViewBag.Forms = await _db.FormTypeCategory
                    .Where(f => userFormIds.Contains(f.Id))
                    .OrderBy(f => f.DisplayOrder)
                    .ToListAsync();
            }
            else
            {
                ViewBag.Forms = new List<FormTypeCategory>();
            }

            var selectedFormIds = (formId ?? Array.Empty<int>()).Where(id => id != 0).ToList();

            var result = new List<AdminFormSubmissionListItem>();
            if (hasFiltered)
            {
                result = grouped;
                if (selectedFormIds.Any())
                    result = result.Where(g => g.FormIds.Any(id => selectedFormIds.Contains(id))).ToList();

                if (!string.IsNullOrWhiteSpace(userId))
                    result = result.Where(g => g.UserId == userId).ToList();

                if (!string.IsNullOrWhiteSpace(search))
                    result = result.Where(g =>
                        (g.UserName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (g.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

                result = result.OrderByDescending(g => g.LastSubmittedAt).ToList();

                // Once specific forms are selected, show only those forms and count answers
                // from just those forms rather than everything the user has ever filled.
                if (selectedFormIds.Any())
                {
                    foreach (var item in result)
                    {
                        var relevantFormIds = item.FormIds.Where(id => selectedFormIds.Contains(id)).ToList();
                        item.FormIds = relevantFormIds;
                        item.FormNames = relevantFormIds
                            .Select(id => formNames.TryGetValue(id, out var name) ? name : $"Form #{id}")
                            .ToList();
                        item.AnsweredQuestions = relevantFormIds
                            .Sum(id => item.FormAnswerCounts.TryGetValue(id, out var count) ? count : 0);
                    }
                }
            }

            if (result.Any())
            {
                var answeredSessionIds = raw.Select(a => a.FormSessionEntryId).ToHashSet();
                var versionRows = (await _db.Database
                    .SqlQuery<AdminFormVersionRow>($"SELECT Id, FormId, UserId, FormVersion AS Label, DATEADD(minute, 330, RcdInsTs) AS CreatedAt FROM FormSessionEntry")
                    .ToListAsync())
                    .Where(v => answeredSessionIds.Contains(v.Id))
                    .ToList();

                foreach (var item in result)
                {
                    if (!Guid.TryParse(item.UserId, out var itemUserGuid))
                        continue;

                    item.FormVersions = versionRows
                        .Where(v => v.UserId == itemUserGuid && v.FormId.HasValue)
                        .GroupBy(v => v.FormId!.Value)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(v => v.CreatedAt)
                                  .Select(v => new AdminFormVersion { Id = v.Id, Label = v.Label ?? "", CreatedAt = v.CreatedAt ?? default })
                                  .ToList());
                }
            }

            ViewBag.Users = users.Values.OrderBy(u => u.UserName).ToList();
            ViewBag.FormId = selectedFormIds;
            ViewBag.UserId = userId;
            ViewBag.Search = search;
            ViewBag.HasFiltered = hasFiltered;

            return View(result);
        }

        public async Task<IActionResult> ViewSubmission(string userId, int[]? formIds, int? versionId)
        {
            var requestedFormIds = (formIds ?? Array.Empty<int>()).Where(id => id != 0).ToList();
            var vm = await FormSubmissionBuilder.BuildAsync(_db, _um, userId, requestedFormIds, versionId);
            if (vm == null)
                return NotFound();

            ViewBag.RequestedFormIds = requestedFormIds;
            ViewBag.ShareLinks = await _db.AdminShareLinks
                .Where(l => l.UserId == userId && l.RevokedAt == null && l.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateShareLink(string userId, int[]? formIds)
        {
            var requestedFormIds = (formIds ?? Array.Empty<int>()).Where(id => id != 0).ToList();

            var link = new AdminShareLink
            {
                Token = GenerateToken(),
                UserId = userId,
                FormIdsCsv = requestedFormIds.Any() ? string.Join(",", requestedFormIds) : null,
                CreatedByUserId = _um.GetUserId(User) ?? "",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
            _db.AdminShareLinks.Add(link);
            await _db.SaveChangesAsync();

            return RedirectToAction("ViewSubmission", new { userId, formIds = requestedFormIds });
        }

        [HttpPost]
        public async Task<IActionResult> RevokeShareLink(int id, string userId)
        {
            var link = await _db.AdminShareLinks.FindAsync(id);
            if (link != null && link.RevokedAt == null)
            {
                link.RevokedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ViewSubmission", new { userId });
        }

        private static string GenerateToken()
        {
            // 256 bits of randomness, URL-safe.
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
}
