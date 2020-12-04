
namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class StatusCapitalisationExtensions
    {
        public static string CapitaliseFirstLetterOnly(this string value)
        {
            return string.IsNullOrEmpty(value) ? value : $"{char.ToUpper(value[0]) + value.Substring(1).ToLower()}";  
        }
    }
}
