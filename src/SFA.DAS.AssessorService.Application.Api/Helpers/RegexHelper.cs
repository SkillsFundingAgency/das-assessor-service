using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Application.Api.Helpers
{
    public class RegexHelper : IRegexHelper
    {
        public bool RegexMatchSuccess(string input, string regexPattern)
        {
            var regex = new Regex(regexPattern);
            return regex.Match(input).Success;
        }
    }
}
