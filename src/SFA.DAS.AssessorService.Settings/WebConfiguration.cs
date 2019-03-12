using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AuthSettings Authentication { get; set; }

        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }

        [JsonRequired] public AzureApiAuthentication AzureApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication ClientApiAuthentication { get; set; }

        [JsonRequired] public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        [JsonRequired] public CertificateDetails CertificateDetails { get; set; }

        [JsonRequired] public SftpSettings Sftp { get; set; }

        [JsonRequired] public string AssessmentOrgsApiClientBaseUrl { get; set; }

        [JsonRequired] public string IfaApiClientBaseUrl { get; set; }

        [JsonRequired] public string IFATemplateStorageConnectionString { get; set; }

        [JsonRequired] public string SqlConnectionString { get; set; }

        public string SpecflowDBTestConnectionString { get; set; }

        [JsonRequired] public string SessionRedisConnectionString { get; set; }
        [JsonRequired] public AuthSettings StaffAuthentication { get; set; }
        [JsonRequired] public ClientApiAuthentication ApplyApiAuthentication { get; set; }
        [JsonRequired] public string ApplyBaseAddress { get; set; }
        [JsonRequired] public string ServiceLink { get; set; }
        [JsonRequired] public DfeSignInConfig DfeSignIn { get; set; }
    }
}