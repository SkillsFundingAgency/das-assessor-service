using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Logging;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class UpdateCertificateHandler : IRequestHandler<UpdateCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ILogger<UpdateCertificateHandler> _logger;

        public UpdateCertificateHandler(ICertificateRepository certificateRepository, ILogger<UpdateCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _logger = logger;
        }

        public async Task<Certificate> Handle(UpdateCertificateRequest request, CancellationToken cancellationToken)
        {
            if (request.Certificate.Status == Domain.Consts.CertificateStatus.Submitted)
            {
                _logger.LogInformation(LoggingConstants.CertificateSubmitted);
                _logger.LogInformation($"Certificate with ID: {request.Certificate.Id} Submitted with reference of {request.Certificate.CertificateReference}");
            }
            return await _certificateRepository.Update(request.Certificate, request.Username, request.Action);
        }
    }
}