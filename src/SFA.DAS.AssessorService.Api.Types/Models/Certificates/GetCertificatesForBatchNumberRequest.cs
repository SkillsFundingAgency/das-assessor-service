using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetCertificatesForBatchNumberRequest : IRequest<CertificatesForBatchNumberResponse>
    {
        public int BatchNumber { get; set; }
    }
}
