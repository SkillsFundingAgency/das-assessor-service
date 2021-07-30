using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates
{
    public class SubmitBatchCertificateRequest : IRequest<Certificate>
    {
        public string RequestId { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string FamilyName { get; set; }
        public string CertificateReference { get; set; }

        public int UkPrn { get; set; }

        public string GetStandardId() => !string.IsNullOrWhiteSpace(StandardReference) ? StandardReference : StandardCode > 0 ? StandardCode.ToString() : string.Empty;
    }
}
