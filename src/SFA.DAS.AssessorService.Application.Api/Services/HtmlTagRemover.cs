namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public static class HtmlTagRemover
    {

        public static string StripOutTags(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            var text = inputText;
  
            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            text= rx.Replace(text, string.Empty);

            while (text.Contains("<"))
            {
                text = text+">";
                text = rx.Replace(text, string.Empty);
            }

            return text;
        }
    }
}
