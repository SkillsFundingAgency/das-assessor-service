namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure
{
    public interface IWebConfiguration
    {
        string SqlConnectionString { get; set; }
        string SandboxSqlConnectionString { get; set; }
        Settings.ExternalApiDataSync ExternalApiDataSync { get; set; }
    }
}