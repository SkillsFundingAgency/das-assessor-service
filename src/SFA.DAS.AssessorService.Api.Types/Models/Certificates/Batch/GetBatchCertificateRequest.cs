using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class GetBatchCertificateRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }
        public string CertificateReference { get; set; }
        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}
