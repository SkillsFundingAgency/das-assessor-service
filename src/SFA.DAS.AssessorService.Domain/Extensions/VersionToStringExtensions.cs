
namespace SFA.DAS.AssessorService.Domain.Extensions
{
    public static class VersionToStringExtensions
    {
        public static string VersionToString(this decimal? version)
        {
            return version.GetValueOrDefault(1).ToString("0.0#");
        }
    }
}
