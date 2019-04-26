namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public static class TextSanitiser
    {
        public static string SanitiseText(string inputText)
        {
            var text = inputText;

            text = HtmlTagRemover.StripOutTags(text);

            text = ExcelFormulaRemover.StripFormulae(text);

            return text;
        }
    }
}
