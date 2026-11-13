using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    [TestFixture(typeof(Certificate))]
    [TestFixture(typeof(FrameworkCertificate))]
    public class WhenSystemUpdatesPrintedCertificate<T>
        where T : CertificateBase, new()
    {
        private const int BatchNumber = 42;
        private const string CertificateReference = "00123456";
        private const string RequestingUser = "Original requesting user";
        private const string UpdatingUser = "Updating user";

        private static DateTime RequestedAt =>
            new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);

        private static DateTime SentToPrinterAt =>
            new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc);

        private static DateTime DeliveredAt =>
            new DateTime(2026, 10, 5, 11, 0, 0, DateTimeKind.Utc);

        private Mock<IAssessorDbContext> _dbContext;
        private Mock<DbSet<CertificateLog>> _logSet;
        private List<CertificateLog> _logs;
        private List<CertificateBatchLog> _batchLogs;

        private CertificateRepository _sut;
        private T _certificate;
        private CertificateBatchLog _batchLog;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new Mock<IAssessorDbContext>();
            _logSet = new Mock<DbSet<CertificateLog>>();
            _logs = new List<CertificateLog>();

            var data = CreateCertificateData();

            _batchLog = new CertificateBatchLog
            {
                Id = Guid.NewGuid(),
                BatchNumber = BatchNumber,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = DeliveredAt,
                CertificateData = Clone(data)
            };

            _batchLogs = new List<CertificateBatchLog>
            {
                _batchLog
            };

            _certificate = new T
            {
                Id = Guid.NewGuid(),
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                PrintRequestedAt = RequestedAt,
                PrintRequestedBy = RequestingUser,
                BatchNumber = BatchNumber,
                ToBePrinted = SentToPrinterAt,
                CreatedAt = RequestedAt.AddMonths(-1),
                UpdatedAt = DeliveredAt,
                CertificateData = data,
                CertificateBatchLog = _batchLog
            };

            _dbContext
                .Setup(x => x.Set<T>())
                .ReturnsDbSet(new List<T> { _certificate });

            _dbContext
                .SetupGet(x => x.CertificateBatchLogs)
                .ReturnsDbSet(_batchLogs);

            _logSet
                .Setup(x => x.Add(It.IsAny<CertificateLog>()))
                .Callback<CertificateLog>(log => _logs.Add(log));

            _dbContext
                .SetupGet(x => x.CertificateLogs)
                .Returns(_logSet.Object);

            _dbContext
                .Setup(x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var unitOfWork = new Mock<IAssessorUnitOfWork>();
            unitOfWork
                .SetupGet(x => x.AssessorDbContext)
                .Returns(_dbContext.Object);

            var mockDateTimeHelper = new Mock<IDateTimeHelper>();
            mockDateTimeHelper
                .Setup(x => x.DateTimeUtcNow)
                .Returns(DateTime.UtcNow);

            _sut = new CertificateRepository(unitOfWork.Object, mockDateTimeHelper.Object);
        }

        [TestCaseSource(nameof(PrintedContentChanges))]
        public async Task ThenBatchIsClearedWhenPrintedContentChanges(
            string propertyName,
            object newValue)
        {
            // Arrange
            var updatedCertificate = Clone(_certificate);
            var originalBatch = Clone(_batchLog);

            SetValue(
                updatedCertificate.CertificateData,
                propertyName,
                newValue);

            var expectedData = Clone(updatedCertificate.CertificateData);

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchCleared(result);

            result.CertificateData.Should()
                .BeEquivalentTo(expectedData);

            _batchLog.Should().BeEquivalentTo(originalBatch);

            AssertSavedAndLogged(result, expectedBatchNumber: null);
        }

        [TestCaseSource(nameof(NonPrintedContentChanges))]
        public async Task ThenBatchIsPreservedWhenOnlyNonPrintedContentChanges(
            string propertyName,
            object newValue)
        {
            // Arrange
            var updatedCertificate = Clone(_certificate);
            var originalBatch = Clone(_batchLog);

            SetValue(
                updatedCertificate.CertificateData,
                propertyName,
                newValue);

            var expectedData = Clone(updatedCertificate.CertificateData);

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchPreserved(result);

            result.CertificateData.Should()
                .BeEquivalentTo(expectedData);

            _batchLog.Should().BeEquivalentTo(originalBatch);

            AssertSavedAndLogged(result, BatchNumber);
        }

        [Test]
        public async Task ThenBatchIsPreservedWhenCertificateDataIsUnchanged()
        {
            // Arrange
            var updatedCertificate = Clone(_certificate);
            var originalBatch = Clone(_batchLog);

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchPreserved(result);

            _batchLog.Should().BeEquivalentTo(originalBatch);

            AssertSavedAndLogged(result, BatchNumber);
        }

        [TestCaseSource(nameof(EquivalentPrintedContentChanges))]
        public async Task ThenBatchIsPreservedWhenRenderedContentIsEquivalent(
            string propertyName,
            object originalValue,
            object newValue)
        {
            // Arrange
            SetValue(
                _certificate.CertificateData,
                propertyName,
                originalValue);

            SetValue(
                _batchLog.CertificateData,
                propertyName,
                originalValue);

            var updatedCertificate = Clone(_certificate);
            var originalBatch = Clone(_batchLog);

            SetValue(
                updatedCertificate.CertificateData,
                propertyName,
                newValue);

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchPreserved(result);

            _batchLog.Should().BeEquivalentTo(originalBatch);

            AssertSavedAndLogged(result, BatchNumber);
        }

        [Test]
        public async Task ThenBatchIsClearedWhenSuppliedEntityAlreadyContainsChanges()
        {
            // Arrange
            var originalBatch = Clone(_batchLog);

            // This is the same instance returned by the mocked repository query.
            ChangePrintedName(_certificate.CertificateData);

            var expectedData = Clone(_certificate.CertificateData);

            // Act
            var result = await UpdateCertificate(_certificate);

            // Assert
            AssertBatchCleared(result);

            result.CertificateData.Should()
                .BeEquivalentTo(expectedData);

            _batchLog.Should().BeEquivalentTo(originalBatch);

            AssertSavedAndLogged(result, expectedBatchNumber: null);
        }

        [Test]
        public async Task ThenOnlyTheCurrentCertificatesCurrentBatchIsCompared()
        {
            // Arrange
            var updatedCertificate = Clone(_certificate);
            ChangePrintedName(updatedCertificate.CertificateData);

            var originalBatch = Clone(_batchLog);

            // These snapshots match the changed data, but neither is
            // the current snapshot for this certificate.
            _batchLogs.Add(new CertificateBatchLog
            {
                BatchNumber = BatchNumber,
                CertificateReference = "Different certificate",
                Status = CertificateStatus.Delivered,
                StatusAt = DeliveredAt,
                CertificateData =
                    Clone(updatedCertificate.CertificateData)
            });

            _batchLogs.Add(new CertificateBatchLog
            {
                BatchNumber = BatchNumber - 1,
                CertificateReference = CertificateReference,
                Status = CertificateStatus.Delivered,
                StatusAt = DeliveredAt.AddDays(-1),
                CertificateData =
                    Clone(updatedCertificate.CertificateData)
            });

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchCleared(result);

            _batchLog.Should().BeEquivalentTo(originalBatch);
            _batchLogs.Should().HaveCount(3);

            AssertSavedAndLogged(result, expectedBatchNumber: null);
        }

        [Test]
        public async Task AndCertificateHasNoBatchThenNoSnapshotIsRequired()
        {
            // Arrange
            _certificate.BatchNumber = null;
            _certificate.ToBePrinted = null;

            // Retain the old navigation object, but remove available snapshots.
            _batchLogs.Clear();

            var updatedCertificate = Clone(_certificate);
            ChangePrintedName(updatedCertificate.CertificateData);

            var expectedData = Clone(updatedCertificate.CertificateData);

            // Act
            var result = await UpdateCertificate(updatedCertificate);

            // Assert
            AssertBatchCleared(result);

            result.CertificateData.Should()
                .BeEquivalentTo(expectedData);

            AssertSavedAndLogged(result, expectedBatchNumber: null);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task AndCurrentSnapshotIsUnavailableThenThrowsWithoutSaving(
            bool snapshotExists)
        {
            // Arrange
            if (snapshotExists)
            {
                _batchLog.CertificateData = null;
            }
            else
            {
                _batchLogs.Clear();
            }

            var updatedCertificate = Clone(_certificate);
            ChangePrintedName(updatedCertificate.CertificateData);

            Func<Task> act = () => UpdateCertificate(updatedCertificate);

            // Act / Assert
            await act.Should().ThrowAsync<InvalidOperationException>();

            _logs.Should().BeEmpty();

            _dbContext.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static void AssertBatchCleared(CertificateBase result)
        {
            result.BatchNumber.Should().BeNull();
            result.ToBePrinted.Should().BeNull();
            result.PrintStatus.Should().BeNull();
            result.PrintStatusAt.Should().BeNull();

            AssertRequestDetailsPreserved(result);
        }

        private static void AssertBatchPreserved(CertificateBase result)
        {
            result.BatchNumber.Should().Be(BatchNumber);
            result.ToBePrinted.Should().Be(SentToPrinterAt);
            result.PrintStatus.Should().Be(CertificateStatus.Delivered);
            result.PrintStatusAt.Should().Be(DeliveredAt);

            AssertRequestDetailsPreserved(result);
        }

        private static void AssertRequestDetailsPreserved(CertificateBase result)
        {
            result.Status.Should().Be(CertificateStatus.Delivered);
            result.PrintRequestedAt.Should().Be(RequestedAt);
            result.PrintRequestedBy.Should().Be(RequestingUser);
        }

        private void AssertSavedAndLogged(CertificateBase result, int? expectedBatchNumber)
        {
            result.Should().BeSameAs(_certificate);
            result.UpdatedBy.Should().Be(UpdatingUser);

            var log = _logs.Should().ContainSingle().Which;

            log.CertificateId.Should().Be(result.Id);
            log.Action.Should().Be(CertificateActions.Amend);
            log.Status.Should().Be(result.Status);
            log.Username.Should().Be(UpdatingUser);
            log.BatchNumber.Should().Be(expectedBatchNumber);
            log.CertificateData.Should()
                .BeEquivalentTo(result.CertificateData);

            _dbContext.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        private async Task<T> UpdateCertificate(T updatedCertificate)
        {
            switch (updatedCertificate)
            {
                case Certificate standard:
                    return (T)(CertificateBase)
                        await _sut.UpdateStandardCertificate(
                            standard,
                            UpdatingUser,
                            CertificateActions.Amend);

                case FrameworkCertificate framework:
                    return (T)(CertificateBase)
                        await _sut.UpdateFrameworkCertificate(
                            framework,
                            UpdatingUser,
                            CertificateActions.Amend);

                default:
                    throw new InvalidOperationException(
                        $"Unsupported certificate type: {typeof(T).Name}");
            }
        }

        public static IEnumerable<TestCaseData> PrintedContentChanges()
        {
            yield return new TestCaseData(
                nameof(CertificateData.AchievementDate),
                RequestedAt.AddDays(-5));

            if (typeof(T) == typeof(Certificate))
            {
                yield return new TestCaseData(
                    nameof(CertificateData.LearnerGivenNames),
                    "Updated given names");

                yield return new TestCaseData(
                    nameof(CertificateData.LearnerFamilyName),
                    "Updated family name");

                yield return new TestCaseData(
                    nameof(CertificateData.StandardName),
                    "Updated standard");

                yield return new TestCaseData(
                    nameof(CertificateData.StandardLevel),
                    4);

                yield return new TestCaseData(
                    nameof(CertificateData.CourseOption),
                    "Updated option");

                yield return new TestCaseData(
                    nameof(CertificateData.OverallGrade),
                    "Merit");

                yield return new TestCaseData(
                    nameof(CertificateData.CoronationEmblem),
                    false);
            }
            else
            {
                yield return new TestCaseData(
                    nameof(CertificateData.FullName),
                    "Updated full name");

                yield return new TestCaseData(
                    nameof(CertificateData.FrameworkName),
                    "Updated framework");

                yield return new TestCaseData(
                    nameof(CertificateData.PathwayName),
                    "Updated pathway");

                yield return new TestCaseData(
                    nameof(CertificateData.FrameworkLevelName),
                    "Higher");

                yield return new TestCaseData(
                    nameof(CertificateData.FrameworkCertificateNumber),
                    "FW999999");
            }
        }

        public static IEnumerable<TestCaseData> NonPrintedContentChanges()
        {
            yield return new TestCaseData(
                nameof(CertificateData.IncidentNumber),
                "INC-123");

            yield return new TestCaseData(
                nameof(CertificateData.AmendReasons),
                new List<string> { "Administrative amendment" });

            yield return new TestCaseData(
                nameof(CertificateData.ReprintReasons),
                new List<string> { "Replacement requested" });

            yield return new TestCaseData(
                nameof(CertificateData.ContactName),
                "Updated recipient");

            yield return new TestCaseData(
                nameof(CertificateData.ContactOrganisation),
                "Updated delivery organisation");

            yield return new TestCaseData(
                nameof(CertificateData.ContactAddLine1),
                "Updated address line 1");

            yield return new TestCaseData(
                nameof(CertificateData.ContactAddLine2),
                "Updated address line 2");

            yield return new TestCaseData(
                nameof(CertificateData.ContactAddLine3),
                "Updated address line 3");

            yield return new TestCaseData(
                nameof(CertificateData.ContactAddLine4),
                "Updated address line 4");

            yield return new TestCaseData(
                nameof(CertificateData.ContactPostCode),
                "AB1 2CD");

            yield return new TestCaseData(
                nameof(CertificateData.Department),
                "Updated department");

            yield return new TestCaseData(
                nameof(CertificateData.EmployerName),
                "Updated employer");

            yield return new TestCaseData(
                nameof(CertificateData.ProviderName),
                "Updated provider");

            yield return new TestCaseData(
                nameof(CertificateData.Version),
                "2.0");

            yield return new TestCaseData(
                nameof(CertificateData.StandardReference),
                "ST9999");

            yield return new TestCaseData(
                nameof(CertificateData.LearningStartDate),
                RequestedAt.AddYears(-3));

            if (typeof(T) == typeof(Certificate))
            {
                yield return new TestCaseData(
                    nameof(CertificateData.FullName),
                    "Updated unused full name");
            }
            else
            {
                yield return new TestCaseData(
                    nameof(CertificateData.LearnerGivenNames),
                    "Updated unused given names");

                yield return new TestCaseData(
                    nameof(CertificateData.LearnerFamilyName),
                    "Updated unused family name");
            }
        }

        public static IEnumerable<TestCaseData> EquivalentPrintedContentChanges()
        {
            yield return new TestCaseData(
                nameof(CertificateData.AchievementDate),
                RequestedAt.Date.AddHours(9),
                RequestedAt.Date.AddHours(17));

            if (typeof(T) == typeof(Certificate))
            {
                yield return new TestCaseData(
                    nameof(CertificateData.CourseOption),
                    null,
                    string.Empty);

                yield return new TestCaseData(
                    nameof(CertificateData.CourseOption),
                    string.Empty,
                    " ");

                yield return new TestCaseData(
                    nameof(CertificateData.CourseOption),
                    " ",
                    "\t");

                yield return new TestCaseData(
                    nameof(CertificateData.OverallGrade),
                    null,
                    string.Empty);

                yield return new TestCaseData(
                    nameof(CertificateData.OverallGrade),
                    " ",
                    "No grade awarded");

                yield return new TestCaseData(
                    nameof(CertificateData.OverallGrade),
                    "No grade awarded",
                    string.Empty);
            }
            else
            {
                yield return new TestCaseData(
                    nameof(CertificateData.PathwayName),
                    "Example framework",
                    "EXAMPLE FRAMEWORK");
            }
        }

        private static void ChangePrintedName(CertificateData data)
        {
            if (typeof(T) == typeof(Certificate))
                data.LearnerGivenNames = "Updated name";
            else
                data.FullName = "Updated name";
        }

        private static void SetValue(
            CertificateData data,
            string propertyName,
            object value)
        {
            typeof(CertificateData)
                .GetProperty(propertyName)
                .SetValue(data, value);
        }

        private static TValue Clone<TValue>(TValue value)
        {
            return JsonConvert.DeserializeObject<TValue>(
                JsonConvert.SerializeObject(value));
        }

        private static CertificateData CreateCertificateData()
        {
            return new CertificateData
            {
                LearnerGivenNames = "Casey",
                LearnerFamilyName = "Example",
                FullName = "Casey Example",
                StandardName = "Example standard",
                StandardReference = "ST0001",
                StandardLevel = 3,
                Version = "1.0",
                OverallGrade = "Distinction",
                AchievementDate = RequestedAt.AddDays(-10),
                CourseOption = "Example option",
                CoronationEmblem = true,
                FrameworkName = "Example framework",
                FrameworkLevelName = "Advanced",
                PathwayName = "Example pathway",
                FrameworkCertificateNumber = "FW123456",
                EmployerName = "Existing employer",
                ProviderName = "Existing provider",
                LearningStartDate = RequestedAt.AddYears(-2),
                ContactName = "Existing recipient",
                ContactOrganisation = "Existing organisation",
                ContactAddLine1 = "1 Existing Road",
                ContactAddLine2 = "Existing village",
                ContactAddLine3 = "Existing town",
                ContactAddLine4 = "Existing county",
                ContactPostCode = "XY1 2ZZ",
                EpaDetails = new EpaDetails
                {
                    LatestEpaOutcome = EpaOutcome.Pass,
                    Epas = new List<EpaRecord>()
                }
            };
        }
    }
}