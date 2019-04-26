namespace SFA.DAS.AssessorService.Application.Api.Services
{
    public static class ExcelFormulaRemover
    {
        public static string StripFormulae(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            var text = inputText;

            text = text.Replace("=", string.Empty);

            return text;
        }
    }
}
