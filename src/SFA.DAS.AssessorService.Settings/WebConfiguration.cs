using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AuthSettings Authentication { get; set; }

        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication ClientApiAuthentication { get; set; }

        [JsonRequired] public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        [JsonRequired] public CertificateDetails CertificateDetails { get; set; }

        [JsonRequired] public SftpSettings Sftp { get; set; }

        [JsonRequired] public string AssessmentOrgsApiClientBaseUrl { get; set; }

        [JsonRequired] public string IFATemplateStorageConnectionString { get; set; }

        [JsonRequired] public string SqlConnectionString { get; set; }

        [JsonRequired] public string SessionRedisConnectionString { get; set; }

        [JsonRequired] public AuthSettings StaffAuthentication { get; set; }
        [JsonRequired] public string GitUsername { get; set; }
        [JsonRequired] public string GitPassword { get; set; }
        [JsonRequired] public string AssessmentOrgsUrl { get; set; }
    }
}