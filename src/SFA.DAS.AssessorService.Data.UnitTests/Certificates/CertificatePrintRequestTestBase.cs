using System;
using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public abstract class CertificatePrintRequestTestBase<T>
        where T : CertificateBase, new()
    {
        protected const string RequestedBy = "Requesting user";

        protected static DateTime RequestedAt =>
            new DateTime(2026, 11, 3, 10, 0, 0, DateTimeKind.Utc);

        protected Mock<IAssessorDbContext> _dbContext;
        protected Mock<DbSet<CertificateLog>> _logSet;
        protected List<CertificateLog> _logs;
        protected CertificateRepository _sut;
        protected T _certificate;
        protected CertificateBase _otherCertificate;
        protected CertificatePrintAddress _address;

        [SetUp]
        public void SetUp()
        {
            _dbContext = new Mock<IAssessorDbContext>();
            _logSet = new Mock<DbSet<CertificateLog>>();
            _logs = new List<CertificateLog>();

            _certificate = CreateCertificate<T>("00123456");

            _dbContext
                .Setup(x => x.Set<T>())
                .ReturnsDbSet(new List<T> { _certificate });

            if (typeof(T) == typeof(Certificate))
            {
                var other = CreateCertificate<FrameworkCertificate>(
                    "00987654");

                _otherCertificate = other;

                _dbContext
                    .Setup(x => x.Set<FrameworkCertificate>())
                    .ReturnsDbSet(new List<FrameworkCertificate> { other });
            }
            else
            {
                var other = CreateCertificate<Certificate>(
                    "00987654");

                _otherCertificate = other;

                _dbContext
                    .Setup(x => x.Set<Certificate>())
                    .ReturnsDbSet(new List<Certificate> { other });
            }

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

            _sut = new CertificateRepository(unitOfWork.Object);

            _address = new CertificatePrintAddress
            {
                ContactName = "New recipient",
                ContactOrganisation = "New organisation",
                ContactAddLine1 = "10 New Road",
                ContactAddLine2 = "New village",
                ContactAddLine3 = "New town",
                ContactAddLine4 = "New county",
                ContactPostCode = "AB1 2CD"
            };
        }

        protected void AssertPrintState(
            CertificateBase result,
            string expectedStatus,
            DateTime beforeUpdatedAt,
            DateTime afterUpdatedAt)
        {
            result.Should().BeSameAs(_certificate);
            result.Status.Should().Be(expectedStatus);
            result.PrintRequestedAt.Should().Be(RequestedAt);
            result.PrintRequestedBy.Should().Be(RequestedBy);

            result.BatchNumber.Should().BeNull();
            result.ToBePrinted.Should().BeNull();
            result.PrintStatus.Should().BeNull();
            result.PrintStatusAt.Should().BeNull();

            result.UpdatedBy.Should().Be(RequestedBy);
            result.UpdatedAt.Should().NotBeNull();
            result.UpdatedAt.Value.Should()
                .BeOnOrAfter(beforeUpdatedAt)
                .And.BeOnOrBefore(afterUpdatedAt);

            VerifySavedOnce();
        }

        protected void AssertIndependentAuditSnapshot(
            CertificateBase result,
            string expectedStatus)
        {
            var log = _logs.Should().ContainSingle().Which;

            log.Id.Should().NotBeEmpty();
            log.CertificateId.Should().Be(result.Id);
            log.Action.Should().Be(CertificateActions.PrintRequest);
            log.Status.Should().Be(expectedStatus);
            log.EventTime.Should().Be(RequestedAt);
            log.Username.Should().Be(RequestedBy);
            log.BatchNumber.Should().BeNull();

            log.CertificateData.Should()
                .BeEquivalentTo(result.CertificateData);

            log.CertificateData.Should()
                .NotBeSameAs(result.CertificateData);

            var expectedSnapshot = Clone(result.CertificateData);

            result.CertificateData.LearnerGivenNames = "Changed later";
            result.CertificateData.ContactName = "Changed recipient";
            result.CertificateData.EpaDetails.LatestEpaOutcome =
                EpaOutcome.Fail;
            result.CertificateData.EpaDetails.Epas[0].EpaOutcome =
                EpaOutcome.Fail;

            log.CertificateData.Should()
                .BeEquivalentTo(expectedSnapshot);

            VerifySavedOnce();
        }

        protected void VerifySavedOnce()
        {
            _dbContext.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        protected void VerifyNothingSavedOrLogged()
        {
            _logs.Should().BeEmpty();

            _logSet.Verify(
                x => x.Add(It.IsAny<CertificateLog>()),
                Times.Never);

            _dbContext.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        protected static CertificateData Clone(CertificateData data)
        {
            return JsonConvert.DeserializeObject<CertificateData>(
                JsonConvert.SerializeObject(data));
        }

        private static T CreateCertificate<T>(
            string reference)
            where T : CertificateBase, new()
        {
            var previousRequestedAt = RequestedAt.AddMonths(-1);

            var data = new CertificateData
            {
                LearnerGivenNames = "Casey",
                LearnerFamilyName = "Example",
                FullName = "Casey Example",
                StandardName = "Example standard",
                StandardReference = "ST0001",
                StandardLevel = 3,
                Version = "1.0",
                OverallGrade = "Distinction",
                AchievementDate = previousRequestedAt.AddDays(-10),
                CourseOption = "Example option",
                CoronationEmblem = true,
                EmployerName = "Existing employer",
                ProviderName = "Existing provider",
                FrameworkName = "Example framework",
                FrameworkLevelName = "Advanced",
                PathwayName = "Example pathway",
                FrameworkCertificateNumber = "FW123456",
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
                    Epas = new List<EpaRecord>
                    {
                        new EpaRecord
                        {
                            EpaDate = previousRequestedAt.AddDays(-10),
                            EpaOutcome = EpaOutcome.Pass
                        }
                    }
                }
            };

            return new T
            {
                Id = Guid.NewGuid(),
                CertificateReference = reference,
                ProviderUkPrn = 12345678,
                DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = CertificateStatus.Delivered,
                PrintRequestedAt = previousRequestedAt,
                PrintRequestedBy = "Previous requesting user",
                BatchNumber = 7,
                ToBePrinted = previousRequestedAt.AddDays(1),
                CreatedAt = previousRequestedAt.AddMonths(-1),
                UpdatedAt = previousRequestedAt.AddDays(2),
                UpdatedBy = "Previous updating user",
                CertificateData = data,
                CertificateBatchLog = new CertificateBatchLog
                {
                    BatchNumber = 7,
                    CertificateReference = reference,
                    Status = CertificateStatus.Delivered,
                    StatusAt = previousRequestedAt.AddDays(2),
                    CertificateData = Clone(data)
                }
            };
        }
    }
}