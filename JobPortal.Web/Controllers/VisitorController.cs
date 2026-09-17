using JobPortal.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using JobPortal.Web.Models;
using Newtonsoft.Json;
using System.Text;
using JobPortal.Web.ViewModels;
using System.Data;
using JobPortal.Domain.Models;
using System.Security.Claims;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using JobPortal.Infrastructure.Migrations;

namespace JobPortal.Web.Controllers
{
    public class VisitorController : Controller
    {
        private readonly IServey _servey;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILibrary _library;
        private readonly ILibraryCategory _libraryCategory;
        private readonly IDForms _dForms;
        private readonly IOneMinute _IOneMinute;
        private readonly IWpPostModel _wpPostModel;
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _um;
        private readonly IFileStorage _files;
        private readonly SignInManager<ApplicationUser> _sm;
        private readonly ITimePassCategory _timePassCategory;
        private readonly IMyform _myform;
        private readonly IFormFeeRepository _fee;
        public VisitorController(AppDbContext db, IFileStorage files, UserManager<ApplicationUser> um, IServey servey, IDForms dForms, IWebHostEnvironment webHostEnvironment, ILibrary library,
            ILibraryCategory libraryCategory, IOneMinute IOneMinute, IWpPostModel wpPostModel, SignInManager<ApplicationUser> sm, ITimePassCategory timePassCategory, IMyform myform, IFormFeeRepository fee)
        {
            _servey = servey;
            _webHostEnvironment = webHostEnvironment;
            _library = library;
            _libraryCategory = libraryCategory;
            _dForms = dForms;
            _IOneMinute = IOneMinute;
            _wpPostModel = wpPostModel;
            _sm = sm;
            _db = db;
            _um = um;
            _files = files;
            _timePassCategory = timePassCategory;
            _myform = myform;
            _fee = fee;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Poll(int? id, int? page, int? qid)
        {
            try
            {
                List<QuestionforServey> lst = new List<QuestionforServey>();
                lst = _servey.GetAllQuestionListForSurvey(id.HasValue ? id.Value : (qid.HasValue == false ? 1 : 0), page.HasValue ? page.Value : 1, qid.HasValue ? qid.Value : 0);
                ViewBag.Header = _servey.GetServeyHeaderById(id.HasValue ? id.Value : 0);

                if (lst != null && lst.Count > 0)
                {
                    foreach (var itm in lst)
                    {
                        itm.AnswerList = _servey.GetAllAnswerByQuestionId(itm.PKID);
                    }
                }
                return View(lst);
            }
            catch
            {
                return RedirectToAction("index");

            }
        }

        [HttpPost]
        public IActionResult Vote(int questionId, int answerId)
        {

            try
            {

                var pollresult = _servey.PollAddorUpdate(questionId, answerId);
                return Json(new { msg = "success", result = pollresult });
            }
            catch (Exception ex)
            {
                return Json(new { msg = "error" });

            }

        }

        public IActionResult LibraryList(int? id)
        {
            int categoryId = 0;
            if (id.HasValue && id.Value > 0)
            {
                categoryId = id.Value;
            }
            var itmsCat = _library.GetAllLibraryAndCategory(categoryId);

            ViewBag.Parentcats = _libraryCategory.GetAllParentCategoryByChildId(categoryId);

            return View(itmsCat);


        }

        public IActionResult Library(int id)
        {

            var itms = _library.GetById(id);
            if (itms != null)
                ViewBag.Parentcats = _libraryCategory.GetAllParentCategoryByChildId(itms.CategoryId);
            else
                ViewBag.Parentcats = new List<LibraryCategoryModel>();
            ViewBag.libCat = _libraryCategory.GetById(itms.CategoryId);
            return View(itms);

        }


        public IActionResult Forms(int? id, int? stepId)
        {
            if (HttpContext.Session.GetString("SessionId") is null)
            {
                HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());
            }
            string guid = HttpContext.Session.GetString("SessionId");
            if (guid == null)
            {
                guid = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("SessionId", guid);
            }
            int userId = HttpContext.Session.GetInt32("UserId") == null ? 0 : HttpContext.Session.GetInt32("UserId").Value;
            ViewBag.UserId = userId;
            ViewBag.RecordedAnser = _dForms.GetAnswerRecordBySessionId(id.Value, guid);
            int sheetId = id.HasValue == true ? id.Value : 1;
            ViewBag.StepId = stepId.HasValue ? stepId.Value : 0;


            DynamicForm model = new DynamicForm();
            ViewBag.Url = Request.Host.Value; //.GetDisplayUrl();

            if (id.HasValue && id.Value > 0)
            {
                model = _dForms.GetFormById(id.Value);
                try
                {
                    if (!string.IsNullOrEmpty(model.CommonCssinStr))
                    {
                        CommonCSS commonCss = JsonConvert.DeserializeObject<CommonCSS>(model.CommonCssinStr);

                        model.CommonCSS = commonCss;
                    }
                    else
                    {
                        model.CommonCSS = new CommonCSS();
                    }
                }
                catch
                {


                }



                var obj = _dForms.GetAllSectionByFormId(id.Value, false);
                if (obj != null)
                {
                    foreach (var itm in obj)
                    {
                        itm.Questions = _dForms.GetAllQuestionBySectionId(itm.PKID, false);

                        foreach (var qst in itm.Questions)
                        {
                            qst.Answers = _dForms.GetAllAnswerByQuestionId(qst.PKID, false);
                        }
                    }
                    model.Sections = obj;
                }
                else
                {
                    model.Sections = new List<DynamicFormSection>();
                }
            }
            return View(model);


        }

