namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure.Settings
{
    public class ExternalApiDataSync : IExternalApiDataSync
    {
        public bool IsEnabled { get; set; }
        public string SourceSqlConnectionString { get; set; }
    }
}