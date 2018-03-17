using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateHandler : IRequestHandler<UpdateCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;

        public UpdateCertificateHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.Update(request.Certificate);
        }
    }
}