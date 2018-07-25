using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class CertificateRequest
    {
        public long Uln { get; set; }
        public string LastName { get; set; }
        public int StdCode { get; set; }

        public CertificateData CertificateData { get; set; }
    }
}
