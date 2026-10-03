using System.Security.Claims;
using System.Text.RegularExpressions;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.AdminAccess
{
    public static class AdminSections
    {
        public const string Dashboard = "Dashboard";
        public const string Forms = "Forms";
        public const string FormFees = "FormFees";
        public const string FormSubmissions = "FormSubmissions";
        public const string Masters = "Masters";
        public const string Polls = "Polls";
        public const string Cms = "Cms";
        public const string Profiles = "Profiles";
        public const string Jobs = "Jobs";
        public const string AdminUsers = "AdminUsers";

        public const int View = 1;
        public const int Full = 2;

        // Sections a sub-role can be granted. Dashboard is open to every admin and AdminUsers is Super Admin only.
        public static readonly (string Key, string Title)[] Grantable =
        {
            (Forms, "Forms (categories, sections, questions, entries)"),
            (FormFees, "Form Fees"),
            (FormSubmissions, "Form Submissions"),
            (Masters, "Masters (skills)"),
            (Polls, "Polls"),
            (Cms, "CMS (library, survey, timepass, news, one minute, my point)"),
            (Profiles, "User Profiles"),
            (Jobs, "Curated Jobs"),
        };

        private static readonly Dictionary<string, string> ControllerSection = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Dashboard"] = Dashboard,
            ["Home"] = Dashboard,
            ["Forms"] = Forms,
            ["AdminAccounts"] = FormFees,
            ["AdminFormSubmissions"] = FormSubmissions,
            ["Skill"] = Masters,
            ["SkillCategory"] = Masters,
            ["Polls"] = Polls,
            ["QuestionCategory"] = Polls,
            ["QuestionMaster"] = Polls,
            ["Library"] = Cms,
            ["QuestionForms"] = Cms,
            ["Timepass"] = Cms,
            ["TimePass5Line"] = Cms,
            ["OneMinute"] = Cms,
            ["MyPoint"] = Cms,
            ["TNCategory"] = Cms,
            ["TPCategory"] = Cms,
            ["TrendingNews"] = Cms,
            ["Profiles"] = Profiles,
            ["CuratedJobs"] = Jobs,
            ["AdminUsers"] = AdminUsers,
        };

        // Anything not listed above is treated as Super Admin only, so new controllers are closed by default.
        public static string ForController(string controller) =>
            ControllerSection.TryGetValue(controller, out var s) ? s : AdminUsers;
    }

    // Admin screens take and show times in India Standard Time (the rest of the app does too); storage is UTC.
    public static class AdminTime
    {
        private static readonly TimeSpan Ist = TimeSpan.FromMinutes(330);

        public static string? ToInput(DateTime? utc) =>
            utc.HasValue ? DateTime.SpecifyKind(utc.Value, DateTimeKind.Utc).Add(Ist).ToString("yyyy-MM-ddTHH:mm") : null;

        public static string Display(DateTime utc) =>
            DateTime.SpecifyKind(utc, DateTimeKind.Utc).Add(Ist).ToString("dd/MM/yyyy hh:mm tt");

        public static bool TryParseInput(string? input, out DateTime? utc)
        {
            utc = null;
            if (string.IsNullOrWhiteSpace(input)) return true;
            if (!DateTime.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var local))
                return false;
            utc = DateTime.SpecifyKind(local - Ist, DateTimeKind.Utc);
            return true;
        }
    }

    public class AdminAccessContext
    {
        public bool IsAdmin { get; init; }

        // Set when a sub-role assignment is outside its validity window; explains why access is empty.
        public string? AccessNotice { get; init; }
        public bool IsSuperAdmin { get; init; }
        public string SubRoleName { get; init; } = "Super Admin";
        public Dictionary<string, int> Levels { get; init; } = new();

        // null = every form.
        public HashSet<int>? AllowedFormIds { get; init; }

        public bool IsFormScoped => AllowedFormIds != null;

        // null = every user. Compared case-insensitively because user ids appear in both cases across the app.
        public HashSet<string>? AllowedUserIds { get; init; }

        public bool IsUserScoped => AllowedUserIds != null;
        public bool CanAccessUser(string? userId) => AllowedUserIds == null || string.IsNullOrWhiteSpace(userId) || AllowedUserIds.Contains(userId);

        public int LevelFor(string section)
        {
            if (!IsAdmin) return 0;
            if (IsSuperAdmin) return AdminSections.Full;
            if (section == AdminSections.Dashboard) return AdminSections.Full;
            if (section == AdminSections.AdminUsers) return 0;
            return Levels.TryGetValue(section, out var l) ? l : 0;
        }

        public bool CanView(string section) => LevelFor(section) >= AdminSections.View;
        public bool CanEdit(string section) => LevelFor(section) >= AdminSections.Full;
        public bool CanAccessForm(int formId) => AllowedFormIds == null || AllowedFormIds.Contains(formId);

        // Narrows a requested form list to the forms this admin may see. An empty result is returned as
        // [-1] because an empty list means "all forms" to callers.
        public List<int> RestrictForms(IEnumerable<int> requested)
        {
            var list = requested.ToList();
            if (AllowedFormIds == null) return list;

            var scoped = (list.Any() ? list.Where(AllowedFormIds.Contains) : AllowedFormIds).ToList();
            return scoped.Any() ? scoped : new List<int> { -1 };
        }
    }

    public interface IAdminAccessService
    {
        Task<AdminAccessContext> GetAsync();
    }

    public class AdminAccessService : IAdminAccessService
    {
        private const string CacheKey = "AdminAccessContext";
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public AdminAccessService(AppDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task<AdminAccessContext> GetAsync()
        {
            var httpContext = _http.HttpContext;
            if (httpContext == null)
                return new AdminAccessContext();

            if (httpContext.Items[CacheKey] is AdminAccessContext cached)
                return cached;

            var ctx = await BuildAsync(httpContext.User);
            httpContext.Items[CacheKey] = ctx;
            return ctx;
        }

        private async Task<AdminAccessContext> BuildAsync(ClaimsPrincipal user)
        {
            if (user.Identity?.IsAuthenticated != true || !user.IsInRole("Admin"))
                return new AdminAccessContext();

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignment = userId == null
                ? null
                : await _db.AdminUserSubRoles
                    .AsNoTracking()
                    .Include(a => a.SubRole).ThenInclude(r => r!.Permissions)
                    .Include(a => a.SubRole).ThenInclude(r => r!.Forms)
                    .Include(a => a.Users)
                    .FirstOrDefaultAsync(a => a.UserId == userId);

            // No assignment means a Super Admin, so existing admins keep full access.
            if (assignment?.SubRole == null)
                return new AdminAccessContext { IsAdmin = true, IsSuperAdmin = true };

            var role = assignment.SubRole;

            // Once the end time passes the admin keeps their login but has no section access; they never fall back to Super Admin.
            if (assignment.ValidUntil <= DateTime.UtcNow)
            {
                return new AdminAccessContext
                {
                    IsAdmin = true,
                    SubRoleName = role.Name,
                    AccessNotice = $"Your \"{role.Name}\" admin access ended on {AdminTime.Display(assignment.ValidUntil)}. Ask a Super Admin to extend it."
                };
            }

            return new AdminAccessContext
            {
                IsAdmin = true,
                SubRoleName = role.Name,
                Levels = role.IsActive
                    ? role.Permissions.ToDictionary(p => p.Section, p => p.AccessLevel)
                    : new Dictionary<string, int>(),
                AllowedFormIds = role.AllFormsAccess ? null : role.Forms.Select(f => f.FormTypeCategoryId).ToHashSet(),
                AllowedUserIds = assignment.AllUsersAccess
                    ? null
                    : assignment.Users.Select(u => u.TargetUserId).ToHashSet(StringComparer.OrdinalIgnoreCase)
            };
        }
    }

    // Applies to every controller in the Admin area: requires an admin, then checks the section, view vs
    // edit, and (for the Forms controller) the form scope.
    public class AdminAccessFilter : IAsyncActionFilter
    {
        private static readonly Regex WriteStart = new("^(Create|Edit|Add|Update|Save|Generate|Import|Upload|Toggle|Set)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex WriteAnywhere = new("(Delete|Remove|Revoke|Approve|Reject)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly HashSet<string> FormIdActions = new(StringComparer.OrdinalIgnoreCase)
        {
            "Index", "Sections", "CreateSection", "ManageForm", "EditCategory", "CategoryDetails", "CategoryDeleteConfirmed",
            "Ratings", "Compare", "ViewEntries", "ViewData", "GetDataByFormandUserId", "GetRatingDataByFormandUserId"
        };

        private readonly IAdminAccessService _access;
        private readonly AppDbContext _db;

        public AdminAccessFilter(IAdminAccessService access, AppDbContext db)
        {
            _access = access;
            _db = db;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var area = context.RouteData.Values["area"] as string;
            if (!string.Equals(area, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new ChallengeResult();
                return;
            }

            var access = await _access.GetAsync();
            var controller = context.RouteData.Values["controller"] as string ?? "";
            var action = context.RouteData.Values["action"] as string ?? "";

            if (!access.IsAdmin)
            {
                context.Result = Denied(context, "This area is for administrators only.");
                return;
            }

            var section = AdminSections.ForController(controller);
            var method = context.HttpContext.Request.Method;
            var isWrite = !(HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
                          || WriteStart.IsMatch(action) || WriteAnywhere.IsMatch(action);

            if (!(isWrite ? access.CanEdit(section) : access.CanView(section)))
            {
                context.Result = Denied(context, access.AccessNotice ?? (isWrite
                    ? "Your admin role has view-only access to this section."
                    : "Your admin role doesn't include this section."));
                return;
            }

            // Any request that names a user (userId, or "userId|profileId" on the compare screens) must be in scope.
            if (access.IsUserScoped
                && context.ActionArguments.TryGetValue("userId", out var userArg)
                && userArg is string userValue
                && !access.CanAccessUser(userValue.Split('|')[0]))
            {
                context.Result = Denied(context, "You don't have access to this user's submissions.");
                return;
            }

            if (access.IsFormScoped && string.Equals(controller, "Forms", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(action, "CreateCategory", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = Denied(context, "Creating new forms needs access to all forms.");
                    return;
                }

                foreach (var formId in await ResolveFormIdsAsync(context, action))
                {
                    if (!access.CanAccessForm(formId))
                    {
                        context.Result = Denied(context, "You don't have access to this form.");
                        return;
                    }
                }
            }

            await next();
        }

        private static IActionResult Denied(ActionExecutingContext context, string message)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            var controller = (Controller)context.Controller;
            controller.ViewBag.AccessDeniedMessage = message;
            return controller.View("~/Areas/Admin/Views/Shared/AccessDenied.cshtml");
        }

        private async Task<List<int>> ResolveFormIdsAsync(ActionExecutingContext context, string action)
        {
            var ids = new List<int>();
            var args = context.ActionArguments;

            int? Arg(string name) => args.TryGetValue(name, out var v) && v is int i ? i : null;

            var id = Arg("id");
            var sectionId = Arg("sectionId");
            var questionId = Arg("questionId");

            if (FormIdActions.Contains(action) && id.HasValue)
                ids.Add(id.Value);

            switch (action.ToLowerInvariant())
            {
                case "editsection":
                case "deletesection":
                    await AddAsync(ids, SectionFormAsync(id));
                    break;
                case "questions":
                case "createquestion":
                    await AddAsync(ids, SectionFormAsync(sectionId));
                    break;
                case "editquestion":
                case "deletequestion":
                    await AddAsync(ids, QuestionFormAsync(id));
                    break;
                case "options":
                case "createoption":
                    await AddAsync(ids, QuestionFormAsync(questionId));
                    break;
                case "editoption":
                case "deleteoption":
                    await AddAsync(ids, OptionFormAsync(id));
                    break;
            }

            // Posted models carry their own parent id, which must also be in scope.
            foreach (var arg in args.Values)
            {
                switch (arg)
                {
                    case FormSection s when s.FormTypeCategoryId > 0:
                        ids.Add(s.FormTypeCategoryId);
                        break;
                    case FormQuestion q when q.SectionId > 0:
                        await AddAsync(ids, SectionFormAsync(q.SectionId));
                        break;
                    case FormOption o when o.QuestionId > 0:
                        await AddAsync(ids, QuestionFormAsync(o.QuestionId));
                        break;
                    case FormTypeCategory c when c.Id > 0:
                        ids.Add(c.Id);
                        break;
                }
            }

            return ids;
        }

        private static async Task AddAsync(List<int> ids, Task<int?> lookup)
        {
            var value = await lookup;
            if (value.HasValue)
                ids.Add(value.Value);
        }

        private Task<int?> SectionFormAsync(int? sectionId) =>
            sectionId == null ? Task.FromResult<int?>(null)
                : _db.FormSections.AsNoTracking().Where(s => s.Id == sectionId).Select(s => (int?)s.FormTypeCategoryId).FirstOrDefaultAsync();

        private Task<int?> QuestionFormAsync(int? questionId) =>
            questionId == null ? Task.FromResult<int?>(null)
                : _db.FormQuestions.AsNoTracking().Where(q => q.Id == questionId).Select(q => (int?)q.Section.FormTypeCategoryId).FirstOrDefaultAsync();

        private Task<int?> OptionFormAsync(int? optionId) =>
            optionId == null ? Task.FromResult<int?>(null)
                : _db.Set<FormOption>().AsNoTracking().Where(o => o.Id == optionId).Select(o => (int?)o.Question.Section.FormTypeCategoryId).FirstOrDefaultAsync();
    }
}
