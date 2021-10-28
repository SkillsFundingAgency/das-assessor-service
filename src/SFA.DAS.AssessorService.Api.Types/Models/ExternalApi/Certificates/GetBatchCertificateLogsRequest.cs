using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates
{
    public class GetBatchCertificateLogsRequest : IRequest<GetBatchCertificateLogsResponse>
    {
        public string CertificateReference { get; set; }
    }
}
