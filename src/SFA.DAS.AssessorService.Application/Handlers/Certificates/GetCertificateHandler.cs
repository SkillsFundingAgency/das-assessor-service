using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Handlers.Certificates
{
    public class GetCertificateHandler : IRequestHandler<GetCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GetCertificateHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(GetCertificateRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.GetCertificate<Certificate>(request.CertificateId, request.IncludeLogs);
        }
    }
}