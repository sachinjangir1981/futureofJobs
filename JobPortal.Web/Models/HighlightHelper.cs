using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;

namespace JobPortal.Web.Models
{
    // Wraps a search term in <mark> wherever it appears, for use on the search results page and
    // on the content detail pages (Library/TrendingNews/Poll/Jobs) reached from a search result.
    public static class HighlightHelper
    {
        // For plain text (e.g. titles) that hasn't been rendered as HTML yet: encodes it safely
        // and then highlights, so the query text itself can never inject markup.
        public static IHtmlContent HighlightText(string? text, string? query)
        {
            if (string.IsNullOrEmpty(text))
                return HtmlString.Empty;

            var encoded = HtmlEncoder.Default.Encode(text);
            if (string.IsNullOrWhiteSpace(query))
                return new HtmlString(encoded);

            var encodedQuery = HtmlEncoder.Default.Encode(query);
            var highlighted = Regex.Replace(encoded, Regex.Escape(encodedQuery), "<mark>$0</mark>", RegexOptions.IgnoreCase);
            return new HtmlString(highlighted);
        }

        // For content that is already rendered HTML (rich article/question bodies): highlights
        // only inside text nodes, leaving tag names and attributes untouched, via a
        // "not inside a tag" lookaround rather than parsing the HTML.
        public static IHtmlContent HighlightHtml(string? html, string? query)
        {
            if (string.IsNullOrEmpty(html))
                return HtmlString.Empty;
            if (string.IsNullOrWhiteSpace(query))
                return new HtmlString(html);

            var pattern = $"(?<!<[^>]*)({Regex.Escape(query)})(?![^<]*>)";
            var highlighted = Regex.Replace(html, pattern, "<mark>$1</mark>", RegexOptions.IgnoreCase);
            return new HtmlString(highlighted);
        }
    }
}
