using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatesBatchToIndicatePrintedHandler : IRequestHandler<UpdateCertificatesBatchToIndicatePrintedRequest>
    {
        private readonly ICertificateRepository _certificateRepository;

        public UpdateCertificatesBatchToIndicatePrintedHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task Handle(UpdateCertificatesBatchToIndicatePrintedRequest updateCertificatesBatchToIndicatePrintedRequest, CancellationToken cancellationToken)
        {
            await _certificateRepository.UpdateStatuses(updateCertificatesBatchToIndicatePrintedRequest);
        }
    } 
}