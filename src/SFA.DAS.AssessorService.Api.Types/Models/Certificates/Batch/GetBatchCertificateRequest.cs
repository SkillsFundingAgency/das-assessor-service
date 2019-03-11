using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class GetBatchCertificateRequest : IRequest<Certificate>
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }

        public int StandardCode { get; set; }
        public string StandardReference { get; set; } // Note: Not used at the moment

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}
