namespace SFA.DAS.AssessorService.Application.Api.Helpers
{
    public interface IRegexHelper
    {
        bool RegexMatchSuccess(string input, string regexPattern);
    }
}