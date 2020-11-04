using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesForBatchNumberHandler : IRequestHandler<GetCertificatesForBatchNumberRequest, CertificatesForBatchNumberResponse>
    {
        private readonly ICertificateRepository _certificateRepository;
        
        public GetCertificatesForBatchNumberHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<CertificatesForBatchNumberResponse> Handle(GetCertificatesForBatchNumberRequest request, CancellationToken cancellationToken)
        {
            var certificates = await _certificateRepository.GetCertificatesForBatch(request.BatchNumber);
            
            var certificatesToBePrintedResponse = new CertificatesForBatchNumberResponse()
            {
                Certificates = certificates
            };

            return certificatesToBePrintedResponse;
        }
    }
}