using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Web.Helpers
{
    public class ApplicationDataFormatHelper
    {
        public static string FormatApplicationDataPropertyPlaceholders(string input, Dictionary<string, object> applicationData)
        {
            string formattedText = input;

            Func<Match, string> evaluator = (match) =>
            {
                var propertyName = match.Groups[2].Value;
                var alignment = match.Groups[3].Value;
                var formatString = match.Groups[4].Value;

                return applicationData.TryGetValue(propertyName, out object value)
                    ? string.Format("{0" + alignment + formatString + "}", value)
                    : string.Empty;
            };

            try
            {
                formattedText = Regex.Replace(
                    formattedText,
                    "{{((\\w+)(,[0-9]*)?)(:[\\w\\s.:/]*)?}}",
                    new MatchEvaluator(evaluator),
                    RegexOptions.IgnorePatternWhitespace,
                    TimeSpan.FromSeconds(.25));
            }
            catch (RegexMatchTimeoutException) { }

            return formattedText;
        }
    }
}
