using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.Messages
{
    public class BatchCertificateRequest
    {
        public CertificateData CertificateData { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }

    }
}
