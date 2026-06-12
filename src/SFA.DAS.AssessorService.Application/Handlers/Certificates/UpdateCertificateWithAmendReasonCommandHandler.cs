using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateWithAmendReasonCommandHandler : IRequestHandler<UpdateCertificateWithAmendReasonCommand, Unit>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateWithAmendReasonCommandHandler> _logger;

        public UpdateCertificateWithAmendReasonCommandHandler(ICertificateRepository certificateRepository, 
            ILogger<UpdateCertificateWithAmendReasonCommandHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCertificateWithAmendReasonCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate<Certificate>(request.CertificateReference) as Certificate;
            if (certificate == null)
                throw new NotFoundException();

            try
            {
                certificate.CertificateData.AmendReasons = AmendReasonsToList(request.Reasons);
                certificate.CertificateData.IncidentNumber = request.IncidentNumber;

                await _certificateRepository.UpdateStandardCertificate(certificate, request.Username, CertificateActions.AmendReason, true, request.OtherReason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to update certificate {request.CertificateReference} with amend reason");
                throw;
            }

            return Unit.Value;
        }

        private List<string> AmendReasonsToList(AmendReasons? amendReasons)
        {
            return amendReasons != null
                ? amendReasons.ToString().Split(',').Select(p => p.Trim()).ToList()
                : new List<string>();
        }
    }
}