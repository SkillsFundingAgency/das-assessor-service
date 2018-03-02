namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        string SqlConnectionString { get; set; }
    }
}