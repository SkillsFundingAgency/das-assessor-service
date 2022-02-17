using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateWithAmendReasonCommandHandler : IRequestHandler<UpdateCertificateWithAmendReasonCommand>
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
            var certificate = await _certificateRepository.GetCertificate(request.CertificateReference);
            if (certificate == null)
                throw new NotFoundException();

            try
            {
                var certificateData = JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
                certificateData.AmendReasons = AmendReasonsToList(request.Reasons);
                certificateData.IncidentNumber = request.IncidentNumber;
                certificate.CertificateData = JsonConvert.SerializeObject(certificateData);

                await _certificateRepository.Update(certificate, request.Username, CertificateActions.AmendReason, true, request.OtherReason);
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