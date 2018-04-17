using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AuthSettings Authentication { get; set; }

        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication ClientApiAuthentication { get; set; }

        [JsonRequired]  public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        [JsonRequired] public CertificateDetails CertificateDetails { get; set; }

        [JsonRequired] public SftpSettings Sftp { get; set; }

        [JsonRequired] public EmailTemplateSettings EmailTemplateSettings { get; set; }
   
        [JsonRequired] public string SqlConnectionString { get; set; }
    }
}