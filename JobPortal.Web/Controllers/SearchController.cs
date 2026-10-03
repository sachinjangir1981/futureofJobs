using JobPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISiteSearchService _search;

        public SearchController(ISiteSearchService search)
        {
            _search = search;
        }

        public async Task<IActionResult> Index(string q)
        {
            var results = await _search.SearchAsync(q);
            return View(results);
        }
    }
}
