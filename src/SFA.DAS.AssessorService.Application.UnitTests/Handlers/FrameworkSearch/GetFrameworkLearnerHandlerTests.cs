using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Application.UnitTests
    .Handlers.FrameworkSearch
{
    [TestFixture]
    public class GetFrameworkLearnerHandlerTests
    {
        private const int BatchNumber = 42;
        private const string CertificateReference = "00123456";

        private static readonly DateTime RequestedAt =
            new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc);

        private static readonly DateTime BatchStatusAt =
            new DateTime(2026, 11, 2, 10, 0, 0, DateTimeKind.Utc);

        private static readonly DateTime LastUpdatedAt =
            new DateTime(2026, 11, 3, 11, 0, 0, DateTimeKind.Utc);

        private Mock<IMapper> _mapper;
        private Mock<IFrameworkLearnerRepository> _frameworkLearnerRepository;
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IStaffCertificateRepository> _staffCertificateRepository;

        private GetFrameworkLearnerHandler _sut;
        private GetFrameworkLearnerRequest _request;
        private FrameworkLearner _learner;
        private FrameworkCertificate _certificate;
        private CertificateBatchLog _batchLog;
        private GetFrameworkLearnerResponse _mappedResponse;

        [SetUp]
        public void SetUp()
        {
            _mapper = new Mock<IMapper>();
            _frameworkLearnerRepository =
                new Mock<IFrameworkLearnerRepository>();
            _certificateRepository =
                new Mock<ICertificateRepository>();
            _staffCertificateRepository =
                new Mock<IStaffCertificateRepository>();

            _request = new GetFrameworkLearnerRequest(
                Guid.NewGuid(),
                allLogs: false);

            _learner = new FrameworkLearner
            {
                Id = _request.Id,
                ApprenticeForename = "Casey",
                ApprenticeSurname = "Example",
                FrameworkName = "Example framework"
            };

            _mappedResponse = new GetFrameworkLearnerResponse
            {
                Id = _learner.Id,
                ApprenticeForename = _learner.ApprenticeForename,
                ApprenticeSurname = _learner.ApprenticeSurname,
                FrameworkName = _learner.FrameworkName
            };

            _batchLog = new CertificateBatchLog
            {
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = BatchStatusAt,
                ReasonForChange = "Delivery confirmed"
            };

            _certificate = new FrameworkCertificate
            {
                Id = Guid.NewGuid(),
                FrameworkLearnerId = _learner.Id,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                PrintRequestedAt = RequestedAt,
                PrintRequestedBy = "Admin user",
                BatchNumber = BatchNumber,
                CertificateBatchLog = _batchLog,
                CreatedAt = RequestedAt.AddDays(-10),
                UpdatedAt = LastUpdatedAt,
                CertificateData = new CertificateData
                {
                    FullName = "Casey Example"
                }
            };

            _frameworkLearnerRepository
                .Setup(x => x.GetFrameworkLearner(_request.Id))
                .ReturnsAsync(() => _learner);

            _certificateRepository
                .Setup(x => x.GetFrameworkCertificate(_request.Id))
                .ReturnsAsync(() => _certificate);

            _mapper
                .Setup(x => x.Map<GetFrameworkLearnerResponse>(
                    It.IsAny<object>()))
                .Returns((object source) =>
                    source == null ? null : _mappedResponse);

            _staffCertificateRepository
                .Setup(x => x.GetAllCertificateLogs(
                    It.IsAny<Guid>()))
                .ReturnsAsync(new List<CertificateLogSummary>());

            _staffCertificateRepository
                .Setup(x => x.GetLatestCertificateLogs(
                    It.IsAny<Guid>(), 3))
                .ReturnsAsync(new List<CertificateLogSummary>());

            _sut = new GetFrameworkLearnerHandler(
                _mapper.Object,
                _frameworkLearnerRepository.Object,
                _certificateRepository.Object,
                _staffCertificateRepository.Object);
        }

        [Test]
        public async Task WhenCalled_ThenLearnerIsRetrievedAndMapped()
        {
            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            _frameworkLearnerRepository.Verify(
                x => x.GetFrameworkLearner(_request.Id),
                Times.Once);

            _certificateRepository.Verify(
                x => x.GetFrameworkCertificate(_request.Id),
                Times.Once);

            _mapper.Verify(
                x => x.Map<GetFrameworkLearnerResponse>(_learner),
                Times.Once);

            result.Id.Should().Be(_learner.Id);
            result.ApprenticeForename.Should()
                .Be(_learner.ApprenticeForename);
            result.ApprenticeSurname.Should()
                .Be(_learner.ApprenticeSurname);
            result.FrameworkName.Should()
                .Be(_learner.FrameworkName);
        }

        [Test]
        public async Task WhenLearnerAndCertificateAreMissing_ThenReturnsNull()
        {
            // Arrange
            _learner = null;
            _certificate = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.Should().BeNull();

            _staffCertificateRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task WhenCertificateIsMissing_ThenCertificateFieldsRemainNull()
        {
            // Arrange
            _certificate = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(_learner.Id);
            result.ApprenticeForename.Should()
                .Be(_learner.ApprenticeForename);
            result.ApprenticeSurname.Should()
                .Be(_learner.ApprenticeSurname);

            result.CertificateReference.Should().BeNull();
            result.CertificateStatus.Should().BeNull();
            result.CertificatePrintRequestedAt.Should().BeNull();
            result.CertificatePrintRequestedBy.Should().BeNull();
            result.CertificatePrintStatus.Should().BeNull();
            result.CertificatePrintStatusAt.Should().BeNull();
            result.CertificatePrintReasonForChange.Should().BeNull();
            result.CertificateLastUpdatedAt.Should().BeNull();
            result.CertificateLogs.Should().BeNull();

            _staffCertificateRepository.VerifyNoOtherCalls();
        }

        [TestCase(CertificateStatus.SentToPrinter)]
        [TestCase(CertificateStatus.Printed)]
        [TestCase(CertificateStatus.Delivered)]
        [TestCase(CertificateStatus.NotDelivered)]
        public async Task WhenCurrentBatchExists_ThenCertificateAndPrintFieldsAreMapped(
            string batchStatus)
        {
            // Arrange
            _batchLog.Status = batchStatus;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateReference.Should()
                .Be(CertificateReference);

            // Certificate status and batch status are separate values.
            result.CertificateStatus.Should()
                .Be(CertificateStatus.Delivered);

            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintRequestedBy.Should()
                .Be("Admin user");

            result.CertificatePrintStatus.Should()
                .Be(batchStatus);
            result.CertificatePrintStatusAt.Should()
                .Be(BatchStatusAt);
            result.CertificatePrintReasonForChange.Should()
                .Be("Delivery confirmed");

            result.CertificateLastUpdatedAt.Should()
                .Be(LastUpdatedAt);
        }

        [Test]
        public async Task WhenReprintAwaitsBatch_ThenRequestFieldsAreReturnedWithoutPrintStatus()
        {
            // Arrange
            _certificate.Status = CertificateStatus.Reprint;
            _certificate.BatchNumber = null;
            _certificate.CertificateBatchLog = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateStatus.Should()
                .Be(CertificateStatus.Reprint);
            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintRequestedBy.Should()
                .Be("Admin user");
            result.CertificatePrintStatus.Should().BeNull();
            result.CertificatePrintStatusAt.Should().BeNull();
            result.CertificatePrintReasonForChange.Should().BeNull();
        }

        [Test]
        public async Task WhenRequestDateIsMissing_ThenItIsNotInferredFromBatchOrUpdateDates()
        {
            // Arrange
            _certificate.PrintRequestedAt = null;
            _certificate.PrintRequestedBy = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificatePrintRequestedAt.Should().BeNull();
            result.CertificatePrintRequestedBy.Should().BeNull();

            result.CertificatePrintStatus.Should()
                .Be(CertificateStatus.Delivered);
            result.CertificatePrintStatusAt.Should()
                .Be(BatchStatusAt);
            result.CertificateLastUpdatedAt.Should()
                .Be(LastUpdatedAt);
        }

        [TestCase(null)]
        [TestCase(43)]
        public async Task WhenBatchIsClearedOrDoesNotMatch_ThenPreviousBatchDetailsAreNotReturned(
            int? currentBatchNumber)
        {
            // Arrange
            _certificate.BatchNumber = currentBatchNumber;

            // Keep the previous navigation object to exercise the guard.
            _certificate.CertificateBatchLog = _batchLog;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateStatus.Should()
                .Be(CertificateStatus.Delivered);
            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintRequestedBy.Should()
                .Be("Admin user");

            result.CertificatePrintStatus.Should().BeNull();
            result.CertificatePrintStatusAt.Should().BeNull();
            result.CertificatePrintReasonForChange.Should().BeNull();
        }

        [Test]
        public async Task WhenBatchNavigationIsMissing_ThenPrintFieldsAreEmpty()
        {
            // Arrange
            _certificate.CertificateBatchLog = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintStatus.Should().BeNull();
            result.CertificatePrintStatusAt.Should().BeNull();
            result.CertificatePrintReasonForChange.Should().BeNull();
        }

        [Test]
        public async Task WhenCertificateHasNotBeenUpdated_ThenLastUpdatedUsesCreatedDate()
        {
            // Arrange
            _certificate.UpdatedAt = null;
            _certificate.DeletedAt = null;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateLastUpdatedAt.Should()
                .Be(_certificate.CreatedAt);

            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintStatusAt.Should()
                .Be(BatchStatusAt);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task WhenLogsAreRequested_ThenCorrectRepositoryMethodAndLogsAreUsed(
            bool allLogs)
        {
            // Arrange
            _request.AllLogs = allLogs;

            var allCertificateLogs = new List<CertificateLogSummary>
            {
                CreateLog(BatchStatusAt, "All records", "Employer A")
            };

            var latestCertificateLogs = new List<CertificateLogSummary>
            {
                CreateLog(RequestedAt, "Latest records", "Employer B")
            };

            _staffCertificateRepository
                .Setup(x => x.GetAllCertificateLogs(_certificate.Id))
                .ReturnsAsync(allCertificateLogs);

            _staffCertificateRepository
                .Setup(x => x.GetLatestCertificateLogs(
                    _certificate.Id, 3))
                .ReturnsAsync(latestCertificateLogs);

            var expectedLogs = allLogs
                ? allCertificateLogs
                : latestCertificateLogs;

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateLogs.Should()
                .BeEquivalentTo(expectedLogs);

            _staffCertificateRepository.Verify(
                x => x.GetAllCertificateLogs(_certificate.Id),
                allLogs ? Times.Once() : Times.Never());

            _staffCertificateRepository.Verify(
                x => x.GetLatestCertificateLogs(_certificate.Id, 3),
                allLogs ? Times.Never() : Times.Once());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task WhenRepositoryReturnsNullLogs_ThenResponseContainsAnEmptyList(
            bool allLogs)
        {
            // Arrange
            _request.AllLogs = allLogs;

            _staffCertificateRepository
                .Setup(x => x.GetAllCertificateLogs(_certificate.Id))
                .ReturnsAsync((List<CertificateLogSummary>)null);

            _staffCertificateRepository
                .Setup(x => x.GetLatestCertificateLogs(
                    _certificate.Id, 3))
                .ReturnsAsync((List<CertificateLogSummary>)null);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateLogs.Should().NotBeNull();
            result.CertificateLogs.Should().BeEmpty();

            result.CertificatePrintRequestedAt.Should()
                .Be(RequestedAt);
            result.CertificatePrintStatus.Should()
                .Be(CertificateStatus.Delivered);
            result.CertificatePrintStatusAt.Should()
                .Be(BatchStatusAt);
        }

        [Test]
        public async Task WhenMultipleLogsAreReturned_ThenDifferencesAreCalculated()
        {
            // Arrange
            _request.AllLogs = true;

            // Repository order is newest first.
            var logs = new List<CertificateLogSummary>
            {
                CreateLog(
                    BatchStatusAt,
                    "Latest name",
                    "Latest employer"),
                CreateLog(
                    BatchStatusAt.AddDays(-1),
                    "Previous name",
                    "Previous employer"),
                CreateLog(
                    BatchStatusAt.AddDays(-2),
                    "Original name",
                    "Original employer")
            };

            _staffCertificateRepository
                .Setup(x => x.GetAllCertificateLogs(_certificate.Id))
                .ReturnsAsync(logs);

            // Act
            var result = await _sut.Handle(
                _request,
                CancellationToken.None);

            // Assert
            result.CertificateLogs.Should().HaveCount(3);

            result.CertificateLogs
                .Select(x => x.EventTime)
                .Should()
                .Equal(logs.Select(x => x.EventTime));

            result.CertificateLogs[0].DifferencesToPrevious
                .Should().NotBeEmpty();

            result.CertificateLogs[0].DifferencesToPrevious
                .SelectMany(x => x.Values)
                .Should().Contain("Latest name");

            result.CertificateLogs[1].DifferencesToPrevious
                .Should().NotBeEmpty();

            result.CertificateLogs[2].DifferencesToPrevious
                .Should().BeEmpty();
        }

        private static CertificateLogSummary CreateLog(
            DateTime eventTime,
            string givenNames,
            string employerName)
        {
            return new CertificateLogSummary
            {
                EventTime = eventTime,
                Action = CertificateActions.Amend,
                ActionBy = "Admin user",
                ActionByEmail = "admin@example.com",
                Status = CertificateStatus.Submitted,
                CertificateData = JsonConvert.SerializeObject(
                    new CertificateData
                    {
                        LearnerGivenNames = givenNames,
                        LearnerFamilyName = "Example",
                        EmployerName = employerName,
                        StandardReference = "ST0001",
                        Version = "1.0"
                    })
            };
        }
    }
}