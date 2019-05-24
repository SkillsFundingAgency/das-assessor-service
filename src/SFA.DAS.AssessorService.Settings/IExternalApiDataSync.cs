namespace SFA.DAS.AssessorService.Settings
{
    public interface IExternalApiDataSync
    {
        bool IsEnabled { get; set; }
        string SourceSqlConnectionString { get; set; }
    }
}