namespace SFA.DAS.AssessorService.Application.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            var result = value.Substring(0, 1).ToLower() +
                         value.Substring(1);
            return result;
        }
    }
}
