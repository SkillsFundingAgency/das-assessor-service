using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates;
using SFA.DAS.AssessorService.Application.Events;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi._HelperClasses;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ExternalApi.Certificates
{
    public class SubmitBatchCertificateHandler : IRequestHandler<SubmitBatchCertificateRequest, Certificate>
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IOutcomeRecordedEventPublisher _outcomeRecordedEventPublisher;
        private readonly ILogger<SubmitBatchCertificateHandler> _logger;

        public SubmitBatchCertificateHandler(
            ICertificateRepository certificateRepository,
            IContactQueryRepository contactQueryRepository,
            IOutcomeRecordedEventPublisher outcomeRecordedEventPublisher,
            ILogger<SubmitBatchCertificateHandler> logger)
        {
            _certificateRepository = certificateRepository;
            _contactQueryRepository = contactQueryRepository;
            _outcomeRecordedEventPublisher = outcomeRecordedEventPublisher;
            _logger = logger;
        }

        public async Task<Certificate> Handle(SubmitBatchCertificateRequest request, CancellationToken cancellationToken)
        {
            return await SubmitCertificate(request);
        }

        private async Task<Certificate> SubmitCertificate(SubmitBatchCertificateRequest request)
        {
            _logger.LogInformation("SubmitCertificate Before Get Certificate from db");
            var certificate = await _certificateRepository.GetCertificate(request.Uln, request.StandardCode);

            if (certificate != null)
            {
                _logger.LogInformation("SubmitCertificate Before Update Certificate Status");
                certificate.Status = CertificateStatus.Submitted;

                _logger.LogInformation("SubmitCertificate Before Update Cert in db");
                var submittedCertificate = await _certificateRepository.UpdateStandardCertificate(certificate, ExternalApiConstants.ApiUserName, CertificateActions.Submit);

                await _outcomeRecordedEventPublisher.PublishIfOutcomeRecorded(submittedCertificate, CertificateActions.Submit);

                return await CertificateHelpers.ApplyStatusInformation(_certificateRepository, _contactQueryRepository, submittedCertificate);
            }
            else
            {
                _logger.LogWarning($"SubmitCertificate Did not find Certificate for Uln {request.Uln} and StandardCode {request.StandardCode}");
                return null;
            }
        }
    }
}
