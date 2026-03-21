using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace JobPortal.Web.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
  //  [Authorize(Roles = "JobSeeker")]
    public class SurveysController : Controller
    {

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly IWebHostEnvironment _env;
        private readonly IFileStorage _files;

        public SurveysController(AppDbContext db, IFileStorage files, UserManager<ApplicationUser> um, IWebHostEnvironment env)
        {
            _db = db; _um = um; _env = env; _files = files;
        }
        public IActionResult Index()
        {
            var list = _db.FormTypeCategory
           .Where(c => c.IsReadOnly == false && c.IsActive==true)
           .OrderBy(c => c.DisplayOrder)
           .ToList();
            return View(list);
        }



        public async Task<IActionResult> ViewSurvey(int id)
        {
            var userId = _um.GetUserId(User);

            var sections = await _db.FormSections
                .Where(s => s.IsActive && s.FormTypeCategoryId == id)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            var answers = await _db.FormAnswers
                .Where(a => a.UserId == userId)
                .ToListAsync();

           // var ratings = _db.QuestionRatings
           //.Where(r => r.UserId == userId)
           //.OrderByDescending(r => r.RatedAt)
           //.ToList();

           // foreach (var rate in ratings)
           // {
           //     rate.UserName = User.Identity != null ? User.Identity.Name : "";
           // }

           // var rategroup = ratings.GroupBy(m => m.RatedBy).ToList();

           // foreach (var rates in rategroup)
           // {
           //     int average = (int)rates.Average(m => m.Score);
           //     foreach (var rate in rates)
           //     {
           //         var ratebyUser = _um.FindByIdAsync(rate.RatedBy).Result;
           //         rate.RatingByName = ratebyUser.UserName;
           //         rate.AvgScore = average;
           //     }
           // }

            var results = new List<Dictionary<string, object>>();
            try
            {
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetRatingsByUserId_New";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@IsAdmin", 0));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId));
                    command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FormId", id));

                    await _db.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {


                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }

                            results.Add(row);
                        }


                    }
                }
            }
            catch
            {


            }

            var vm = new FormVm
            {
                FormCategoryId = id,
                Sections = sections,
                AnswersText = answers.Where(a => a.AnswerText != null).ToDictionary(a => a.QuestionId, a => a.AnswerText!),
                AnswersFile = answers.Where(a => a.FilePath != null).ToDictionary(a => a.QuestionId, a => a.FilePath!),
                
                AvgRating = 0,
                NewRatings = results
            };

            return View(vm);
        }



        public async Task<IActionResult> SaveForm(Dictionary<int, string> answers, List<IFormFile> files, string actionType, int formCategoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isDraft = actionType == "draft";
            foreach (var ans in answers)
            {
                var existing = await _db.FormAnswers
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.QuestionId == ans.Key);

                if (existing != null)
                {
                     
                    existing.AnswerText = ans.Value;
                    existing.IsDraft = isDraft;
                    existing.SubmittedAt = DateTime.UtcNow;

                }
                else
                {
                    _db.FormAnswers.Add(new FormAnswer
                    {
                        UserId = userId,
                        QuestionId = ans.Key,
                        AnswerText = ans.Value,
                        SubmittedAt = DateTime.UtcNow,
                        IsDraft = isDraft
                    });
                    await _db.SaveChangesAsync();
                }
            }

            // File uploads
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var qId = int.Parse(file.Name.Replace("file_", ""));
                    var path = Path.Combine("wwwroot/uploads", file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                        await file.CopyToAsync(stream);

                    var existing = await _db.FormAnswers
                        .FirstOrDefaultAsync(a => a.UserId == userId && a.QuestionId == qId);

                    if (existing != null)
                    {
                        existing.FilePath = "/uploads/" + file.FileName;
                        existing.SubmittedAt = DateTime.UtcNow;
                        existing.IsDraft = isDraft;
                    }
                    else
                    {
                        _db.FormAnswers.Add(new FormAnswer
                        {
                            UserId = userId,
                            QuestionId = qId,
                            FilePath = "/uploads/" + file.FileName,
                            SubmittedAt = DateTime.UtcNow,
                            IsDraft = isDraft
                        });
                    }
                }
            }

            if (isDraft)
                TempData["Message"] = "Draft saved successfully!";
            else
                TempData["Message"] = "Form submitted successfully!";

            await _db.SaveChangesAsync();
            return RedirectToAction("index", new { id= formCategoryId });
        }
    }
}
