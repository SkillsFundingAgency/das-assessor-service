
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class StatusCapitalisationExtensions
    {
        public static string FormatStatus(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var withSpaces = Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");

            return $"{char.ToUpper(withSpaces[0]) + withSpaces.Substring(1).ToLower()}";
        }
    }
}
