namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure
{
    public interface IWebConfiguration
    {
        Settings.ExternalApiDataSync ExternalApiDataSync { get; set; }
    }
}