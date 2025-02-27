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
    public class UpdateCertificateWithReprintReasonCommandHandler : IRequestHandler<UpdateCertificateWithReprintReasonCommand, Unit>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateWithReprintReasonCommandHandler> _logger;

        public UpdateCertificateWithReprintReasonCommandHandler(ICertificateRepository certificateRepository, 
            ILogger<UpdateCertificateWithReprintReasonCommandHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateCertificateWithReprintReasonCommand request, CancellationToken cancellationToken)
        {
            var certificate = await _certificateRepository.GetCertificate(request.CertificateReference) as Certificate;
            if (certificate == null)
                throw new NotFoundException();

            try
            {
                certificate.CertificateData.ReprintReasons = ReprintReasonsToList(request.Reasons);
                certificate.CertificateData.IncidentNumber = request.IncidentNumber;

                await _certificateRepository.UpdateStandardCertificate(certificate, request.Username, CertificateActions.ReprintReason, true, request.OtherReason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to update certificate {request.CertificateReference} with reprint reason");
                throw;
            }

            return Unit.Value;
        }

        private List<string> ReprintReasonsToList(ReprintReasons? reprintReasons)
        {
            return reprintReasons != null
                ? reprintReasons.ToString().Split(',').Select(p => p.Trim()).ToList()
                : new List<string>();
        }
    }
}