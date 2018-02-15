namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiSettings Api { get; set; }
        string SqlConnectionString { get; set; }
    }
}