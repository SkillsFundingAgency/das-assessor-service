using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Messages.Events;

namespace SFA.DAS.AssessorService.Application.Events
{
    public class OutcomeRecordedEventPublisher : IOutcomeRecordedEventPublisher
    {
        private readonly IMessageSession _messageSession;
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<OutcomeRecordedEventPublisher> _logger;

        public OutcomeRecordedEventPublisher(
            IMessageSession messageSession,
            ILearnerRepository learnerRepository,
            ILogger<OutcomeRecordedEventPublisher> logger)
        {
            _messageSession = messageSession;
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task PublishIfOutcomeRecorded(Certificate updated, string action)
        {
            if (updated == null || action != CertificateActions.Submit || updated.Status != CertificateStatus.Submitted)
            {
                return;
            }

            var epaDetails = updated.CertificateData?.EpaDetails;
            if (!TryMapOutcome(epaDetails?.LatestEpaOutcome, out var outcome))
            {
                _logger.LogInformation("OutcomeRecordedEvent skipped for certificate {CertificateId}: unsupported outcome {Outcome}", updated.Id, epaDetails?.LatestEpaOutcome);
                return;
            }

            var achievementDate = updated.CertificateData.AchievementDate;
            if (achievementDate == null)
            {
                _logger.LogWarning("OutcomeRecordedEvent skipped for certificate {CertificateId}: no achievement date", updated.Id);
                return;
            }

            var learner = await _learnerRepository.Get(updated.Uln, updated.StandardCode);
            if (learner?.ApprenticeshipId == null)
            {
                _logger.LogInformation("OutcomeRecordedEvent skipped for certificate {CertificateId}: no learner apprenticeship id", updated.Id);
                return;
            }

            _logger.LogInformation("Publishing OutcomeRecordedEvent for certificate {CertificateId}, apprenticeship {ApprenticeshipId}, outcome {Outcome}",
                updated.Id, learner.ApprenticeshipId, outcome);

            await _messageSession.Publish(new OutcomeRecordedEvent
            {
                ApprenticeshipId = learner.ApprenticeshipId.Value,
                AchievementDate = achievementDate.Value,
                Outcome = outcome,
                StandardName = updated.CertificateData.StandardName,
                CertificateId = updated.Id
            });
        }

        private static bool TryMapOutcome(string latestEpaOutcome, out OutcomeType outcome)
        {
            if (string.Equals(latestEpaOutcome, EpaOutcome.Pass, StringComparison.OrdinalIgnoreCase))
            {
                outcome = OutcomeType.Pass;
                return true;
            }

            if (string.Equals(latestEpaOutcome, EpaOutcome.Fail, StringComparison.OrdinalIgnoreCase))
            {
                outcome = OutcomeType.Fail;
                return true;
            }

            outcome = default;
            return false;
        }
    }
}
