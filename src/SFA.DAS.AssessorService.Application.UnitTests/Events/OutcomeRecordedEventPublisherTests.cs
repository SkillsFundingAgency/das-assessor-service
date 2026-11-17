using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Events;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Messages.Events;

namespace SFA.DAS.AssessorService.Application.UnitTests.Events
{
    [TestFixture]
    public class OutcomeRecordedEventPublisherTests
    {
        private Mock<IMessageSession> _messageSession;
        private Mock<ILearnerRepository> _learnerRepository;
        private Mock<ILogger<OutcomeRecordedEventPublisher>> _logger;
        private OutcomeRecordedEventPublisher _sut;

        private const long Uln = 1234567890;
        private const int StandardCode = 10;
        private const long ApprenticeshipId = 999;

        [SetUp]
        public void Setup()
        {
            _messageSession = new Mock<IMessageSession>();
            _learnerRepository = new Mock<ILearnerRepository>();
            _logger = new Mock<ILogger<OutcomeRecordedEventPublisher>>();

            _learnerRepository
                .Setup(r => r.Get(Uln, StandardCode))
                .ReturnsAsync(new Learner { ApprenticeshipId = ApprenticeshipId });

            _sut = new OutcomeRecordedEventPublisher(_messageSession.Object, _learnerRepository.Object, _logger.Object);
        }

        private static Certificate BuildCertificate(string status, string epaOutcome, DateTime? achievementDate, string standardName = "Standard Name")
        {
            return new Certificate
            {
                Id = Guid.NewGuid(),
                Uln = Uln,
                StandardCode = StandardCode,
                Status = status,
                CertificateData = new CertificateData
                {
                    StandardName = standardName,
                    AchievementDate = achievementDate,
                    EpaDetails = new EpaDetails
                    {
                        LatestEpaOutcome = epaOutcome
                    }
                }
            };
        }

        [TestCase(EpaOutcome.Pass, OutcomeType.Pass)]
        [TestCase(EpaOutcome.Fail, OutcomeType.Fail)]
        public async Task WhenSubmittedWithSupportedOutcome_ThenEventIsPublished(string epaOutcome, OutcomeType expectedOutcome)
        {
            // Arrange
            var achievementDate = new DateTime(2024, 1, 1);
            var certificate = BuildCertificate(CertificateStatus.Submitted, epaOutcome, achievementDate);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.Is<OutcomeRecordedEvent>(e =>
                e.ApprenticeshipId == ApprenticeshipId &&
                e.AchievementDate == achievementDate &&
                e.Outcome == expectedOutcome &&
                e.StandardName == certificate.CertificateData.StandardName &&
                e.CertificateId == certificate.Id), It.IsAny<PublishOptions>()), Times.Once);
        }

        [Test]
        public async Task WhenSubmittedWithFailTwiceInARow_ThenEventIsPublishedEachTime()
        {
            // Arrange
            var achievementDate = new DateTime(2024, 1, 1);
            var certificate = BuildCertificate(CertificateStatus.Submitted, EpaOutcome.Fail, achievementDate);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Exactly(2));
        }

        [Test]
        public async Task WhenCertificateIsNull_ThenEventIsNotPublished()
        {
            // Act
            await _sut.PublishIfOutcomeRecorded(null, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenActionIsNotSubmit_ThenEventIsNotPublished()
        {
            // Arrange
            var certificate = BuildCertificate(CertificateStatus.Submitted, EpaOutcome.Pass, DateTime.UtcNow);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Start);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenStatusIsNotSubmitted_ThenEventIsNotPublished()
        {
            // Arrange
            var certificate = BuildCertificate(CertificateStatus.Draft, EpaOutcome.Pass, DateTime.UtcNow);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenOutcomeIsUnsupported_ThenEventIsNotPublished()
        {
            // Arrange
            var certificate = BuildCertificate(CertificateStatus.Submitted, "Withdrawn", DateTime.UtcNow);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenAchievementDateIsMissing_ThenEventIsNotPublished()
        {
            // Arrange
            var certificate = BuildCertificate(CertificateStatus.Submitted, EpaOutcome.Pass, null);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenLearnerHasNoApprenticeshipId_ThenEventIsNotPublished()
        {
            // Arrange
            _learnerRepository
                .Setup(r => r.Get(Uln, StandardCode))
                .ReturnsAsync(new Learner { ApprenticeshipId = null });

            var certificate = BuildCertificate(CertificateStatus.Submitted, EpaOutcome.Pass, DateTime.UtcNow);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }

        [Test]
        public async Task WhenLearnerIsNotFound_ThenEventIsNotPublished()
        {
            // Arrange
            _learnerRepository
                .Setup(r => r.Get(Uln, StandardCode))
                .ReturnsAsync((Learner)null);

            var certificate = BuildCertificate(CertificateStatus.Submitted, EpaOutcome.Pass, DateTime.UtcNow);

            // Act
            await _sut.PublishIfOutcomeRecorded(certificate, CertificateActions.Submit);

            // Assert
            _messageSession.Verify(m => m.Publish(It.IsAny<OutcomeRecordedEvent>(), It.IsAny<PublishOptions>()), Times.Never);
        }
    }
}