        [HttpPost]
        public IActionResult Forms(DynamicForm form, int? id, int? stepId, IFormCollection coll)
        {
            string guid = Guid.NewGuid().ToString();
            if (HttpContext.Session.GetString("SessionId") is not null)
            {
                guid = HttpContext.Session.GetString("SessionId");
            }
            int userId = HttpContext.Session.GetInt32("UserId") == null ? 0 : HttpContext.Session.GetInt32("UserId").Value;
            ViewBag.UserId = userId;

            string sectionIds = coll["formsection.PKID"];
            string questionIds = coll["question.PKID"];
            string questiontypes = coll["question.QuestionType"];
            int[] iQuesids = Array.ConvertAll(questionIds.Split(','), int.Parse);
            int[] iQuesTypes = Array.ConvertAll(questiontypes.Split(','), int.Parse);
            int iCounter = 0;
            int questionType = 0;
            string answers = "";


            foreach (var sectionId in sectionIds.Split(','))
            {
                if (string.IsNullOrEmpty(sectionId))
                    continue;

                var questions = _dForms.GetAllQuestionBySectionId(int.Parse(sectionId), false);
                StringBuilder st = new StringBuilder();
                string questionId = "";

                foreach (var ques in questions)
                {
                    answers = "";
                    questionId = ques.PKID.ToString();
                    questionType = ques.QuestionType;

                    if (questionType == 1) //Single Choice - Radio Button
                    {
                        answers = coll[questionId];
                        if (answers != null)
                            _dForms.RecordAnswer(int.Parse(sectionId), ques.PKID, int.Parse(answers), answers, answers, guid.ToString(), userId);
                    }
                    else if (questionType == 2) //Multiple Choice - Checkbox
                    {
                        answers = coll[questionId];
                        if (answers != null)
                        {
                            foreach (var st1 in answers.Split(","))
                            {
                                _dForms.RecordAnswer(int.Parse(sectionId), ques.PKID, int.Parse(st1), st1, st1, guid.ToString(), userId);
                            }
                        }
                    }
                    else if (questionType == 3) //Range Slider
                    {
                        answers = coll[questionId];
                        if (answers != null)
                        {

                            _dForms.RecordAnswer(int.Parse(sectionId), ques.PKID, ques.PKID, answers, answers, guid.ToString(), userId);

                        }
                    }
                    else if (questionType == 4) //Text Answer
                    {
                        answers = coll[questionId];
                        if (answers != null)
                        {

                            _dForms.RecordAnswer(int.Parse(sectionId), ques.PKID, ques.PKID, answers, answers, guid.ToString(), 0);

                        }
                    }
                    else if (questionType == 5) //Upload (Image/Doc/PDF)
                    {

                        string userfile = "";
                        if (answers != null)
                        {
                            var allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };

                            var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/visitor/");
                            var iuserFile = coll.Files[questionId];


                            if (iuserFile != null && iuserFile.Length > 0)
                            {
                                var ext = Path.GetExtension(iuserFile.FileName).ToLower();
                                userfile = Guid.NewGuid().ToString() + ext;
                                if (allowedExtensions.Contains(ext)) //check what type of extension  
                                {
                                    var filePath = Path.Combine(paperBasePath, userfile);
                                    // Save the file
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        iuserFile.CopyTo(stream);
                                    }



                                }


                            }

                            _dForms.RecordAnswer(int.Parse(sectionId), ques.PKID, ques.PKID, userfile, userfile, guid.ToString(), userId);

                        }

                    }
                    iCounter++;

                }
            }
            ViewBag.RecordedAnser = _dForms.GetAnswerRecordBySessionId(id.Value, guid.ToString());
            DynamicForm model = new DynamicForm();
            ViewBag.Url = Request.Host.Value; //.GetDisplayUrl();

