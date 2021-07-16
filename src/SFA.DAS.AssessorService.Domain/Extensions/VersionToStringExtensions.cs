
namespace SFA.DAS.AssessorService.Domain.Extensions
{
    public static class VersionToStringExtensions
    {
        public static string VersionToString(this decimal? version)
        {
            return version.GetValueOrDefault(1).VersionToString();
        }

        public static string VersionToString(this decimal version)
        {
            return version.ToString("0.0#");
        }
    }
}
