using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobPortal.Web.ViewModels;
using JobPortal.Domain.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;

namespace JobPortal.Web.Areas.JobSeeker.Controllers;
[Area("JobSeeker")]
//[Authorize(Roles = "JobSeeker")]
public class ProfileController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _um;
    private readonly IWebHostEnvironment _env;
    private readonly IFileStorage _files;
    private readonly ILogger<ProfileController> _logger;
    public ProfileController(ILogger<ProfileController> logger,AppDbContext db, IFileStorage files, UserManager<ApplicationUser> um, IWebHostEnvironment env)
    {
        _db = db; _um = um; _env = env; _files = files;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var userId = Guid.Parse(_um.GetUserId(User));
        var profile = _db.JobSeekerProfiles.Where(p => p.UserId == userId).ToList();
        return View(profile);
    }

    public async Task<IActionResult> Steps(int id)
    {
        //here id is profile id to edit
        var userId = _um.GetUserId(User);
        
        var sections = await _db.FormSections
            .Where(s => s.IsActive && s.FormTypeCategoryId == 1 && s.Title !="Profile")
            .Include(s => s.Questions)
               .ThenInclude(s => s.QuestionRatings.Where(r => r.UserId == userId && r.RatedBy == userId))
               .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();

        var answers = await _db.FormAnswers
            .Where(a => a.UserId == userId && a.ProfileId ==id)
            .ToListAsync();

        var ratings = _db.QuestionRatings
       .Where(r => r.UserId == userId)
       .OrderByDescending(r => r.RatedAt)
       .ToList();

        foreach (var rate in ratings)
        {
            rate.UserName = User.Identity != null ? User.Identity.Name : "";
        }

        var rategroup = ratings.GroupBy(m => m.RatedBy).ToList();

        foreach (var rates in rategroup)
        {
            int average = (int)rates.Average(m => m.Score);
            foreach (var rate in rates)
            {
                var ratebyUser = _um.FindByIdAsync(rate.RatedBy).Result;
                rate.RatingByName = ratebyUser.UserName;
                rate.AvgScore = average;
            }
        }

        var results = new List<Dictionary<string, object>>();
        try
        {
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetRatingsByUserId";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId));

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

        //var vm = new FormVm()
        //{
        //    Sections = sections,
        //    AnswersText = answers.Where(a => a.AnswerText != null).ToDictionary(a => a.QuestionId, a => a.AnswerText!),
        //    AnswersFile = answers.Where(a => a.FilePath != null).ToDictionary(a => a.QuestionId, a => a.FilePath!),
        //    Ratings = ratings,
        //    AvgRating = ratings.Average(m => m.Score),
        //    NewRatings = results
        //};
        var vm = new FormVm();

        vm.Sections = sections;
        vm.AnswersText = answers.Where(a => a.AnswerText != null).ToDictionary(a => a.QuestionId, a => a.AnswerText!);
        vm.AnswersFile = answers.Where(a => a.FilePath != null).ToDictionary(a => a.QuestionId, a => a.FilePath!);
        vm.Ratings = ratings;
        vm.AvgRating = 0;
        vm.NewRatings = results;
        vm.ProfileId = id;


        return View(vm);
    }


    [HttpPost]
    public async Task<IActionResult> SaveForm(Dictionary<int, string> answers, List<IFormFile> files, string actionType, int ProfileId)
    {
        try
        {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        bool isDraft = actionType == "draft";
        foreach (var ans in answers)
        {
            var existing = await _db.FormAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QuestionId == ans.Key && a.ProfileId == ProfileId);

            if (existing != null)
            {
                existing.AnswerText = ans.Value;
                existing.IsDraft = isDraft;
                existing.SubmittedAt = DateTime.UtcNow;
                existing.ProfileId = ProfileId;
            }
            else
            {
                _db.FormAnswers.Add(new FormAnswer
                {
                    UserId = userId,
                    QuestionId = ans.Key,
                    AnswerText = ans.Value,
                    SubmittedAt = DateTime.UtcNow,
                    IsDraft = isDraft,
                    ProfileId = ProfileId
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
        return RedirectToAction("steps", new {id = ProfileId });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving form");
            return RedirectToAction("steps", new { id = ProfileId });
        }
    }

    [HttpPost("Save")]
    [RequestSizeLimit(104857600)]
    public async Task<IActionResult> Save()
    {
        var form = Request.Form;
        var files = Request.Form.Files;
        var userId = _um.GetUserId(User);

        var questions = await _db.FormQuestions
            .Include(q => q.Options)
            .ToListAsync();

        // Existing answers for update
        var existing = await _db.FormAnswers.Where(a => a.UserId == userId).ToListAsync();

        // Handle text/single/multiple
        foreach (var key in form.Keys.Where(k => k.StartsWith("Answers[")))
        {
            // key format: Answers[QUESTION_ID]
            var qId = int.Parse(key.Substring(8, key.Length - 9));
            var q = questions.FirstOrDefault(x => x.Id == qId);
            if (q == null) continue;

            var values = form[key]; // StringValues (can be multi for checkboxes)
            string? answerText = null;

            if (q.QuestionType == QuestionType.MultipleChoice)
            {
                // Join selected options by |
                answerText = string.Join("|", values.ToArray());
            }
            else
            {
                answerText = values.FirstOrDefault();
            }

            var existingAns = existing.FirstOrDefault(a => a.QuestionId == qId);
            if (existingAns == null)
            {
                _db.FormAnswers.Add(new FormAnswer
                {
                    UserId = userId!,
                    QuestionId = qId,
                    AnswerText = answerText
                });
            }
            else
            {
                existingAns.AnswerText = answerText;
            }
        }

        // Handle file uploads (name = Files[QUESTION_ID])
        foreach (var file in files)
        {
            if (!file.Name.StartsWith("Files[")) continue;
            var inside = file.Name.Substring(6, file.Name.Length - 7);
            if (!int.TryParse(inside, out var qId)) continue;

            var q = questions.FirstOrDefault(x => x.Id == qId);
            if (q == null || q.QuestionType != QuestionType.FileUpload) continue;

            var relPath = await _files.SaveAsync(file, $"uploads/profile/{userId}");

            var existingFileAns = existing.FirstOrDefault(a => a.QuestionId == qId);
            if (existingFileAns == null)
            {
                _db.FormAnswers.Add(new FormAnswer
                {
                    UserId = userId!,
                    QuestionId = qId,
                    FilePath = relPath
                });
            }
            else
            {
                // optionally delete old
                if (!string.IsNullOrWhiteSpace(existingFileAns.FilePath))
                    _files.Delete(existingFileAns.FilePath);
                existingFileAns.FilePath = relPath;
            }
        }

        await _db.SaveChangesAsync();
        TempData["Saved"] = "Profile saved successfully.";
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {

       
        var userId = Guid.Parse(_um.GetUserId(User));

            FormattableString sql = $@"delete t from ( select   row_number() over( partition by QuestionId order by QuestionId) as rn from [ucards].[FormAnswers]  where userid='{userId}' and profileid={id} ) as t where rn>1";
            _db.Database.ExecuteSql(sql);

            var profile = _db.JobSeekerProfiles.FirstOrDefault(p => p.UserId == userId && p.Id == id) ?? new JobPortal.Domain.Models.JobSeekerProfile();
        var profilelist = _db.JobSeekerProfiles.Where(p => p.UserId == userId && p.Id != id).OrderBy(p => p.RcdInsTs).ToList();
        ViewBag.Profilelist = profilelist;
        if ((profilelist != null && profilelist.Count > 0) && profile.Id == 0)
        {
            profile.ProfileName = $"My Profile {profilelist.Count + 1}";
            profile.FullName = profilelist[0].FullName;
            profile.Phone = profilelist[0].Phone;
            profile.Experience = profilelist[0].Experience;
            profile.CurrentStateId = profilelist[0].CurrentStateId;
            if (profilelist[0].CurrentStateId.HasValue)
            {
                ViewBag.CurrentCities = _db.Cities.Where(c => c.StateId == profilelist[0].CurrentStateId.Value).ToList();
            }
            profile.CurrentCityId = profilelist[0].CurrentCityId;
            profile.DesiredStateId = profilelist[0].DesiredStateId;
            if (profilelist[0].DesiredStateId.HasValue)
            {
                ViewBag.DesiredCities = _db.Cities.Where(c => c.StateId == profilelist[0].DesiredStateId.Value).ToList();
            }
            profile.DesiredCityId = profilelist[0].DesiredCityId;
        }

        ViewBag.States = _db.States.ToList();
        if (profile.CurrentStateId.HasValue)
        {
            ViewBag.CurrentCities = _db.Cities.Where(c => c.StateId == profile.CurrentStateId.Value).ToList();
        }
        else
        {
            if (ViewBag.CurrentCities == null)
                ViewBag.CurrentCities = new List<City>();
        }

        if (profile.DesiredStateId.HasValue)
        {
            ViewBag.DesiredCities = _db.Cities.Where(c => c.StateId == profile.DesiredStateId.Value).ToList();
        }
        else
        {
            if (ViewBag.DesiredCities == null)
                ViewBag.DesiredCities = new List<City>();
        }



        ViewBag.Sections = await _db.FormQuestions
           .Where(s => s.SectionId == 1)
              .Include(s => s.QuestionRatings.Where(r => r.UserId == userId.ToString() && r.RatedBy == userId.ToString()))
               .Include(q => q.Options)
           .OrderBy(s => s.DisplayOrder)
           .ToListAsync();

        var answers = await _db.FormAnswers
            .Where(a => a.UserId == userId.ToString() && a.ProfileId == id)
            .ToListAsync();

        ViewBag.Ratings = _db.QuestionRatings
       .Where(r => r.UserId == userId.ToString())
       .OrderByDescending(r => r.RatedAt)
       .ToList();

        ViewBag.AnswersText = answers.Where(a => a.AnswerText != null).ToDictionary(a => a.QuestionId, a => a.AnswerText!);
        ViewBag.AnswersFile = answers.Where(a => a.FilePath != null).ToDictionary(a => a.QuestionId, a => a.FilePath!);
            return View(profile);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.StackTrace);
            return View(new JobSeekerProfile());
        }
       
    }

    [HttpPost]
    public async Task<IActionResult> Edit(JobPortal.Domain.Models.JobSeekerProfile vm, IFormFile resume, Dictionary<int, string> answers, List<IFormFile> files)
    {
        var userId = Guid.Parse(_um.GetUserId(User));
        var profile = _db.JobSeekerProfiles.FirstOrDefault(p => p.UserId == userId && p.Id == vm.Id);
        //if (resume != null && resume.Length > 0)
        //{
        //    var filename = $"{Guid.NewGuid()}_{resume.FileName}";
        //    var filePath = Path.Combine(_env.WebRootPath, "resumes", filename);
        //    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        //    using var fs = new FileStream(filePath, FileMode.Create);
        //    await resume.CopyToAsync(fs);

        //}
        if (profile == null)
        {
            vm.UserId = userId;
            vm.RcdInsTs = DateTime.UtcNow;

            _db.JobSeekerProfiles.Add(vm);
        }
        else
        {
            profile.FullName = vm.FullName;
            profile.Phone = vm.Phone;

            profile.Experience = vm.Experience;

            profile.CurrentStateId = vm.CurrentStateId;
            profile.DesiredStateId = vm.DesiredStateId;
            profile.CurrentCityId = vm.CurrentCityId;
            profile.DesiredCityId = vm.DesiredCityId;
            profile.ProfileName = vm.ProfileName;
            profile.RcdUpdtTs = DateTime.UtcNow;
            _db.JobSeekerProfiles.Update(profile);
        }
        _db.SaveChanges();


        //Saving Additional fields


        foreach (var ans in answers)
        {
            var existing = await _db.FormAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId.ToString() && a.QuestionId == ans.Key && a.ProfileId == vm.Id);

            if (existing != null)
            {
                existing.AnswerText = ans.Value;
                existing.IsDraft = false;
                existing.SubmittedAt = DateTime.UtcNow;
                existing.ProfileId = vm.Id;
            }
            else
            {
                _db.FormAnswers.Add(new FormAnswer
                {
                    UserId = userId.ToString(),
                    QuestionId = ans.Key,
                    AnswerText = ans.Value,
                    SubmittedAt = DateTime.UtcNow,
                    IsDraft = false,
                    ProfileId = vm.Id
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
                    .FirstOrDefaultAsync(a => a.UserId == userId.ToString() && a.QuestionId == qId && a.ProfileId == vm.Id);

                if (existing != null)
                {
                    existing.FilePath = "/uploads/" + file.FileName;
                    existing.SubmittedAt = DateTime.UtcNow;
                    existing.IsDraft = false;
                }
                else
                {
                    _db.FormAnswers.Add(new FormAnswer
                    {
                        UserId = userId.ToString(),
                        QuestionId = qId,
                        FilePath = "/uploads/" + file.FileName,
                        SubmittedAt = DateTime.UtcNow,
                        IsDraft = false,
                        ProfileId = vm.Id
                    });
                }
            }
        }
        int iSections = _db.FormSections.Where(s => s.FormTypeCategoryId == 1).Count();
        if (iSections > 1)
        {
            return RedirectToAction("Steps", new { id=vm.Id});

        }
        else
        {
            return RedirectToAction("Index");
        }
    }



    [Authorize(Roles = "JobSeeker")]
    public IActionResult RateSelf()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = _db.ProfileRatings
            .FirstOrDefault(r => r.UserId == userId && r.RatedBy == userId);

        return View(existing ?? new ProfileRating { Score = 0 });
    }

    [HttpPost]
    [Authorize(Roles = "JobSeeker")]
    public IActionResult RateSelfAjax(int questionId, int score, int profileId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //var existing = _db.ProfileRatings
        //    .FirstOrDefault(r => r.UserId == userId && r.RatedBy == "Self");

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
        //        RatedBy = "Self",
        //        RatedAt = DateTime.UtcNow
        //    });
        //}

        //_db.SaveChanges();

        var existing = _db.QuestionRatings.FirstOrDefault(r => r.UserId == userId && r.QuestionId == questionId
        && r.RatedBy == userId && r.ProfileId == profileId);

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
                RatedBy = userId,
                RatedAt = DateTime.UtcNow,
                ProfileId = profileId

            });
        }

        _db.SaveChanges();

        // (Optional) return other info like avg for question:
        var avg = _db.QuestionRatings
            .Where(r => r.QuestionId == questionId && r.RatedBy != userId && r.ProfileId == profileId)
            .Select(r => (int?)r.Score)
            .Average();

        return Json(new { success = true, score = score, averageOther = avg, message = "Self-rating saved!" });
    }


}
