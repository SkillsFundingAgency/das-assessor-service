namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }
        CertificateDetails CertificateDetails { get; set; }
        SftpSettings Sftp { get; set; }
        string AssessmentOrgsApiClientBaseUrl { get; set; }
        string IFATemplateStorageConnectionString { get; set; }
        string SqlConnectionString { get; set; }
        string SessionRedisConnectionString { get; set; }
        AuthSettings StaffAuthentication { get; set; }
        string GitUsername { get; set; }
        string GitPassword { get; set; }
        string AssessmentOrgsUrl { get; set; }
    }
}