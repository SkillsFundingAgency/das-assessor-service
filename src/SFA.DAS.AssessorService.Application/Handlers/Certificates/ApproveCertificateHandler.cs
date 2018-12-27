using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class ApproveCertificateHandler : IRequestHandler<CertificateApprovalRequest>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateHandler> _logger;

        public ApproveCertificateHandler(ICertificateRepository certificateRepository,
            ILogger<UpdateCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task Handle(CertificateApprovalRequest request, CancellationToken cancellationToken)
        {
            await _certificateRepository.ApproveCertificates(request.ApprovalResults.ToList(), request.userName, request.ActionHint);
        }
    }
}