using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificatePrintRequestHandler : IRequestHandler<UpdateCertificatePrintRequestCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificatePrintRequestHandler> _logger;

        public UpdateCertificatePrintRequestHandler(IMediator mediator, ICertificateRepository certificateRepository, ILogger<UpdateCertificatePrintRequestHandler> logger)
        {
            _mediator = mediator;
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCertificatePrintRequestCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _mediator.Send(new GetCertificateRequest(request.CertificateId, false), cancellationToken)
                ?? throw new NotFoundException();

            if (!string.Equals(certificate.LatestEPAOutcome, EpaOutcome.Pass, StringComparison.InvariantCultureIgnoreCase) ||
                !string.Equals(certificate.Status, CertificateStatus.Submitted, StringComparison.InvariantCultureIgnoreCase) ||
                certificate.PrintRequestedAt != null)
            {
                throw new ArgumentException("Certificate is not in a valid state to request print");
            }

            await _certificateRepository.UpdatePrintRequested(certificate.Id, request.Address, request.PrintRequestedAt, request.PrintRequestedBy);

            return Unit.Value;
        }
    }
}
