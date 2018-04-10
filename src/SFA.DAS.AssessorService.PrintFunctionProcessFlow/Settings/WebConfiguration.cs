using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AuthSettings Authentication { get; set; }

        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication ClientApiAuthentication { get; set; }

        [JsonRequired] public SftpSettings Sftp { get; set; }

        [JsonRequired] public string SqlConnectionString { get; set; }
    }
}