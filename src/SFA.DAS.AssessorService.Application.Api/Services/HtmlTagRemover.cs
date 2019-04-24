namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public static class HtmlTagRemover
    {

        public static string StripOutTags(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            return rx.Replace(inputText, string.Empty);
        }
    }
}
