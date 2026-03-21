using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = "Admin")]
    public class ProfilesController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        public ProfilesController(AppDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }


        public IActionResult Index(string status = "all")
        {
            var users = _db.Users.ToList();
            //var profiles = _db.JobSeekerProfiles.ToList();

            var profiles  = (
    from p in _db.JobSeekerProfiles
    join u in _db.Users on p.UserId equals u.Id
    select p
).ToList();

            var userId = _um.GetUserId(User);
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var formAnswers = _db.FormAnswers
                .Include(a => a.Question)
                .ThenInclude(q => q.Section)
                 .Include(s => s.Question)
                 .ThenInclude(s => s.QuestionRatings.Where(r => r.RatedBy == adminId))
                 
                .ToList();

            // Group answers by user
            var grouped = formAnswers
                .Where(a => a.Question.Section.FormTypeCategoryId  == 1)
                .GroupBy(a => a.ProfileId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Apply filter
            if (status == "draft")
                grouped = grouped.Where(x => x.Value.Any(a => a.IsDraft)).ToDictionary(x => x.Key, x => x.Value);
            else if (status == "final")
                grouped = grouped.Where(x => x.Value.Any(a => !a.IsDraft)).ToDictionary(x => x.Key, x => x.Value);

            ViewBag.Users = users.ToDictionary(u => u.Id, u => u.Email);
            ViewBag.Status = status;
            if (grouped == null)
                grouped = new Dictionary<int, List<FormAnswer>>();


            var allRatings = _db.ProfileRatings.ToList();

            var myRatings = allRatings.Where(r => r.RatedBy == adminId)
                                      .ToDictionary(r => r.UserId, r => (int?)r.Score);

            var avgRatings = allRatings.GroupBy(r => r.UserId)
                                       .ToDictionary(g => g.Key, g => g.Average(r => r.Score));

            ViewBag.Users = users.ToDictionary(u => u.Id, u => u.Email);
            ViewBag.Status = status;
            ViewBag.MyRatings = myRatings;   // Dictionary<string, int?>
            ViewBag.AvgRatings = avgRatings; // Dictionary<string, double>
            ViewBag.Profiles = profiles.ToDictionary(u => u.Id, u => u.ProfileName );


            return View(grouped);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult RateProfile(string userId, int questionId, int score, int profileId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var existing = _db.ProfileRatings
            //    .FirstOrDefault(r => r.UserId == userId && r.RatedBy == adminId);

            //if (existing != null)
            //{
            //    existing.Score = score;
            //    existing.RatedAt = DateTime.UtcNow;
            //}
            //else
            //{
            //    _db.ProfileRatings.Add(new ProfileRating
            //    {
            //        UserId = userId,
            //        Score = score,
            //        RatedBy = adminId,
            //        RatedAt = DateTime.UtcNow
            //    });
            //}
            //_db.SaveChanges();

            //var newAvg = _db.ProfileRatings.Where(r => r.UserId == userId).Average(r => r.Score);

            var existing = _db.QuestionRatings.FirstOrDefault(r => r.UserId == userId && r.QuestionId == questionId && r.RatedBy == adminId && r.ProfileId== profileId);

            if (existing != null)
            {
                existing.Score = score;
                existing.RatedAt = DateTime.UtcNow;
                existing.ProfileId = profileId;
                _db.Update(existing);
            }
            else
            {
                _db.QuestionRatings.Add(new QuestionRating
                {
                    UserId = userId,
                    QuestionId = questionId,
                    Score = score,
                    RatedBy = adminId,
                    RatedAt = DateTime.UtcNow,
                    ProfileId=profileId
                });
            }

            _db.SaveChanges();

            // (Optional) return other info like avg for question:
            var avg = _db.QuestionRatings
                .Where(r => r.QuestionId == questionId && r.RatedBy != adminId)
                .Select(r => (int?)r.Score)
                .Average();


            return Json(new { success = true, score = score, average = avg });
        }



        public async Task<IActionResult> LoadRatings(string userId, int profileId)
        {
            //    var ratings = _db.QuestionRatings
            //.Where(r => r.UserId == userId)
            //.OrderByDescending(r => r.RatedAt)
            //.ToList();




            //    var rategroup = ratings.GroupBy(m => m.RatedBy).ToList();

            //    foreach (var rates in rategroup)
            //    {
            //        int average = (int)rates.Average(m => m.Score);
            //        foreach (var rate in rates)
            //        {
            //            var ratebyUser = _um.FindByIdAsync(rate.RatedBy).Result;
            //            rate.RatingByName = ratebyUser.UserName;
            //            rate.AvgScore = average;
            //            rate.UserName = User.Identity != null ? User.Identity.Name : "";
            //        }
            //    }
            //    var rategroup2 = ratings.GroupBy(m => m.QuestionId).ToList();
            //    foreach (var rates in rategroup2)
            //    {
            //        var question = _db.FormQuestions.Where(m => m.Id == rates.Key).FirstOrDefault();
            //        foreach (var rate in rates)
            //        {
            //            rate.Question = question;
            //        }
            //    }

            // Dynamic Pivot 

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetRatingsByUserId";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@profileId", profileId));
                await _db.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<Dictionary<string, object>>();

                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }

                        results.Add(row);
                    }
                    return PartialView("_RatingsPartial", results);
                    //return results;
                }
            }


            // 2. Return the partial view, passing the model
           // return PartialView("_RatingsPartial", ratings);
        }

    }
}
