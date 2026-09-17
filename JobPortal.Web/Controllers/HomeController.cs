using Azure;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Models;
using JobPortal.Web.Models;
using JobPortal.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JobPortal.Web.Controllers;
public class HomeController : Controller
{

    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly ILibrary _library;
    private readonly ILibraryCategory _libraryCategory;
    private readonly IServey _servey;
    private readonly IContactForms _contactForms;
    private readonly ILeadershipReflection _leadershipReflection;
    private readonly UserManager<ApplicationUser> _um;
    private readonly SignInManager<ApplicationUser> _sm;
    private readonly IOneMinute _IOneMinute;
    private readonly ICuratedJobs _curatedJobs;
    private readonly ITimePassCategory _timePassCategory;
    private readonly IMyform _myform;
    private readonly IFormPaymentRepository _formPayment;
    private readonly IFormFeeRepository _fee;
    public HomeController(AppDbContext context, ILogger<HomeController> logger, ILibrary library, ILibraryCategory libraryCategory,
        IServey servey, IContactForms contactForms, ILeadershipReflection leadershipReflection, UserManager<ApplicationUser> um,
        SignInManager<ApplicationUser> sm, IOneMinute IOneMinute, ICuratedJobs curatedJobs,
        ITimePassCategory timePassCategory, IMyform myform, IFormPaymentRepository formPayment, IFormFeeRepository fee)
    {
        _sm = sm;
        _context = context;
        _logger = logger;
        _library = library;
        _libraryCategory = libraryCategory;
        _servey = servey;
        _contactForms = contactForms;
        _leadershipReflection = leadershipReflection;
        _um = um;
        _IOneMinute = IOneMinute;
        _curatedJobs = curatedJobs;
        _timePassCategory = timePassCategory;
        _myform = myform;
        _formPayment = formPayment;
        _fee = fee;
    }
    public IActionResult Index(string returnUrl = null)
    {

        //var categories = _context.Categories.Take(6).ToList();
        //var jobs = _context.Jobs
        //                   .OrderByDescending(j => j.PublishedAt)
        //                   .Take(5)
        //                   .ToList();
        //var locations = _context.Locations.Take(6).ToList();

        //var vm = new HomeViewModel
        //{
        //    Categories = categories,
        //    LatestJobs = jobs,
        //    Locations = locations
        //};

        return View();
    }

    public IActionResult aboutus()
    {
        return View();
    }

    public IActionResult FutureofWork()
    {
        return View();
    }

