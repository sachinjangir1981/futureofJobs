using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Infrastructure.Services;
using JobPortal.Web.Models;
using JobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;

namespace JobPortal.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Admin")]
public class FormsController : Controller
{
    private readonly IMyform _myform;
    private readonly AppDbContext _db;
    private readonly ICompareServices _compareServices;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FormsController(AppDbContext db, ICompareServices compareServices,
        IWebHostEnvironment webHostEnvironment, IMyform myform)
    {
        _db = db;
        _compareServices = compareServices;
        _webHostEnvironment = webHostEnvironment;
        _myform = myform;
    }

    // ========== Sections ==========



    public IActionResult Index(int? id)
    {
        return RedirectToAction(nameof(Sections), new { id = id });

    }


    public async Task<IActionResult> Sections(int? id)
    {
        ViewBag.CategoryId = id ?? 1;
        var sections = await _db.FormSections.Where(m => m.FormTypeCategoryId == id)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
        return View(sections);
    }


    public IActionResult CreateSection(int? id)
    {
        return View(new FormSection { FormTypeCategoryId = id ?? 1 });
    }


    [HttpPost]
    public async Task<IActionResult> CreateSection(FormSection model)
    {
        model.FormTypeCategoryId = model.FormTypeCategoryId == 0 ? 1 : model.FormTypeCategoryId;
        model.Id = 0;
        if (!ModelState.IsValid) return View(model);
        _db.FormSections.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Sections), new { id = model.FormTypeCategoryId });
    }


    public async Task<IActionResult> EditSection(int id, int layout = 1)
    {

        ViewBag.Layout = layout;
        var section = await _db.FormSections.FindAsync(id);
        if (section == null) return NotFound();
        section.FormTypeCategoryId = section.FormTypeCategoryId == 0 ? 1 : section.FormTypeCategoryId;
        return View(section);
    }

    [HttpPost]
    public async Task<IActionResult> EditSection(int id, FormSection model, int layout = 1)
    {
        model.FormTypeCategoryId = model.FormTypeCategoryId == 0 ? 1 : model.FormTypeCategoryId;
        ViewBag.Layout = layout;
        if (!ModelState.IsValid) return View(model);
        var entity = await _db.FormSections.FindAsync(id);
        if (entity == null) return NotFound();

        entity.Title = model.Title;
        entity.DisplayOrder = model.DisplayOrder;
        entity.IsActive = model.IsActive;



        await _db.SaveChangesAsync();
        if (layout == 1)
            return RedirectToAction(nameof(Sections), new { id = model.FormTypeCategoryId });
        else
            return Json(new { success = true, title = entity.Title });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSection(int id, int layout = 1)
    {
        var entity = await _db.FormSections.FindAsync(id);
        if (entity == null) return NotFound();
        _db.FormSections.Remove(entity);
        await _db.SaveChangesAsync();

        if (layout == 1)
            return RedirectToAction(nameof(Sections));
        else
            return Json(new { success = true });
    }

    // ========== Questions ==========


    public async Task<IActionResult> Questions(int sectionId)
    {
        var section = await _db.FormSections.Include(s => s.Questions.OrderBy(q => q.DisplayOrder))
                                            .FirstOrDefaultAsync(s => s.Id == sectionId);
        if (section == null) return NotFound();
        return View(section);
    }

    [HttpGet]
    public IActionResult CreateQuestion(int sectionId, int layout = 1)
    {
        ViewBag.Layout = layout;
        var q = new FormQuestion { SectionId = sectionId, QuestionType = Domain.Models.QuestionType.Text };
        return View(q);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion(int sectionId, FormQuestion model, int layout = 1)
    {
        ViewBag.Layout = layout;
        if (!ModelState.IsValid) return View(model);
        _db.FormQuestions.Add(model);
        await _db.SaveChangesAsync();

        if (layout == 1)
            return RedirectToAction(nameof(Questions), new { sectionId });
        else
            return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> EditQuestion(int id, int layout = 1)
    {
        ViewBag.Layout = layout;
        var q = await _db.FormQuestions.FindAsync(id);
        if (q == null) return NotFound();
        return View(q);
    }

    [HttpPost]
    public async Task<IActionResult> EditQuestion(int id, FormQuestion model, int layout = 1)
    {
        ViewBag.Layout = layout;
        if (!ModelState.IsValid) return View(model);
        var q = await _db.FormQuestions.FindAsync(id);
        if (q == null) return NotFound();

        q.QuestionText = model.QuestionText;
        q.QuestionType = model.QuestionType;
        q.IsRequired = model.IsRequired;
        q.DisplayOrder = model.DisplayOrder;
        q.IsCompare = model.IsCompare;
        q.IsRating = model.IsRating;
        q.IsFilter = model.IsFilter;
        q.Weightage = model.Weightage;
        q.Marks = model.Marks;
        q.Explanation = model.Explanation;
        await _db.SaveChangesAsync();
        if (layout == 1)
            return RedirectToAction(nameof(Questions), new { sectionId = q.SectionId });
        else
            return Json(new { success = true });

    }

    [HttpPost]
    public async Task<IActionResult> DeleteQuestion(int id, int layout = 1)
    {
        var q = await _db.FormQuestions.FindAsync(id);
        if (q == null) return NotFound();
        var sectionId = q.SectionId;
        _db.FormQuestions.Remove(q);
        await _db.SaveChangesAsync();

        if (layout == 1)
            return RedirectToAction(nameof(Questions), new { sectionId });
        else
            return Json(new { success = true });
    }

    // ========== Options ==========

    [HttpGet("Options/{questionId}")]
    public async Task<IActionResult> Options(int questionId)
    {
        var q = await _db.FormQuestions
            .Include(x => x.Options.OrderBy(o => o.DisplayOrder))
            .FirstOrDefaultAsync(x => x.Id == questionId);
        if (q == null) return NotFound();
        return View(q);
    }

    [HttpGet]
    public IActionResult CreateOption(int questionId, int layout = 1)
    {
        ViewBag.Layout = layout;
        return View(new FormOption { QuestionId = questionId });
            }

    [HttpPost]
    public async Task<IActionResult> CreateOption(int questionId, FormOption model, int layout = 1)
    {
        ViewBag.Layout = layout;
        if (!ModelState.IsValid) return View(model);
        _db.FormOptions.Add(model);
        await _db.SaveChangesAsync();
       
        if (layout == 1)
            return RedirectToAction(nameof(Options), new { questionId });
        else
            return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> EditOption(int id, int layout = 1)
    {
        ViewBag.Layout = layout;
        var opt = await _db.FormOptions.FindAsync(id);
        if (opt == null) return NotFound();
        return View(opt);
    }

    [HttpPost]
    public async Task<IActionResult> EditOption(int id, FormOption model, int layout = 1)
    {
        ViewBag.Layout = layout;
        if (!ModelState.IsValid) return View(model);
        var opt = await _db.FormOptions.FindAsync(id);
        if (opt == null) return NotFound();

        opt.OptionText = model.OptionText;
        opt.DisplayOrder = model.DisplayOrder;
        opt.IsActive = model.IsActive;
        opt.Marks = model.Marks;
        opt.Weightage = model.Weightage;
        opt.Explanation = model.Explanation;
       
        await _db.SaveChangesAsync();
        if (layout == 1)
            return RedirectToAction(nameof(Options), new { questionId = opt.QuestionId });
        else
            return Json(new { success = true });

    }

    [HttpPost]
    public async Task<IActionResult> DeleteOption(int id, int layout = 1)
    {
        var opt = await _db.FormOptions.FindAsync(id);
        if (opt == null) return NotFound();
        var qid = opt.QuestionId;
        _db.FormOptions.Remove(opt);
        await _db.SaveChangesAsync();

        if (layout == 1)
            return RedirectToAction(nameof(Options), new { questionId = qid });
        else
            return Json(new { success = true });
    }


    public async Task<IActionResult> ManageForm(int? id)
    {

        var sections = await _db.FormSections
            .Where(s => s.IsActive && s.FormTypeCategoryId == id)
            .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();



        var vm = new FormVm
        {
            Sections = sections

        };
        return View(vm);
    }

    public IActionResult Categories()
    {
        var list = _db.FormTypeCategory.Where(m=>m.Id >5)
            .OrderBy(c => c.DisplayOrder)
            .ToList();
        return View(list);
    }

    public IActionResult CreateCategory()
    {
        ViewBag.AllRoles = _myform.GetAllRolesByFormId(0);
        return   View( new FormTypeCategory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateCategory(FormTypeCategory model, IFormFile fImageUrl, IFormFile fBgImageUrl, IFormCollection coll)
    {
         
            var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/forms/");
            string imagephoto = model.ImageUrl == null ? "" : model.ImageUrl;

            if (fImageUrl != null && fImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext))
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fImageUrl.CopyTo(stream);
                    }

                    model.ImageUrl = newimagename;
                }
            }
            else
            {
                model.ImageUrl = imagephoto;
            }



            string bgimagephoto = model.BgImageUrl == null ? "" : model.BgImageUrl;

            if (fBgImageUrl != null && fBgImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fBgImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext))
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fBgImageUrl.CopyTo(stream);
                    }

                    model.BgImageUrl = newimagename;
                }
            }
            else
            {
                model.BgImageUrl = bgimagephoto;
            }


            
            model.IsReadOnly = false;
            _db.FormTypeCategory.Add(model);
            _db.SaveChanges();


        string rolesIds = "";
        var AllRoles = _myform.GetAllRolesByFormId(model.Id);

        foreach (var sroleId in AllRoles)
        {
            var chkName = "chk_" + sroleId.RoleId.ToString();
            string isChecked = coll[chkName].ToString();
            if (isChecked == "on")
            {
                if (string.IsNullOrEmpty(rolesIds))
                {
                    rolesIds = sroleId.RoleId.ToString();
                }
                else
                {
                    rolesIds += "," + sroleId.RoleId.ToString();
                }
            }
        }

        _myform.UpdateFormRoles(model.Id, rolesIds);

        return RedirectToAction("Categories", "Forms");
        
    }

    public IActionResult EditCategory(int id)
    {
       
        var category = _db.FormTypeCategory.Find(id);
        if (category == null) return NotFound();
        ViewBag.AllRoles = _myform.GetAllRolesByFormId(id);
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditCategory(FormTypeCategory model, IFormFile fImageUrl, IFormFile fBgImageUrl, IFormCollection coll)
    {
        
            var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
            var coverphotoBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/forms/");
            string imagephoto = model.ImageUrl == null ? "" : model.ImageUrl;

            if (fImageUrl != null && fImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext))
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fImageUrl.CopyTo(stream);
                    }

                    model.ImageUrl = newimagename;
                }
            }
            else
            {
                model.ImageUrl = imagephoto;
            }



            string bgimagephoto = model.BgImageUrl == null ? "" : model.BgImageUrl;

            if (fBgImageUrl != null && fBgImageUrl.Length > 0)
            {
                var ext = Path.GetExtension(fBgImageUrl.FileName).ToLower();
                var newimagename = Guid.NewGuid().ToString() + ext;
                if (allowedExtensions.Contains(ext))
                {
                    var filePath = Path.Combine(coverphotoBasePath, newimagename);
                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fBgImageUrl.CopyTo(stream);
                    }

                    model.BgImageUrl = newimagename;
                }
            }
            else
            {
                model.BgImageUrl = bgimagephoto;
            }


            model.IsReadOnly = false;
            _db.Update(model);
            _db.SaveChanges();

        string rolesIds = "";
        var AllRoles = _myform.GetAllRolesByFormId(model.Id);

        foreach (var sroleId in AllRoles)
        {
            var chkName = "chk_" + sroleId.RoleId.ToString();
            string isChecked = coll[chkName].ToString();
            if (isChecked == "on")
            {
                if (string.IsNullOrEmpty(rolesIds))
                {
                    rolesIds = sroleId.RoleId.ToString();
                }
                else
                {
                    rolesIds += "," + sroleId.RoleId.ToString();
                }
            }
        }
        
            _myform.UpdateFormRoles(model.Id, rolesIds);
            return RedirectToAction("Categories","Forms");
        
         
    }

    public IActionResult CategoryDetails(int id)
    {
        var category = _db.FormTypeCategory.Find(id);
        if (category == null) return NotFound();
        return View(category);
    }
   
    public IActionResult CategoryDeleteConfirmed(int id)
    {
        var category = _db.FormTypeCategory.Find(id);
        if (category != null)
        {
            _db.FormTypeCategory.Remove(category);
            _db.SaveChanges();
        }
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Ratings(int id)
    {
       

      
       

        var results = new List<Dictionary<string, object>>();
        try
        {
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetRatingsByUserId_New";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@IsAdmin", 1));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", Guid.Empty));
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
            Sections = new List<FormSection>(),
            AnswersText = new Dictionary<int, string>(),
            AnswersFile = new Dictionary<int, string>(),
            Ratings = new List<QuestionRating>(),
            AvgRating = 0,
            NewRatings = results
        };

        return View(vm);
    }


    public async Task<IActionResult> Compare(int id)
    {
        var compareFormData = await _compareServices.CompareData(id, "","0");
        var userList  = await _compareServices.GetUserListByFormId(id);
       // userList.Insert(0, new UserListForCompare { UserId = "", UserName  = "Select User" });
        ViewBag.UserList = userList;
        return View(compareFormData);
    }

    [HttpPost]
    public async Task<IActionResult> GetDataByFormandUserId(int id, string userId)
    {
        try
        {
            var items = userId.Split('|');
            var compareFormData = await _compareServices.CompareData(id, items[0], items[1]);

            return Json(compareFormData);
        }
        catch (Exception ex)
        {

          
            return Json(ex);
        }
      
    }

    [HttpPost]
    public async Task<IActionResult> GetRatingDataByFormandUserId(int id, string userId)
    {

        try
        {
            var ratingScore = await _compareServices.GetRatingScore(id, userId);

            return Json(ratingScore);
        }
        catch (Exception ex)
        {


            return Json(ex);
        }
      
    }

    public async Task<IActionResult> ViewEntries(int id)
    {
        ViewBag.FormId = id;
        var userList = await _compareServices.GetUserListByFormId(id,isDataEntryOnly: true);
        return View(userList);
    }

 
    public async Task<IActionResult> ViewData(int id, string userId, string status="")
    {
        ViewBag.FormId = id;
     
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var formAnswers = _db.FormAnswers
            .Include(a => a.Question)
            .ThenInclude(q => q.Section)
             .Include(s => s.Question)
             .ThenInclude(s => s.QuestionRatings.Where(r => r.RatedBy == adminId))
            .ToList();

        // Group answers by user
        var grouped = formAnswers
            .Where(a => a.Question.Section.FormTypeCategoryId == id && a.UserId == userId)
            .GroupBy(a => a.UserId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Apply filter
        if (status == "draft")
            grouped = grouped.Where(x => x.Value.Any(a => a.IsDraft)).ToDictionary(x => x.Key, x => x.Value);
        else if (status == "final")
            grouped = grouped.Where(x => x.Value.Any(a => !a.IsDraft)).ToDictionary(x => x.Key, x => x.Value);

       
        ViewBag.Status = status;
        if (grouped == null)
            grouped = new Dictionary<string, List<FormAnswer>>();


        var allRatings = _db.ProfileRatings.ToList();

        var myRatings = allRatings.Where(r => r.RatedBy == adminId)
                                  .ToDictionary(r => r.UserId, r => (int?)r.Score);

        var avgRatings = allRatings.GroupBy(r => r.UserId)
                                   .ToDictionary(g => g.Key, g => g.Average(r => r.Score));

       
        ViewBag.Status = status;
        ViewBag.MyRatings = myRatings;   // Dictionary<string, int?>
        ViewBag.AvgRatings = avgRatings; // Dictionary<string, double>



        return View(grouped);
    }
}

