namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        SftpSettings Sftp { get; set; }
        string SqlConnectionString { get; set; }
    }
}