    public IActionResult FutureofWorkList()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(6);
        return View(itmsCat);
    }

    public IActionResult thebluetickdoctrine()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(12);
        return View(itmsCat);
    }

    public IActionResult hellogenz()
    {
        return View();
    }

    public IActionResult genzpolls()
    {

        var polllist = _servey.GetAllServeyListForGenZ();
        return View(polllist);
    }

    public IActionResult genzlibrary()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(5);
        return View(itmsCat);
    }



    public IActionResult fow_thesocietalarena()
    {
        return View();
    }

    public IActionResult fow_finalthought()
    {
        return View();
    }

    public IActionResult Green_Education_Skilling()
    {
        return View();
    }

    public IActionResult green_education_polls()
    {
        var items = _servey.GetAllServeyListForByText("Green Education -");
        return View(items);

    }

    public IActionResult GE_GreenDomain()
    {
        return View();
    }
    public IActionResult GE_SkillMatters()
    {
        return View();
    }

    public IActionResult GE_LearnAnywhere()
    {
        return View();
    }

    public IActionResult GE_Readiness()
    {
        return View();
    }

    public IActionResult Greenjob_Profiles_Career_Pathways()
    {
        return View();
    }

    public IActionResult gj_the_first_usual_first_job()
    {
        int catId = 6;
        ViewBag.Category = _timePassCategory.GetById(catId);
        var lst = _timePassCategory.GetAllByParentId(catId);
        return View(lst);
    }

    public IActionResult first_job_details(int? id)
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

    public IActionResult gj_high_level_jobs()
    {
        int catId = 32;
        ViewBag.Category = _timePassCategory.GetById(catId);
        var lst = _timePassCategory.GetAllByParentId(catId);
        return View(lst);
    }

    public IActionResult high_level_jobs_details(int? id)
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

    public IActionResult gj_green_career_pathways()
    {
        int catId = 39;
        ViewBag.Category = _timePassCategory.GetById(catId);
        var lst = _timePassCategory.GetAllByParentId(catId);
        return View(lst);
    }

    public IActionResult green_career_pathways_details(int? id)
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
    public IActionResult Greenjob_Search()
    {
        return View();
    }


    public IActionResult Greenjob_Curated_Job_Listing()
    {
        var itms = _curatedJobs.GetAll(false);
        return View(itms);
    }

    public IActionResult Greenjob_CV()
    {
        return View();
    }


    public IActionResult Greenjob_Intern()
    {
        return View();
    }

    public IActionResult The_Problem_with_todays_education()
    {
        return View();
    }

    public IActionResult The_revelation_no_one_talk_about()
    {
        return View();
    }

    public IActionResult The_true_internship_philosopgy()
    {
        return View();
    }

    public IActionResult The_six_stage_reality_of_modern_learning()
    {
        return View();
    }

    public IActionResult Why_internshipa_are_the_true_education()
    {
        return View();
    }

    public IActionResult The_hellokisan_internship_philosophy()
    {
        return View();
    }

    public IActionResult Begin_your_internship_journey()
    {
        return View();
    }

    public IActionResult Greenjob_Midcareer()
    {
        return View();
    }

    public IActionResult Mid_airplane_mode()
    {
        return View();
    }

    public IActionResult Mid_break_the_cage()
    {
        return View();
    }

    public IActionResult Mid_build()
    {
        return View();
    }

    public IActionResult Mid_fobs()
    {
        return View();
    }

    public IActionResult Mid_figuring_out()
    {
        return View();
    }

    public IActionResult Mid_talk_it_out()
    {
        return View();
    }

    public IActionResult SiverTalent()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(7);
        return View(itmsCat);
    }

    public IActionResult SiverTalent_BusinessStructure()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(7);
        return View(itmsCat);
    }

    public IActionResult Talent_hiring()
    {
        return View();
    }

    public IActionResult talent_scouting()
    {
        return View();
    }

    public IActionResult talent_screening()
    {
        return View();
    }

    public IActionResult talent_interview()
    {
        return View();
    }

    public IActionResult talent_onboarding()
    {
        return View();
    }

    public IActionResult talent_mission()
    {
        return View();
    }

    public IActionResult hrpolicy()
    {

        var itmsCat = _library.GetAllLibraryAndCategory(4);
        return View(itmsCat);
    }



    public IActionResult employability_index()
    {
        return View();
    }

    public IActionResult ei_whay_this_matters()
    {
        return View();
    }

    public IActionResult ei_what_we_are_doing()
    {
        return View();
    }

    public IActionResult ei_do_it()
    {
        return View();
    }

    public IActionResult ei_ai()
    {
        return View();
    }

    public IActionResult ei_ethical()
    {
        return View();
    }

    public IActionResult ei_takeaway()
    {
        return View();
    }

    public IActionResult welcome_inneroperatingsystem()
    {
        return View();
    }

    public IActionResult theparadoxofchoices()
    {
        return View();
    }





    public IActionResult employer()
    {
        return View();
    }

    public IActionResult employer_vibe_check()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(2);
        return View(itmsCat);

    }

    public IActionResult employer_compendium()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(3);
        return View(itmsCat);

    }

    public IActionResult candidate()
    {

        return View();

    }

    public IActionResult employerreg()
    {
        return View();
    }

    public IActionResult academicreg()
    {
        return View();
    }

    public IActionResult institution_dna()
    {
        return View();
    }


    public IActionResult leadership_reflections()
    {
        return View();
    }




    public async Task<IActionResult> leadership_reflections_dashboard(string id)
    {

        bool isAuthenticated = User.Identity.IsAuthenticated;
        ViewBag.IsAuthenticated = isAuthenticated;
        if (isAuthenticated && string.IsNullOrEmpty(id))
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            // ApplicationUser user = await _um.FindByNameAsync(currentUserName);

            var model = _leadershipReflection.GetByUserId(Guid.Parse(currentUserName), 1);
            if (model == null)
            {
                model = new LeadershipReflection();
                model.PostDate = DateOnly.FromDateTime(DateTime.Now);
            }
            return View(model);
        }
        if (!string.IsNullOrEmpty(id))
        {
            Guid leadership = Guid.Parse(id);
            var model = _leadershipReflection.GetByURId(leadership, 1);
            if (model == null)
            {
                model = new LeadershipReflection();
                model.PostDate = DateOnly.FromDateTime(DateTime.Now);
            }
            return View(model);
        }
        else
        {
            var mod = new LeadershipReflection();
            mod.PostDate = DateOnly.FromDateTime(DateTime.Now);
            return View(mod);
        }

    }

    [HttpPost]
    public async Task<IActionResult> leadership_reflections_dashboard(LeadershipReflection leadership)
    {

        string role = "Normal";
        ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

        if (!ViewBag.IsAuthenticated)
        {
            var user = new ApplicationUser { UserName = leadership.UserName, Email = leadership.UserName };
            var r = await _um.CreateAsync(user, leadership.Password);
            string error = "";
            if (!r.Succeeded)
            {

                foreach (var e in r.Errors)
                    ModelState.AddModelError("", e.Description);

                return View(leadership);
            }

            await _um.AddClaimAsync(user, new Claim("AccountType", role));
            if ((await _um.GetRolesAsync(user)).Count > 0)
            {
                await _um.RemoveFromRoleAsync(user, role);
            }
            await _um.AddToRoleAsync(user, role);


            await _sm.SignInAsync(user, false);
            leadership.UserId = user.Id;
        }
        else
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            leadership.UserId = Guid.Parse(currentUserName);

        }
        leadership.GridFormType = 1;
        Guid urid = _leadershipReflection.Add(leadership);
        TempData["Msg"] = "Your record has been successfully submitted";
        return RedirectToAction("leadership_reflections_dashboard", new { Id = urid.ToString().ToLower() });
    }

    #region "Academics Section"
    public IActionResult academics()
    {
        return View();
    }

    public IActionResult academics_the_sacred_backboneof_civilization()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(9);
        return View(itmsCat);
    }

    public IActionResult academics_reluctant_teacher_the_underbelly_of_academics()
    {
        var itmsCat = _library.GetAllLibraryAndCategory(10);
        return View(itmsCat);
    }

    public IActionResult academics_campus_has_stories()
    {
        var items = _servey.GetAllServeyListForByText("Academics -");
        return View(items);
    }

    #endregion


    public IActionResult contact()
    {

        ViewBag.Msg = "";

        return View();
    }

    [HttpPost]
    public IActionResult contact(ContactForm form)
    {
        int i = _contactForms.AddContactDetails(form);
        if (i > 0)
            ViewBag.Msg = "Form submitted successfully";
        else
            ViewBag.Msg = "There is some issue in submitting the form";
        return View();
    }


    public IActionResult test()
    {
        return View();
    }


    [HttpPost]
    [Route("home/exhibitors")]
    public IActionResult exhibitors(string hidid)
    {
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExhibitorDto>>(hidid);

        foreach (var item in data)
        {
            _library.ImportExhibitorData(item);
        }

        return Ok(new
        {
            success = true,
            //count = data.Count
        });
    }

    public IActionResult exhibitorlist(int? id)
    {
        int recType = id.HasValue ? id.Value : 1;
        ViewBag.RecType = recType;
        var itms = _library.SelectAllExhibitorData(recType);
        return View(itms);
    }

    public IActionResult exhibitorimport()
    {
        return View();
    }

    public IActionResult student_to_campus()
    {
        var itms = _IOneMinute.GetAllOneMinuteItem((int)CommoEnum.TimepassforStudents);
        return View(itms);
    }

    public IActionResult student_timepass(int? id)
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


    public IActionResult st_competency_delivery_framework(string id)
    {
        bool isAuthenticated = User.Identity.IsAuthenticated;
        ViewBag.IsAuthenticated = isAuthenticated;
        if (isAuthenticated && string.IsNullOrEmpty(id))
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            // ApplicationUser user = await _um.FindByNameAsync(currentUserName);

            var model = _leadershipReflection.GetByUserId(Guid.Parse(currentUserName), 2);
            if (model == null)
            {
                model = new LeadershipReflection();
                model.PostDate = DateOnly.FromDateTime(DateTime.Now);
            }
            return View(model);
        }
        if (!string.IsNullOrEmpty(id))
        {
            Guid leadership = Guid.Parse(id);
            var model = _leadershipReflection.GetByURId(leadership, 2);
            if (model == null)
            {
                model = new LeadershipReflection();
                model.PostDate = DateOnly.FromDateTime(DateTime.Now);
            }
            return View(model);
        }
        else
        {
            var mod = new LeadershipReflection();
            mod.PostDate = DateOnly.FromDateTime(DateTime.Now);
            return View(mod);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> st_competency_delivery_framework(LeadershipReflection leadership)
    {

        string role = "Normal";
        ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

        if (!ViewBag.IsAuthenticated)
        {
            var user = new ApplicationUser { UserName = leadership.UserName, Email = leadership.UserName };
            var r = await _um.CreateAsync(user, leadership.Password);
            string error = "";
            if (!r.Succeeded)
            {

                foreach (var e in r.Errors)
                    ModelState.AddModelError("", e.Description);

                return View(leadership);
            }

            await _um.AddClaimAsync(user, new Claim("AccountType", role));
            if ((await _um.GetRolesAsync(user)).Count > 0)
            {
                await _um.RemoveFromRoleAsync(user, role);
            }
            await _um.AddToRoleAsync(user, role);


            await _sm.SignInAsync(user, false);
            leadership.UserId = user.Id;
        }
        else
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var currentUserName = currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
            leadership.UserId = Guid.Parse(currentUserName);

        }
        leadership.GridFormType = 2;
        Guid urid = _leadershipReflection.Add(leadership);
        TempData["Msg"] = "Your record has been successfully submitted";
        return RedirectToAction("st_competency_delivery_framework", new { Id = urid.ToString().ToLower() });
    }


    public IActionResult myaccount()
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserName = User.Identity.Name;
            if (User.IsInRole("Student/JobSeeker"))
            {
                ViewBag.Role = 1;
            }
            else if (User.IsInRole("Employer"))
            {
                ViewBag.Role = 2;
            }
            else if (User.IsInRole("Academics"))
            {
                ViewBag.Role = 3;
            }
            else if (User.IsInRole("Intern"))
            {
                ViewBag.Role = 4;
            }
            else if (User.IsInRole("Mid-Career"))
            {
                ViewBag.Role = 5;
            }
            else if (User.IsInRole("Silver-Talent"))
            {
                ViewBag.Role = 6;
            }
            else
            {
                ViewBag.Role = 1;
            }
            // use userId here
            ViewBag.UserId = userId;
            var lst = _myform.GetAll(Guid.Parse(userId));

            ViewBag.UserId = userId;
            ViewBag.Balance = _formPayment.GetUserCurrentBalance(Guid.Parse(userId));
            ViewBag.Ledger = _formPayment.GetAllLedgerDetailByUserId(Guid.Parse(userId));

            return View(lst);
        }
        else
        {
            // user is not logged in
            return RedirectToAction("Login", "Account");
        }
    }


    [HttpGet]
    public JsonResult getVersionsByFormId(int id)
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // use userId here
            ViewBag.UserId = userId;
            var itms = _myform.GetAllFormVersionsByFormId(id, userId: Guid.Parse(userId));
            var data = new
            {
                Status = true,
                Message = "Success",
                Items = itms
            };
            return Json(data);
        }
        else
        {
            var data = new
            {
                Status = true,
                Message = "Error! Please login to load data",

            };
            // user is not logged in
            return Json(data);
        }




    }

    public IActionResult AddAmount()
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserName = User.Identity.Name;
            if (User.IsInRole("Student/JobSeeker"))
            {
                ViewBag.Role = 1;
            }
            else if (User.IsInRole("Employer"))
            {
                ViewBag.Role = 2;
            }
            else if (User.IsInRole("Academics"))
            {
                ViewBag.Role = 3;
            }
            else if (User.IsInRole("Intern"))
            {
                ViewBag.Role = 4;
            }
            else if (User.IsInRole("Mid-Career"))
            {
                ViewBag.Role = 5;
            }
            else if (User.IsInRole("Silver-Talent"))
            {
                ViewBag.Role = 6;
            }
            else
            {
                ViewBag.Role = 1;
            }
            // use userId here
            ViewBag.UserId = userId;
            ViewBag.Balance = _formPayment.GetUserCurrentBalance(Guid.Parse(userId));
            return View();
        }
        else
        {
            // user is not logged in
            return RedirectToAction("Login", "Account");
        }
    }

    public IActionResult MyLedger()
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserName = User.Identity.Name;
            
            // use userId here
            ViewBag.UserId = userId;
            ViewBag.Balance = _formPayment.GetUserCurrentBalance(Guid.Parse(userId));
            var lst = _formPayment.GetAllLedgerDetailByUserId(Guid.Parse(userId));



            return View(lst);
        }
        else
        {
            // user is not logged in
            return RedirectToAction("Login", "Account");
        }
    }

    [HttpPost]
    public IActionResult AddAmount(IFormCollection coll)
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int roleId = 1;
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
            UserLedger ledger = new UserLedger();
            ledger.UserId = Guid.Parse(userId);
            ledger.Credit = Convert.ToDecimal(coll["Amount"]);
            ledger.Debit = 0;
            ledger.Particular = "Added amount through my account section";
            ledger.RoleId = roleId;
            ledger.CouponId = 0;
            ledger.RcdInsTs = DateTime.Now.ToString();
            ledger.RcdUpdt = DateTime.Now.ToString();
            _formPayment.AddCredit(ledger);

            return RedirectToAction("MyAccount");
        }
        else
        {
            // user is not logged in
            return RedirectToAction("Login", "Account");
        }
    }


    [HttpPost]
    public IActionResult PayAmount(int Id)
    {
        if (User?.Identity != null && User.Identity.IsAuthenticated)
        {
            if (HttpContext.Session.GetString("SessionId") is null)
            {
                HttpContext.Session.SetString("SessionId", Guid.NewGuid().ToString());
            }
            string sessionId = HttpContext.Session.GetString("SessionId");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int roleId = 1;
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

            var formFeeDetails = _fee.GetUserFormFeeDetails(roleId, Id, Guid.Parse(userId));
             int IsNeedTopay = _fee.CheckforFeeSession(Id, Guid.Parse(userId), sessionId);
            ViewBag.FormFee = formFeeDetails;
            ViewBag.NeedToPay = IsNeedTopay;
            Decimal amountToPay = IsNeedTopay == 0 ? formFeeDetails.FTFeeAmount : formFeeDetails.FeeAmount;
            UserLedger ledger = new UserLedger();
            ledger.UserId = Guid.Parse(userId);
            ledger.Credit = 0;
            ledger.Debit = amountToPay;
            ledger.Particular = string.Format("Amount INR {0} paid for {1} record in Form - {2} ", amountToPay, (formFeeDetails.IsFormFilledByUser==false? "insert a new":"update an existing"), formFeeDetails.FormName);
            ledger.RoleId = roleId;
            ledger.CouponId = 0;
            ledger.RcdInsTs = DateTime.Now.ToString();
            ledger.RcdUpdt = DateTime.Now.ToString();
            _formPayment.AddDebit(ledger);
            _fee.AddNewFeeSession(Id, Guid.Parse(userId), sessionId);
            return RedirectToAction("OpenSurvey","Visitor", new { id = Id });
        }
        else
        {
            // user is not logged in
            return RedirectToAction("Login", "Account");
        }
    }
}

