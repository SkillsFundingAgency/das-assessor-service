using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiAuthentication ApiAuthentication { get; set; }
        ClientApiAuthentication ClientApiAuthentication { get; set; }
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }
        CertificateDetails CertificateDetails { get; set; }
        SftpSettings Sftp { get; set; }
        EmailTemplateSettings EmailTemplateSettings { get; set; }
        string SqlConnectionString { get; set; }
    }
}