            if (id.HasValue && id.Value > 0)
            {
                model = _dForms.GetFormById(id.Value);

                try
                {
                    if (!string.IsNullOrEmpty(model.CommonCssinStr))
                    {
                        CommonCSS commonCss = JsonConvert.DeserializeObject<CommonCSS>(model.CommonCssinStr);

                        model.CommonCSS = commonCss;
                    }
                    else
                    {
                        model.CommonCSS = new CommonCSS();
                    }
                }
                catch
                {


                }


                var obj = _dForms.GetAllSectionByFormId(id.Value, false);
                if (obj != null)
                {
                    foreach (var itm in obj)
                    {
                        itm.Questions = _dForms.GetAllQuestionBySectionId(itm.PKID, false);

                        foreach (var qst in itm.Questions)
                        {
                            qst.Answers = _dForms.GetAllAnswerByQuestionId(qst.PKID, false);
                        }
                    }
                    model.Sections = obj;
                }
                else
                {
                    model.Sections = new List<DynamicFormSection>();
                }
            }

            var val = coll["hidBtnclickfrom"];

            return RedirectToAction("Entries", "Visitor", new { id = model.PKID, blockId = guid.ToString().ToLower() });

        }


        public IActionResult OneMinuteList()
        {

            var lst = _IOneMinute.GetAllOneMinuteItem(2);
            return View(lst);

        }

        public IActionResult OneMinute(int? id)
        {
            ViewBag.Msg = "";


            if (id.HasValue)
            {
                var itm = _IOneMinute.GetOneMinuteItemById(id.Value);

                return View(itm);
            }
            else
            {
                return RedirectToAction("index");
            }

        }


        public IActionResult DygmPointList()
        {
            var lst = _IOneMinute.GetAllOneMinuteItem(3);
            return View(lst);
        }

        public IActionResult MyPoint(int? id)
        {

            ViewBag.Msg = "";


            if (id.HasValue)
            {
                var itm = _IOneMinute.GetOneMinuteItemById(id.Value);

                return View(itm);
            }
            else
            {
                return RedirectToAction("index");
            }

        }


        public IActionResult Timepassdetails()
        {

            return View();

        }
        public IActionResult TimePassList()
        {
            var itms = _IOneMinute.GetAllOneMinuteItem();
            return View(itms);

        }

        public IActionResult TimePass(int? id)
        {
            ViewBag.Msg = "";


            if (id.HasValue)
            {
                var itm = _IOneMinute.GetOneMinuteItemById(id.Value);

                return View(itm);
            }
            else
            {
                return RedirectToAction("index");
            }

        }

        public IActionResult TrendingNewsList(int? id)
        {
            ViewBag.Category = _wpPostModel.SelectCategoryById(id.HasValue ? id.Value : 0);
            var lst = _wpPostModel.GetAllPosts(id.HasValue ? id.Value : 0);
            return View(lst);
        }
        public IActionResult TrendingNews(int id)
        {
            ViewBag.Msg = "";
            try
            {

                var post = _wpPostModel.SelectById(id);
                if (post == null)
                    return RedirectToAction("index");
                ViewBag.Category = _wpPostModel.SelectCategoryById(post.CategoryId);
                return View(post);
            }
            catch
            {
                return RedirectToAction("index");
            }
        }
        public async Task<IActionResult> OpenSurvey(int id, int? stepId, int? questionid)
        {
            ViewBag.Msg = "";
            ViewBag.NeedToPay = 0;
            try
            {
                if (HttpContext.Session.GetString("SessionId") is null)
                {
                    HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());
                }
                string sessionId = HttpContext.Session.GetString("SessionId");
                ViewBag.ViewOnly =  questionid.HasValue ;
                bool isAuthenticated = User.Identity.IsAuthenticated;
                ViewBag.IsAuthenticated = isAuthenticated;
                string userId = "";
                if (isAuthenticated)
                    userId = _um.GetUserId(User);
                else
                    userId = Guid.Empty.ToString();
                    var frmCategory = _db.FormTypeCategory.Where(m => m.Id == id).FirstOrDefault();
                ViewBag.FormCategory = frmCategory;
                int roleId = 0;
                if (User.IsInRole("Student/JobSeeker"))
                {
                    roleId = 1;
                }
                else if (User.IsInRole("Employer"))
                {
                    roleId = 2;
                }
                else if (User.IsInRole("Academics"))
                {
                    roleId = 3;
                }
                else if (User.IsInRole("Intern"))
                {
                    roleId = 4;
                }
                else if (User.IsInRole("Mid-Career"))
                {
                    roleId = 5;
                }
                else if (User.IsInRole("Silver-Talent"))
                {
                    roleId = 6;
                }
                else
                {
                    roleId = 1;
                }
                if (frmCategory != null)
                {
                    if (frmCategory.ForRole == 1)
                    {
                        ViewBag.InstitureName = "";
                    }
                    else if (frmCategory.ForRole == 2)
                    {
                        ViewBag.InstitureName = "Company Name *";
                    }
                    else if (frmCategory.ForRole == 3)
                    {
                        ViewBag.InstitureName = "Academic Institution *";
                    }
                    else
                    {
                        ViewBag.InstitureName = "";
                    }
                }
               //ViewBag.FormFee =   _fee.GetUserFormFeeDetails(roleId, id, Guid.Parse(userId));
               //ViewBag.NeedToPay = _fee.CheckforFeeSession(id, Guid.Parse(userId), sessionId);
                var formFeeDetails = _fee.GetUserFormFeeDetails(roleId, id, Guid.Parse(userId));
                int IsNeedTopay = _fee.CheckforFeeSession(id, Guid.Parse(userId), sessionId);
                ViewBag.FormFee = formFeeDetails;
                ViewBag.NeedToPay = IsNeedTopay;
                ViewBag.AmountToPay = IsNeedTopay == 0 ? formFeeDetails.FTFeeAmount : formFeeDetails.FeeAmount;
                var sections = await _db.FormSections
                    .Where(s => s.IsActive && s.FormTypeCategoryId == id)
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Options)
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var answers = new List<FormAnswer>();

                if (stepId.HasValue)
                {
                    answers = await _db.FormAnswers
                   .Where(a => a.FormSessionEntryId == stepId.Value)
                   .ToListAsync();
                }
                else  if (isAuthenticated && !stepId.HasValue)
                {
                    answers = await _db.FormAnswers
                   .Where(a => a.UserId == userId)
                   .ToListAsync();
                }
               




                var vm = new FormVmVisitor
                {
                    FormCategoryId = id,
                    Sections = sections,
                    AnswersText = answers.Where(a => a.AnswerText != null &&  (stepId.HasValue== false || a.FormSessionEntryId==stepId.Value)).ToDictionary(a => a.QuestionId, a => a.AnswerText!),
                    AnswersFile = answers.Where(a => a.FilePath != null).ToDictionary(a => a.QuestionId, a => a.FilePath!),
                    PostDate = DateOnly.FromDateTime(DateTime.Now)
                };

                return View(vm);
            }
            catch
            {
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> OpenSurvey(FormVmVisitor model, Dictionary<int, string> answers, List<IFormFile> files, string actionType, int formCategoryId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = "JobSeeker";
            var FormCategory = _db.FormTypeCategory.Where(m => m.Id == formCategoryId).FirstOrDefault();
            if(FormCategory !=null)
            {
                  role = FormCategory.ForRole  == 1 ? "JobSeeker" : (FormCategory.ForRole == 2 ? "Employer" : (FormCategory.ForRole == 3 ? "Academics" : "Normal"));
            }

            var userId = Guid.Empty.ToString();

            
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            if (!ViewBag.IsAuthenticated)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.UserName };
                var r = await _um.CreateAsync(user, model.Password);
                string error = "";
                if (!r.Succeeded)
                {

                    foreach (var e in r.Errors)
                        ModelState.AddModelError("", e.Description);

                    return View();
                }

                await _um.AddClaimAsync(user, new Claim("AccountType", role));
                if ((await _um.GetRolesAsync(user)).Count > 0)
                {
                    await _um.RemoveFromRoleAsync(user, role);
                }
                await _um.AddToRoleAsync(user, role);


                await _sm.SignInAsync(user, false);
                userId = user.Id.ToString();
            }
            else
            {

                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                userId = currentUserName;


            }
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
            {
               
                HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);
            }
            string sessionId = HttpContext.Session.GetString("SessionId");
            int formSessionId = 0;
            if (!string.IsNullOrEmpty(sessionId))
            {
                formSessionId= _myform.GetFormSessionId(sessionId, formCategoryId, Guid.Parse(userId));
            }

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
                     
                    existing.FormSessionEntryId = formSessionId;
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
                        FormSessionEntryId = formSessionId
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


            TempData["Message"] = "Form submitted successfully!";

            await _db.SaveChangesAsync();
            return RedirectToAction("OpenSurvey", new { id = formCategoryId });
        }

        public IActionResult Havingsaidthatlist()
        {
            var itms = _IOneMinute.GetAllOneMinuteItem(5);
            return View(itms);

        }

        public IActionResult Havingsaidthat(int? id)
        {
            ViewBag.Msg = "";


            if (id.HasValue)
            {
                var itm = _IOneMinute.GetOneMinuteItemById(id.Value);

                return View(itm);
            }
            else
            {
                return RedirectToAction("index");
            }

        }

        public IActionResult categorylist(int? id)
        {
            int catId = 6;
            if (id.HasValue)
            {
                catId = id.Value;
            }
            ViewBag.Category =  _timePassCategory.GetById(catId);
            var lst = _timePassCategory.GetAllByParentId(catId);
            // var itms = _IOneMinute.GetAllOneMinuteItem(5);
            return View(lst);

        }

        [HttpGet]
        public JsonResult getItemsById(int id)
        {
           
            var itms = _IOneMinute.GetAllOneMinuteItem(id).OrderBy(m=>m.Title);
            var data = new
            {
                Status = true,
                Message = "Success",
                Items = itms
            };

            return Json(data);

        }

        public IActionResult linedetails(int? id)
        {

            ViewBag.Msg = "";


            if (id.HasValue)
            {
                var itm = _IOneMinute.GetOneMinuteItemById(id.Value);
                ViewBag.Category = _timePassCategory.GetById(itm.LanguageId);
               

                return View(itm);
            }
            else
            {
                return RedirectToAction("index");
            }

        }
    }
}
