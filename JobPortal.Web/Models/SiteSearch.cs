using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Dapper_ORM.Services;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using JobPortal.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Web.Models
{
    public class SiteSearchResultItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Snippet { get; set; }
        public string Url { get; set; } = "";
    }

    public class SiteSearchResults
    {
        public string Query { get; set; } = "";
        public List<Job> Jobs { get; set; } = new();
        public List<SiteSearchResultItem> LibraryItems { get; set; } = new();
        public List<SiteSearchResultItem> BlogPosts { get; set; } = new();
        public List<SiteSearchResultItem> Forms { get; set; } = new();
        public List<SiteSearchResultItem> QuestionCategories { get; set; } = new();
        public List<SiteSearchResultItem> Polls { get; set; } = new();
        public int TotalCount => Jobs.Count + LibraryItems.Count + BlogPosts.Count + Forms.Count
            + QuestionCategories.Count + Polls.Count;
    }

    // Raw shape for the Questions -> Questionnaires (category) join; not part of the public DTO
    // because the link target (CategoryId) differs from the question's own Id.
    internal class PollQuestionRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int CategoryId { get; set; }
    }

    public interface ISiteSearchService
    {
        Task<SiteSearchResults> SearchAsync(string query);
    }

    public class SiteSearchService : ISiteSearchService
    {
        private readonly IDapper _dapper;
        private readonly IJobService _jobs;
        private readonly AppDbContext _db;

        public SiteSearchService(IDapper dapper, IJobService jobs, AppDbContext db)
        {
            _dapper = dapper;
            _jobs = jobs;
            _db = db;
        }

        public async Task<SiteSearchResults> SearchAsync(string query)
        {
            var result = new SiteSearchResults { Query = query ?? "" };
            if (string.IsNullOrWhiteSpace(query))
                return result;

            // FREETEXT treats a multi-word query as "any of these words" (with word-form
            // matching), which is far too loose - e.g. "silver talent" would match anything
            // containing just "talent". CONTAINS with a quoted phrase requires the words to
            // appear together, matching how the old LIKE '%term%' search actually behaved.
            var phrase = "\"" + query.Replace("\"", "") + "\"";

            // Library, blog posts, poll categories and poll questions each open their own SQL
            // connection (see Dapperr.GetAll), so they're independent and safe to run concurrently.
            // Jobs and Forms both go through the same scoped AppDbContext, which isn't safe for
            // concurrent use, so that pair stays sequential - but runs alongside the four in parallel.
            var libraryTask = Task.Run(() => SearchLibrary(phrase));
            var postsTask = Task.Run(() => SearchBlogPosts(phrase));
            var categoryTask = Task.Run(() => SearchQuestionCategories(phrase));
            var pollTask = Task.Run(() => SearchPolls(phrase));

            result.Jobs = _jobs.Search(query, 1, 10).ToList();
            result.Forms = await SearchFormsAsync(query);

            result.LibraryItems = await libraryTask;
            result.BlogPosts = await postsTask;
            result.QuestionCategories = await categoryTask;
            result.Polls = await pollTask;

            return result;
        }

        private List<SiteSearchResultItem> SearchLibrary(string phrase)
        {
            var libParams = new DynamicParameters();
            libParams.Add("@q", phrase, DbType.String);
            var rows = _dapper.GetAll<SiteSearchResultItem>(
                @"SELECT TOP 10 PKID AS Id, Title, Content AS Snippet FROM dbo.LibraryList
                  WHERE ISNULL(IsDelete, 0) = 0 AND CONTAINS((Title, Content), @q)
                  ORDER BY PKID DESC",
                libParams, commandType: CommandType.Text);
            return rows.Select(r => new SiteSearchResultItem
            {
                Id = r.Id,
                Title = r.Title,
                Snippet = ToSnippet(r.Snippet),
                Url = $"/Visitor/Library/{r.Id}"
            }).ToList();
        }

        private List<SiteSearchResultItem> SearchBlogPosts(string phrase)
        {
            var postParams = new DynamicParameters();
            postParams.Add("@q", phrase, DbType.String);
            var rows = _dapper.GetAll<SiteSearchResultItem>(
                @"SELECT TOP 10 PKID AS Id, Title, PostContent AS Snippet FROM dbo.WPPosts
                  WHERE IsDeleted = 0 AND IsActive = 1 AND CONTAINS((Title, PostContent), @q)
                  ORDER BY PostDate DESC",
                postParams, commandType: CommandType.Text);
            return rows.Select(r => new SiteSearchResultItem
            {
                Id = r.Id,
                Title = r.Title,
                Snippet = ToSnippet(r.Snippet),
                Url = $"/Visitor/TrendingNews/{r.Id}"
            }).ToList();
        }

        private async Task<List<SiteSearchResultItem>> SearchFormsAsync(string query)
        {
            var forms = await _db.FormTypeCategory
                .Where(f => f.IsActive && f.IsPublic &&
                    (f.FormCategory.Contains(query) || f.Details.Contains(query)))
                .OrderBy(f => f.DisplayOrder)
                .Take(10)
                .ToListAsync();
            return forms.Select(f => new SiteSearchResultItem
            {
                Id = f.Id,
                Title = f.FormCategory,
                Snippet = ToSnippet(f.Details),
                Url = $"/Visitor/OpenSurvey/{f.Id}"
            }).ToList();
        }

        private List<SiteSearchResultItem> SearchQuestionCategories(string phrase)
        {
            var categoryParams = new DynamicParameters();
            categoryParams.Add("@q", phrase, DbType.String);
            var rows = _dapper.GetAll<SiteSearchResultItem>(
                @"SELECT TOP 10 PKID AS Id, Questionaire AS Title,
                    (ISNULL(SubHeading, '') + ' ' + ISNULL(Details, '')) AS Snippet
                  FROM dbo.Questionnaires
                  WHERE IsActive = 1 AND IsDelete = 0
                    AND CONTAINS((Questionaire, SubHeading, Details), @q)
                  ORDER BY DisplayOrder",
                categoryParams, commandType: CommandType.Text);
            return rows.Select(r => new SiteSearchResultItem
            {
                Id = r.Id,
                Title = r.Title,
                Snippet = ToSnippet(r.Snippet),
                Url = $"/Visitor/Poll/{r.Id}"
            }).ToList();
        }

        private List<SiteSearchResultItem> SearchPolls(string phrase)
        {
            var pollParams = new DynamicParameters();
            pollParams.Add("@q", phrase, DbType.String);
            var rows = _dapper.GetAll<PollQuestionRow>(
                @"SELECT TOP 10 q.PKID AS Id, q.Question AS Title, m.CategoryId AS CategoryId
                  FROM dbo.Questions q
                  INNER JOIN dbo.Questionnaries_Question_Mapping m ON m.QuestionId = q.PKID
                  WHERE q.IsActive = 1 AND CONTAINS(q.Question, @q)
                  ORDER BY q.PKID DESC",
                pollParams, commandType: CommandType.Text);
            return rows.Select(r => new SiteSearchResultItem
            {
                Id = r.Id,
                Title = ToSnippet(r.Title) ?? r.Title,
                Url = $"/Visitor/Poll/{r.CategoryId}"
            }).ToList();
        }

        private static string? ToSnippet(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return null;

            var text = Regex.Replace(html, "<.*?>", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text.Length > 160 ? text[..160] + "..." : text;
        }
    }
}
