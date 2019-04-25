using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class SftpSettings : ISftp
    {
        [JsonRequired]
        public string RemoteHost { get; set; }
        [JsonRequired]
        public int Port { get; set; }
        [JsonRequired]
        public string Username { get; set; }
        [JsonRequired]
        public string Password { get; set; }
        [JsonRequired]
        public string UploadDirectory { get; set; }
        [JsonRequired]
        public string ProofDirectory { get; set; }
        [JsonRequired]
        public bool UseJson { get; set; }
    }
}
