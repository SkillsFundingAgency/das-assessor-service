using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesForBatchNumberHandler : IRequestHandler<GetCertificatesForBatchNumberRequest, CertificatesForBatchNumberResponse>
    {
        private readonly ICertificateBatchLogRepository _certificateBatchLogRepository;
        
        public GetCertificatesForBatchNumberHandler(ICertificateBatchLogRepository certificateBatchLogRepository)
        {
            _certificateBatchLogRepository = certificateBatchLogRepository;
        }

        public async Task<CertificatesForBatchNumberResponse> Handle(GetCertificatesForBatchNumberRequest request, CancellationToken cancellationToken)
        {
            var certificates = await _certificateBatchLogRepository.GetCertificatesForBatch(request.BatchNumber);
            
            var certificatesToBePrintedResponse = new CertificatesForBatchNumberResponse()
            {
                Certificates = certificates
            };

            return certificatesToBePrintedResponse;
        }
    }
}