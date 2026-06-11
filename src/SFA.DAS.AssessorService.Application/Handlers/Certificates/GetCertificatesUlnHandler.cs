using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetCertificatesUlnHandler : IRequestHandler<GetCertificatesUlnRequest, GetCertificatesUlnResponse>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificatesUlnHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<GetCertificatesUlnResponse> Handle(GetCertificatesUlnRequest request, CancellationToken cancellationToken)
        {
            return new GetCertificatesUlnResponse
            {
                Certificates = await _certificateRepository.GetPrintableCertificates(request.Uln)
            };
        }
    }
}