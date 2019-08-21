using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Internal
{
    public class UpdateBatchCertificateRequest
    {
        public string RequestId { get; set; }
        public CertificateData CertificateData { get; set; }

        public int UkPrn { get; set; }

    }
}
