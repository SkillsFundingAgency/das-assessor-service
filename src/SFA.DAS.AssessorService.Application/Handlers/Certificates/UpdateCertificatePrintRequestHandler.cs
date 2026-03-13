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
            var certificate = await _mediator.Send(new GetCertificateRequest(request.CertificateId, false), cancellationToken);

            if (certificate == null)
            {
                throw new NotFoundException();
            }

            if (!string.Equals(certificate.LatestEPAOutcome, EpaOutcome.Pass, StringComparison.InvariantCultureIgnoreCase) ||
                !string.Equals(certificate.Status, CertificateStatus.Submitted, StringComparison.InvariantCultureIgnoreCase) ||
                certificate.PrintRequestedAt != null)
            {
                throw new ArgumentException("Certificate is not in a valid state to request print");
            }

            certificate.CertificateData.ContactName = request.Address.ContactName;
            certificate.CertificateData.ContactOrganisation = request.Address.ContactOrganisation;
            certificate.CertificateData.ContactAddLine1 = request.Address.ContactAddLine1;
            certificate.CertificateData.ContactAddLine2 = request.Address.ContactAddLine2;
            certificate.CertificateData.ContactAddLine3 = request.Address.ContactAddLine3;
            certificate.CertificateData.ContactAddLine4 = request.Address.ContactAddLine4;
            certificate.CertificateData.ContactPostCode = request.Address.ContactPostCode;

            certificate.PrintRequestedAt = request.PrintRequestedAt;
            certificate.PrintRequestedBy = request.PrintRequestedBy;
            certificate.Status = CertificateStatus.PrintRequested;

            var actor = certificate.CertificateData?.ContactName ?? request.Address?.ContactName ?? string.Empty;

            _logger.LogInformation("Print requested for certificate {CertificateId} by {Actor}", certificate.Id, actor);

            await _certificateRepository.UpdateStandardCertificate(certificate, actor, "PrintRequest", updateLog: true);

            return Unit.Value;
        }
    }
}
