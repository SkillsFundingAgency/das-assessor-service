using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Enums;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates
{
    [TestFixture]
    public class WhenRequestingFrameworkCertificateReprint
    {
        private static DateTime RequestedAt =>
            new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);

        private Mock<ICertificateRepository> _repository;
        private Mock<IFrameworkLearnerRepository> _learnerRepository;
        private Mock<IDateTimeHelper> _dateTimeHelper;
        private ReprintFrameworkCertificateHandler _sut;

        private ReprintFrameworkCertificateRequest _request;
        private FrameworkLearner _learner;
        private FrameworkCertificate _certificate;
        private FrameworkCertificate _createdCertificate;
        private CertificateData _dataAtReprintRequest;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ICertificateRepository>();
            _learnerRepository = new Mock<IFrameworkLearnerRepository>();
            _dateTimeHelper = new Mock<IDateTimeHelper>();

            _request = new ReprintFrameworkCertificateRequest
            {
                FrameworkLearnerId = Guid.NewGuid(),
                Username = "Admin user",
                IncidentNumber = "INC-123",
                Reasons =
                    ReprintReasons.DeliveryFailed |
                    ReprintReasons.LostOrDamaged,
                ContactName = "New recipient",
                ContactAddLine1 = "10 New Road",
                ContactAddLine2 = "New village",
                ContactAddLine3 = "New town",
                ContactAddLine4 = "New county",
                ContactPostcode = "AB1 2CD"
            };

            _learner = new FrameworkLearner
            {
                Id = _request.FrameworkLearnerId,
                ApprenticeForename = "Casey",
                ApprenticeSurname = "Example",
                ApprenticeFullname = "Casey Middle Example",
                ApprenticeDoB = new DateTime(1990, 1, 1),
                ApprenticeStartdate = new DateTime(2009, 9, 1),
                CertificationDate = new DateTime(2012, 6, 1),
                TrainingCode = "444-2-1",
                FrameworkName = "Imported framework",
                PathwayName = "Imported pathway",
                ApprenticeshipLevelName = "Advanced",
                FrameworkCertificateNumber = "FW123456",
                Ukprn = "12345678"
            };

            _certificate = new FrameworkCertificate
            {
                Id = Guid.NewGuid(),
                FrameworkLearnerId = _request.FrameworkLearnerId,
                Status = CertificateStatus.Delivered,
                PrintRequestedAt = RequestedAt.AddMonths(-1),
                CertificateData = new CertificateData
                {
                    LearnerGivenNames = "Existing",
                    LearnerFamilyName = "Name",
                    FullName = "Existing Full Name",
                    FrameworkName = "Existing framework",
                    PathwayName = "Existing pathway",
                    FrameworkLevelName = "Advanced",
                    OverallGrade = CertificateGrade.Pass,
                    ContactName = "Previous recipient"
                }
            };

            _repository
                .Setup(x => x.GetFrameworkCertificate(
                    _request.FrameworkLearnerId))
                .ReturnsAsync(() => _certificate);

            _learnerRepository
                .Setup(x => x.GetFrameworkLearner(
                    _request.FrameworkLearnerId))
                .ReturnsAsync(() => _learner);

            _repository
                .Setup(x => x.UpdateReprintRequested<FrameworkCertificate>(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()))
                .Callback<Guid, DateTime, string>((id, date, user) =>
                {
                    _dataAtReprintRequest =
                        Clone(_certificate.CertificateData);
                })
                .ReturnsAsync(() => _certificate);

            _repository
                .Setup(x => x.NewFrameworkCertificate(
                    It.IsAny<FrameworkCertificate>()))
                .Callback<FrameworkCertificate>(certificate =>
                    _createdCertificate = certificate)
                .Returns((FrameworkCertificate certificate) =>
                    Task.FromResult(certificate));

            _dateTimeHelper
                .SetupGet(x => x.DateTimeUtcNow)
                .Returns(RequestedAt);

            _sut = new ReprintFrameworkCertificateHandler(
                _repository.Object,
                _learnerRepository.Object,
                _dateTimeHelper.Object,
                Mock.Of<ILogger<ReprintFrameworkCertificateHandler>>());
        }

        [Test]
        public async Task AndCertificateExistsThenDeliveryDetailsAreSavedBeforeRequestingReprint()
        {
            // Arrange
            var expectedData = Clone(_certificate.CertificateData);

            expectedData.ContactName = _request.ContactName;
            expectedData.ContactAddLine1 = _request.ContactAddLine1;
            expectedData.ContactAddLine2 = _request.ContactAddLine2;
            expectedData.ContactAddLine3 = _request.ContactAddLine3;
            expectedData.ContactAddLine4 = _request.ContactAddLine4;
            expectedData.ContactPostCode = _request.ContactPostcode;
            expectedData.IncidentNumber = _request.IncidentNumber;
            expectedData.ReprintReasons = new List<string>
            {
                "DeliveryFailed",
                "LostOrDamaged"
            };

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(_certificate);
            result.Status.Should().Be(CertificateStatus.Reprint);

            // Captured when the repository operation was invoked,
            // rather than only checking the entity afterwards.
            _dataAtReprintRequest.Should().BeEquivalentTo(expectedData);

            _repository.Verify(
                x => x.UpdateReprintRequested<FrameworkCertificate>(
                    _certificate.Id,
                    RequestedAt,
                    _request.Username),
                Times.Once);

            _repository.Verify(
                x => x.NewFrameworkCertificate(
                    It.IsAny<FrameworkCertificate>()),
                Times.Never);

            _learnerRepository.Verify(
                x => x.GetFrameworkLearner(It.IsAny<Guid>()),
                Times.Never);
        }

        [Test]
        public async Task AndCertificateDoesNotExistThenFirstReprintRecordsRequestDetails()
        {
            // Arrange
            _certificate = null;

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeSameAs(_createdCertificate);
            result.Status.Should().Be(CertificateStatus.Reprint);
            result.PrintRequestedAt.Should().Be(RequestedAt);
            result.PrintRequestedBy.Should().Be(_request.Username);
            result.CreatedBy.Should().Be(_request.Username);
            result.FrameworkLearnerId.Should()
                .Be(_request.FrameworkLearnerId);
            result.DateOfBirth.Should().Be(_learner.ApprenticeDoB);
            result.ProviderUkPrn.Should().Be(12345678);
            result.BatchNumber.Should().BeNull();
            result.ToBePrinted.Should().BeNull();
            result.CertificateReference.Should().BeEmpty();

            result.CertificateData.Should().BeEquivalentTo(
                new CertificateData
                {
                    TrainingCode = _learner.TrainingCode,
                    LearnerGivenNames = _learner.ApprenticeForename,
                    LearnerFamilyName = _learner.ApprenticeSurname,
                    FullName = _learner.ApprenticeFullname,
                    LearningStartDate = _learner.ApprenticeStartdate,
                    AchievementDate = _learner.CertificationDate,
                    OverallGrade = CertificateGrade.Pass,
                    SendTo = CertificateSendTo.Apprentice,
                    FrameworkName = _learner.FrameworkName,
                    PathwayName = _learner.PathwayName,
                    FrameworkLevelName = _learner.ApprenticeshipLevelName,
                    FrameworkCertificateNumber =
                        _learner.FrameworkCertificateNumber,
                    IncidentNumber = _request.IncidentNumber,
                    ReprintReasons = new List<string>
                    {
                        "DeliveryFailed",
                        "LostOrDamaged"
                    },
                    ContactName = _request.ContactName,
                    ContactAddLine1 = _request.ContactAddLine1,
                    ContactAddLine2 = _request.ContactAddLine2,
                    ContactAddLine3 = _request.ContactAddLine3,
                    ContactAddLine4 = _request.ContactAddLine4,
                    ContactPostCode = _request.ContactPostcode,
                    EpaDetails = new EpaDetails
                    {
                        Epas = new List<EpaRecord>()
                    }
                });

            _repository.Verify(
                x => x.NewFrameworkCertificate(
                    It.IsAny<FrameworkCertificate>()),
                Times.Once);

            VerifyNoExistingCertificateReprint();
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task AndReasonsAreMissingThenAnEmptyReasonsListIsUsed(
            bool firstReprint)
        {
            // Arrange
            _request.Reasons = null;

            if (firstReprint)
                _certificate = null;

            // Act
            await _sut.Handle(_request, CancellationToken.None);

            // Assert
            var data = firstReprint
                ? _createdCertificate.CertificateData
                : _dataAtReprintRequest;

            data.ReprintReasons.Should().NotBeNull();
            data.ReprintReasons.Should().BeEmpty();
        }

        [Test]
        public async Task AndLearnerAndCertificateAreMissingThenNothingIsCreated()
        {
            // Arrange
            _certificate = null;
            _learner = null;

            // Act
            var result = await _sut.Handle(
                _request, CancellationToken.None);

            // Assert
            result.Should().BeNull();

            _repository.Verify(
                x => x.NewFrameworkCertificate(
                    It.IsAny<FrameworkCertificate>()),
                Times.Never);

            VerifyNoExistingCertificateReprint();

            _dateTimeHelper.VerifyGet(
                x => x.DateTimeUtcNow,
                Times.Never);
        }

        private void VerifyNoExistingCertificateReprint()
        {
            _repository.Verify(
                x => x.UpdateReprintRequested<FrameworkCertificate>(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        private static CertificateData Clone(CertificateData data)
        {
            return JsonConvert.DeserializeObject<CertificateData>(
                JsonConvert.SerializeObject(data));
        }
    }
}