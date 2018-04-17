using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public class CertificateDetails : ICertificateDetails
    {
        [JsonRequired]
        public string ChairName{ get; set; }
        [JsonRequired]
        public string ChairTitle { get; set; }
    }
}
