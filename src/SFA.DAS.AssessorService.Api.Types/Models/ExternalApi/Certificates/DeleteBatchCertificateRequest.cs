using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates
{
    public class DeleteBatchCertificateRequest : IRequest<Unit>
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }

        public int StandardCode { get; set; }
        public string StandardReference { get; set; } // Note: Not used at the moment

        public string CertificateReference { get; set; }
        public int UkPrn { get; set; }
    }
}
