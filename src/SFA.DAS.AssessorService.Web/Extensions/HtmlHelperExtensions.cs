using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent SetZenDeskSuggestion(this IHtmlHelper html, string suggestion)
        {            
            return new HtmlString($"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{  search: '{suggestion}' }});</script>");
        }

        public static IHtmlContent SetZenDeskLabels(this IHtmlHelper html, params string[] labels)
        {
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!first) apiCallString += ",";
                first = false;

                apiCallString += $"'{ EscapeApostrophes(label) }'";
            }

            apiCallString += "] });</script>";

            return new HtmlString(apiCallString);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }
    }
}
