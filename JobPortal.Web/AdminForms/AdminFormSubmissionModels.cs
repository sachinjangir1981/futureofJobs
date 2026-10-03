using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.AdminForms
{
    public class AdminFormSubmissionListItem
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<int> FormIds { get; set; } = new();
        public List<string> FormNames { get; set; } = new();
        public string FormNamesDisplay => string.Join(", ", FormNames);
        public Dictionary<int, int> FormAnswerCounts { get; set; } = new();
        public Dictionary<int, int> FormTotalQuestions { get; set; } = new();
        public Dictionary<int, List<AdminFormVersion>> FormVersions { get; set; } = new();
        public int AnsweredQuestions { get; set; }
        public DateTime LastSubmittedAt { get; set; }
    }

    public class AdminUserSubmissionsDetail
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<AdminFormSubmissionDetail> Forms { get; set; } = new();
    }

    public class AdminFormSubmissionDetail
    {
        public int FormId { get; set; }
        public string FormName { get; set; } = "";
        public List<AdminFormVersion> Versions { get; set; } = new();
        public int SelectedVersionId { get; set; }
        public List<AdminSubmissionSection> Sections { get; set; } = new();
    }

    public class AdminFormVersionRow
    {
        public int Id { get; set; }
        public int? FormId { get; set; }
        public Guid? UserId { get; set; }
        public string? Label { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class AdminFormVersion
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

    public class AdminSubmissionSection
    {
        public string Title { get; set; } = "";
        public List<AdminSubmissionQuestion> Questions { get; set; } = new();
    }

    public class AdminSubmissionQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public QuestionType QuestionType { get; set; }
        public string DisplayAnswer { get; set; } = "";
        public List<AdminSubmissionOption> Options { get; set; } = new();
        public string? FilePath { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }

    public class AdminSubmissionOption
    {
        public string OptionText { get; set; } = "";
        public bool IsSelected { get; set; }
    }

    // Shared between the admin (authenticated) submission viewer and the public share-link
    // viewer, so both render identical read-only data from a single code path.
    public static class FormSubmissionBuilder
    {
        public static async Task<AdminUserSubmissionsDetail?> BuildAsync(
            AppDbContext db, UserManager<ApplicationUser> um, string userId, List<int> requestedFormIds, int? versionId = null)
        {
            var user = await um.FindByIdAsync(userId);
            if (user == null)
                return null;

            var filledFormIds = await db.FormAnswers
                .Where(a => a.UserId == userId && !a.IsDraft)
                .Select(a => a.Question.Section.FormTypeCategoryId)
                .Distinct()
                .ToListAsync();

            if (requestedFormIds.Any())
                filledFormIds = filledFormIds.Where(id => requestedFormIds.Contains(id)).ToList();

            var forms = await db.FormTypeCategory
                .Where(f => filledFormIds.Contains(f.Id))
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();

            var answers = await db.FormAnswers
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var vm = new AdminUserSubmissionsDetail
            {
                UserId = userId,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Forms = new List<AdminFormSubmissionDetail>()
            };

            foreach (var form in forms)
            {
                var sections = await db.FormSections
                    .Where(s => s.IsActive && s.FormTypeCategoryId == form.Id)
                    .Include(s => s.Questions)
                        .ThenInclude(q => q.Options)
                    .OrderBy(s => s.DisplayOrder)
                    .ToListAsync();

                var versions = await db.Database
                    .SqlQuery<AdminFormVersion>($@"SELECT Id, FormVersion AS Label, DATEADD(minute, 330, RcdInsTs) AS CreatedAt
                        FROM FormSessionEntry WHERE FormId = {form.Id} AND UserId = {Guid.Parse(userId)} ORDER BY RcdInsTs DESC")
                    .ToListAsync();

                // Every submit in a new session is a new version; show the requested one, else the newest that has answers.
                var questionIds = sections.SelectMany(s => s.Questions).Select(q => q.Id).ToHashSet();
                var formAnswers = answers.Where(a => questionIds.Contains(a.QuestionId)).ToList();
                var selectedVersionId = versions.Any(v => v.Id == versionId)
                    ? versionId!.Value
                    : versions.Select(v => v.Id).FirstOrDefault(id => formAnswers.Any(a => a.FormSessionEntryId == id));

                var answersByQuestion = formAnswers
                    .Where(a => selectedVersionId == 0 || a.FormSessionEntryId == selectedVersionId)
                    .GroupBy(a => a.QuestionId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.SubmittedAt).ThenByDescending(a => a.Id).First());

                vm.Forms.Add(new AdminFormSubmissionDetail
                {
                    FormId = form.Id,
                    FormName = form.FormCategory,
                    Versions = versions,
                    SelectedVersionId = selectedVersionId,
                    Sections = sections
                        .OrderBy(s => s.DisplayOrder)
                        .Select(s => new AdminSubmissionSection
                        {
                            Title = s.Title,
                            Questions = s.Questions
                                .OrderBy(q => q.DisplayOrder)
                                .Select(q =>
                                {
                                    answersByQuestion.TryGetValue(q.Id, out var ans);
                                    return new AdminSubmissionQuestion
                                    {
                                        QuestionId = q.Id,
                                        QuestionText = q.QuestionText,
                                        QuestionType = q.QuestionType,
                                        DisplayAnswer = FormatAnswer(q, ans),
                                        Options = BuildOptions(q, ans),
                                        FilePath = ans?.FilePath,
                                        SubmittedAt = ans?.SubmittedAt
                                    };
                                })
                                .ToList()
                        })
                        .ToList()
                });
            }

            return vm;
        }

        private static string FormatAnswer(FormQuestion question, FormAnswer? answer)
        {
            if (answer == null || string.IsNullOrEmpty(answer.AnswerText))
                return "";

            if (question.QuestionType == QuestionType.SingleChoice || question.QuestionType == QuestionType.MultipleChoice)
            {
                var selectedIds = GetSelectedOptionIds(answer);

                var labels = question.Options
                    .Where(o => selectedIds.Contains(o.Id))
                    .Select(o => o.OptionText)
                    .ToList();

                // Neither AnswerId nor AnswerText resolved to a known option (some questions
                // store literal text/HTML instead) - fall back to showing it as-is rather than blank.
                if (labels.Count == 0)
                    return answer.AnswerText;

                return string.Join(", ", labels);
            }

            return answer.AnswerText;
        }

        private static List<AdminSubmissionOption> BuildOptions(FormQuestion question, FormAnswer? answer)
        {
            if (question.QuestionType != QuestionType.SingleChoice && question.QuestionType != QuestionType.MultipleChoice)
                return new List<AdminSubmissionOption>();

            var selectedIds = GetSelectedOptionIds(answer);

            return question.Options
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new AdminSubmissionOption
                {
                    OptionText = o.OptionText,
                    IsSelected = selectedIds.Contains(o.Id)
                })
                .ToList();
        }

        private static HashSet<int> GetSelectedOptionIds(FormAnswer? answer)
        {
            var ids = new HashSet<int>();
            if (answer == null)
                return ids;

            // AnswerId is a direct FK to the selected FormOptions.Id (used for single-select answers).
            if (answer.AnswerId != 0)
                ids.Add(answer.AnswerId);

            // AnswerText can additionally hold '|'-joined option ids (used for multi-select answers).
            foreach (var part in (answer.AnswerText ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part, out var id))
                    ids.Add(id);
            }

            return ids;
        }
    }
}
