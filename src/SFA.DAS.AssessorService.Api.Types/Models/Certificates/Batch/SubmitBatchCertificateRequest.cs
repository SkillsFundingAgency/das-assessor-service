using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates.Batch
{
    public class SubmitBatchCertificateRequest : IRequest<Certificate> // TODO: Figure out what the response type should be!
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string FamilyName { get; set; }

        public int UkPrn { get; set; }
        public string Username { get; set; }
    }
}
