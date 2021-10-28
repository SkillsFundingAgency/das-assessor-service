using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class GetBatchCertificateLogsHandler : IRequestHandler<GetBatchCertificateLogsRequest, GetBatchCertificateLogsResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetBatchCertificateLogsHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<GetBatchCertificateLogsResponse> Handle(GetBatchCertificateLogsRequest request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.CertificateReference);

            var logs = await _certificateRepository.GetCertificateLogsFor(certificate.Id);

            return new GetBatchCertificateLogsResponse
            {
                CertificateLogs = logs.Select(log => new BatchCertificateLog
                {
                    Status = log.Status,
                    EventTime = log.EventTime
                })
            };
        }
    }
}
