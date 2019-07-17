namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure.Settings
{
    public interface IExternalApiDataSync
    {
        bool IsEnabled { get; set; }
    }
}