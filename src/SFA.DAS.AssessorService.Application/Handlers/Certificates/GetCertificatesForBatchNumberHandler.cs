using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Handlers.Certificates
{
    public class GetCertificatesForBatchNumberHandler : IRequestHandler<GetCertificatesForBatchNumberRequest, CertificatesForBatchNumberResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;
        
        public GetCertificatesForBatchNumberHandler(IBatchLogQueryRepository batchLogQueryRepository)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<CertificatesForBatchNumberResponse> Handle(GetCertificatesForBatchNumberRequest request, CancellationToken cancellationToken)
        {
            var certificates = await _batchLogQueryRepository.GetCertificatesForBatch(request.BatchNumber);
            
            var certificatesToBePrintedResponse = new CertificatesForBatchNumberResponse()
            {
                Certificates = certificates
            };

            return certificatesToBePrintedResponse;
        }
    }
}