
namespace SFA.DAS.AssessorService.Data.Extensions
{
    public static class VersionToStringExtensions
    {
        public static string VersionToString(this decimal? version)
        {
            return version.GetValueOrDefault(1).ToString("0.0#");
        }
    }
}
