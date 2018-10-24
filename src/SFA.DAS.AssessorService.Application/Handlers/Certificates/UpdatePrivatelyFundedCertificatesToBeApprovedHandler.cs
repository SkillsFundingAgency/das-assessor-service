using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdatePrivatelyFundedCertificatesToBeApprovedHandler : IRequestHandler<UpdateCertificateRequestToBeApproved>
    {
        private readonly ICertificateRepository _certificateRepository;

        public UpdatePrivatelyFundedCertificatesToBeApprovedHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task Handle(UpdateCertificateRequestToBeApproved request, CancellationToken cancellationToken)
        {
            await _certificateRepository.UpdatePrivatelyFundedCertificatesToBeApproved();
        }
    }
}