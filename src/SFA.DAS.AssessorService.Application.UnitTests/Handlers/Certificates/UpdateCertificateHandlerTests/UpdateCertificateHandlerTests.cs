using FizzWare.NBuilder;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CertificateStatus = SFA.DAS.AssessorService.Domain.Consts.CertificateStatus;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificateHandlerTests
{
    [TestFixture]

    public class UpdateCertificateHandlerTests
    {
        private Guid CertificateId = Guid.NewGuid();
        private string CertificateReference = "00010000";

        [TestCase("UI", CertificateSendTo.None, CertificateSendTo.Employer, true, false)]
        [TestCase("UI", CertificateSendTo.None, CertificateSendTo.Apprentice, true, true)]
        [TestCase("UI", CertificateSendTo.Apprentice, CertificateSendTo.Employer, true, false)]
        [TestCase("UI", CertificateSendTo.Employer, CertificateSendTo.Employer, false, false)]
        [TestCase("UI", CertificateSendTo.Apprentice, CertificateSendTo.Apprentice, false, true)]
        [TestCase("UI", CertificateSendTo.Employer, CertificateSendTo.Apprentice, true, true)]
        [TestCase("API", CertificateSendTo.None, CertificateSendTo.Employer, false, false)] // special case for retaining API created values which are Employer values
        [TestCase("API", CertificateSendTo.None, CertificateSendTo.Apprentice, true, true)]
        [TestCase("API", CertificateSendTo.Apprentice, CertificateSendTo.Employer, true, false)]
        [TestCase("API", CertificateSendTo.Employer, CertificateSendTo.Employer, false, false)]
        [TestCase("API", CertificateSendTo.Apprentice, CertificateSendTo.Apprentice, false, true)]
        [TestCase("API", CertificateSendTo.Employer, CertificateSendTo.Apprentice, true, true)]
        public async Task WhenSendToIsChanged_ThenAddressDetailsAreReset(string createdBy, CertificateSendTo currentSendTo, CertificateSendTo updatedSendTo,
            bool addressDetailsCleared, bool fullNameTransferred)
        {
            // Arrange
            var fixture = new TheFixture()
                .WithCertificate(CertificateId, CertificateReference, CertificateStatus.Draft, createdBy, currentSendTo);

            // Act
            await fixture.Handle(CertificateStatus.Draft, updatedSendTo);

            // Assert
            fixture.VerifyAddressDetailsReset(addressDetailsCleared, fullNameTransferred);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task WhenOverallGradeAndAchievementDateChanges_ThenEpaDetailsAreUpdated(
            DateTime? updatedAchievementDate, string updatedOverallGrade, string updatedCertificateStatus,
            DateTime? currentLatestEpaDate, string currentLatestEpaOutcome,
            DateTime? expectedLatestEpaDate, string expectedLatestEpaOutcome, int expectedEpasCount)
        {
            // Arrange
            var fixture = new TheFixture()
                .WithCertificate(CertificateId, CertificateReference, CertificateStatus.Draft, currentLatestEpaOutcome, currentLatestEpaDate);

            // Act
            await fixture.Handle(updatedCertificateStatus, updatedOverallGrade, updatedAchievementDate);

            // Assert
            fixture.VerifyUpdatedAchievementDate(updatedAchievementDate);
            fixture.VerifyUpdatedOverallGrade(updatedOverallGrade);
            fixture.VerifyUpdatedEpaDetails(expectedLatestEpaOutcome, expectedLatestEpaDate, expectedEpasCount);
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { null, CertificateGrade.Pass, CertificateStatus.Draft, null, null, null, null, 0 },
                new object[] { new DateTime(2019, 10, 1), CertificateGrade.Pass, CertificateStatus.Draft, null, null, null, null, 0 },
                new object[] { new DateTime(2019, 10, 1), CertificateGrade.Pass, CertificateStatus.Submitted, null, null, new DateTime(2019, 10, 1), EpaOutcome.Pass, 1 },
                new object[] { new DateTime(2019, 10, 1), CertificateGrade.Pass, CertificateStatus.ToBeApproved, null, null, new DateTime(2019, 10, 1), EpaOutcome.Pass, 1 },
                new object[] { null, CertificateGrade.Fail, CertificateStatus.Draft, null, null, null, null, 0},
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Fail, CertificateStatus.Draft, null, null, null, null, 0},
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Fail, CertificateStatus.Submitted, null, null, new DateTime(2019, 10, 2), EpaOutcome.Fail, 1 },
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Fail, CertificateStatus.ToBeApproved, null, null, new DateTime(2019, 10, 2), EpaOutcome.Fail, 1 },
                new object[] { new DateTime(2019, 10, 3), CertificateGrade.Pass, CertificateStatus.Submitted, new DateTime(2019, 10, 2), EpaOutcome.Fail, new DateTime(2019, 10, 3), EpaOutcome.Pass, 2 },
                new object[] { new DateTime(2019, 10, 3), CertificateGrade.Pass, CertificateStatus.ToBeApproved, new DateTime(2019, 10, 2), EpaOutcome.Fail, new DateTime(2019, 10, 3), EpaOutcome.Pass, 2 },
                new object[] { new DateTime(2019, 10, 8), CertificateGrade.Pass, CertificateStatus.Submitted, new DateTime(2019, 10, 8), EpaOutcome.Fail, new DateTime(2019, 10, 8), EpaOutcome.Pass, 2 },
                new object[] { new DateTime(2019, 10, 8), CertificateGrade.Pass, CertificateStatus.ToBeApproved, new DateTime(2019, 10, 8), EpaOutcome.Fail, new DateTime(2019, 10, 8), EpaOutcome.Pass, 2 },
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Pass, CertificateStatus.Submitted, new DateTime(2019, 10, 2), "pass", new DateTime(2019, 10, 2), EpaOutcome.Pass, 1 },
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Pass, CertificateStatus.Submitted, new DateTime(2019, 10, 2), EpaOutcome.Pass, new DateTime(2019, 10, 2), "pass", 1 },
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Pass, CertificateStatus.ToBeApproved, new DateTime(2019, 10, 2), "pass", new DateTime(2019, 10, 2), EpaOutcome.Pass, 1 },
                new object[] { new DateTime(2019, 10, 2), CertificateGrade.Pass, CertificateStatus.ToBeApproved, new DateTime(2019, 10, 2), EpaOutcome.Pass, new DateTime(2019, 10, 2), "pass", 1 }
            };
        }

        public class TheFixture
        {
            private Mock<ICertificateRepository> _certificateRepository;
            private Mock<IMediator> _mediator;
            private UpdateCertificateHandler _sut;

            private Certificate _certificate;
            private Certificate _updatedCertificate;

            public TheFixture()
            {
                _certificateRepository = new Mock<ICertificateRepository>();
                _mediator = new Mock<IMediator>();
                _sut = new UpdateCertificateHandler(_certificateRepository.Object, _mediator.Object, new Mock<ILogger<UpdateCertificateHandler>>().Object);
            }

            public TheFixture WithCertificate(Guid id, string certficateReference, string status, string grade, DateTime? achievementDate)
            {
                _certificate = BuildCertificate(id, certficateReference, status, grade, achievementDate);

                _mediator.Setup(r => r.Send(It.Is<GetCertificateRequest>(p => p.CertificateId == _certificate.Id), It.IsAny<CancellationToken>())).ReturnsAsync(_certificate);
                _certificateRepository.Setup(r => r.GetCertificateLogsFor(_certificate.Id)).ReturnsAsync(new List<CertificateLog>());

                return this;
            }

            public TheFixture WithCertificate(Guid id, string certficateReference, string status, string createdBy, CertificateSendTo sendTo)
            {
                _certificate = BuildCertificate(id, certficateReference, status, createdBy, sendTo);

                _mediator.Setup(r => r.Send(It.Is<GetCertificateRequest>(p => p.CertificateId == _certificate.Id), It.IsAny<CancellationToken>())).ReturnsAsync(_certificate);
                _certificateRepository.Setup(r => r.GetCertificateLogsFor(_certificate.Id)).ReturnsAsync(new List<CertificateLog>());

                return this;
            }

            private Certificate BuildCertificate(Guid id, string certficateReference, string status, string createdBy, CertificateSendTo sendTo)
            {
                var certificateData = Builder<CertificateData>.CreateNew()
                    .With(ecd => ecd.SendTo = sendTo)
                    .Build();

                var certificate = Builder<Certificate>.CreateNew()
                    .With(c => c.Id = id)
                    .With(c => c.CertificateReference = certficateReference)
                    .With(c => c.Status = status)
                    .With(c => c.CreatedBy = createdBy)
                    .With(c => c.CertificateData = JsonConvert.SerializeObject(certificateData))
                    .Build();

                return certificate;
            }

            private Certificate BuildCertificate(Guid id, string certficateReference, string status, string grade, DateTime? achievementDate)
            {
                var certificateData = Builder<CertificateData>.CreateNew()
                    .With(ecd => ecd.OverallGrade = grade)
                    .With(ecd => ecd.AchievementDate = achievementDate)
                    .With(ecd => ecd.EpaDetails = BuildEpaDetails(grade, achievementDate))
                    .Build();

                var certificate = Builder<Certificate>.CreateNew()
                    .With(c => c.Id = id)
                    .With(c => c.CertificateReference = certficateReference)
                    .With(c => c.Status = status)
                    .With(c => c.CertificateData = JsonConvert.SerializeObject(certificateData))
                    .Build();

                return certificate;
            }

            private EpaDetails BuildEpaDetails(string grade, DateTime? achievementDate)
            {
                return achievementDate.HasValue
                    ? new EpaDetails
                    {
                        LatestEpaDate = achievementDate.Value,
                        LatestEpaOutcome = grade,
                        Epas = new List<EpaRecord>
                            {
                                new EpaRecord
                                {
                                    EpaDate = achievementDate.Value,
                                    EpaOutcome = grade
                                }
                            }
                    }
                    : null;
            }

            public async Task<Certificate> Handle(string updatedStatus, string updatedOverallGrade, DateTime? updatedAchievementDate)
            {
                var updateCertificate = CloneCertificate(_certificate);
                updateCertificate.Status = updatedStatus;

                var updateCertificateData = JsonConvert.DeserializeObject<CertificateData>(updateCertificate.CertificateData);
                updateCertificateData.OverallGrade = updatedOverallGrade;
                updateCertificateData.AchievementDate = updatedAchievementDate;
                updateCertificate.CertificateData = JsonConvert.SerializeObject(updateCertificateData);

                _updatedCertificate = await _sut.Handle(new UpdateCertificateRequest(updateCertificate) { Username = "user" }, new CancellationToken());
                return _updatedCertificate;
            }

            public async Task<Certificate> Handle(string updatedStatus, CertificateSendTo updatedSendTo)
            {
                var updateCertificate = CloneCertificate(_certificate);
                updateCertificate.Status = updatedStatus;

                var updateCertificateData = JsonConvert.DeserializeObject<CertificateData>(updateCertificate.CertificateData);
                updateCertificateData.SendTo = updatedSendTo;
                updateCertificate.CertificateData = JsonConvert.SerializeObject(updateCertificateData);

                _updatedCertificate = await _sut.Handle(new UpdateCertificateRequest(updateCertificate) { Username = "user" }, new CancellationToken());
                return _updatedCertificate;
            }


            public void VerifyUpdatedAchievementDate(DateTime? achievenmentDate)
            {
                _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                    GetCertificateData(updatedCertificate).AchievementDate == achievenmentDate), "user", null, true, null));
            }

            public void VerifyUpdatedOverallGrade(string updatedOverallGrade)
            {
                _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                    GetCertificateData(updatedCertificate).OverallGrade == updatedOverallGrade), "user", null, true, null));
            }

            public void VerifyUpdatedEpaDetails(string expectedLatestEpaOutcome, DateTime? expectedLatestEpaDate, int expectedEpasCount)
            {
                _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                    MatchCertificate(updatedCertificate, expectedLatestEpaOutcome, expectedLatestEpaDate, expectedEpasCount)), "user", null, true, null));
            }

            public void VerifyAddressDetailsReset(bool addressDetailsCleared, bool fullNameTransferred)
            {
                if (addressDetailsCleared)
                {
                    _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                        MatchCertificateOnContactAddressDetails(updatedCertificate,
                        fullNameTransferred ? GetCertificateData(_certificate).FullName : null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null)), "user", null, true, null), Times.Once);
                }
                else
                {
                    _certificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                        MatchCertificateOnContactAddressDetails(updatedCertificate,
                            fullNameTransferred ? GetCertificateData(_certificate).FullName : GetCertificateData(_certificate).ContactName,
                            GetCertificateData(_certificate).Department,
                            GetCertificateData(_certificate).ContactOrganisation,
                            GetCertificateData(_certificate).ContactAddLine1,
                            GetCertificateData(_certificate).ContactAddLine2,
                            GetCertificateData(_certificate).ContactAddLine3,
                            GetCertificateData(_certificate).ContactAddLine4,
                            GetCertificateData(_certificate).ContactPostCode)), "user", null, true, null), Times.Once);
                }
            }

            private bool MatchCertificate(Certificate certificate, string expectedLatestEpaOutcome, DateTime? expectedLatestEpaDate, int expectedEpasCount)
            {
                var certificateData = GetCertificateData(certificate);

                bool numberOfEpaDetailsMatched = (certificateData.EpaDetails?.Epas?.Count ?? 0) == expectedEpasCount;
                bool latestEpaDateMatched = certificateData.EpaDetails?.LatestEpaDate == expectedLatestEpaDate;
                bool latestEpaOutcomeMatched = (certificateData.EpaDetails?.LatestEpaOutcome ?? string.Empty).Equals(expectedLatestEpaOutcome ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);

                string failureMessage = "Certificate.CertificateData.EpaDetails.{0} did not match; expected {1} actual value {2}";

                if (!numberOfEpaDetailsMatched)
                    TestContext.WriteLine(string.Format(failureMessage, $"{nameof(CertificateData.EpaDetails.Epas)}.{nameof(CertificateData.EpaDetails.Epas.Count)}", expectedEpasCount, certificateData.EpaDetails?.Epas?.Count ?? 0));
                else if (!latestEpaDateMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.EpaDetails.LatestEpaDate), expectedLatestEpaDate, certificateData.EpaDetails?.LatestEpaDate));
                else if (!latestEpaOutcomeMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.EpaDetails.LatestEpaOutcome), expectedLatestEpaOutcome, certificateData.EpaDetails?.LatestEpaOutcome));

                return numberOfEpaDetailsMatched &&
                    latestEpaDateMatched &&
                    latestEpaOutcomeMatched;
            }

            private bool MatchCertificateOnContactAddressDetails(Certificate certificate, string expectedContactName,
                string expectedDepartment, string expectedContactOrganisation, string expectedContactAddLine1, string expectedContactAddLine2,
                string expectedContactAddLine3, string expectedContactAddLine4, string expectedPostCode)
            {
                var certificateData = GetCertificateData(certificate);

                var contactNameMatched = certificateData.ContactName == expectedContactName;
                var departmentMatched = certificateData.Department == expectedDepartment;
                var contactOrganisationMatched = certificateData.ContactOrganisation == expectedContactOrganisation;
                var contactAddLine1Matched = certificateData.ContactAddLine1 == expectedContactAddLine1;
                var contactAddLine2Matched = certificateData.ContactAddLine2 == expectedContactAddLine2;
                var contactAddLine3Matched = certificateData.ContactAddLine3 == expectedContactAddLine3;
                var contactAddLine4Matched = certificateData.ContactAddLine4 == expectedContactAddLine4;
                var contactPostCodeMatched = certificateData.ContactPostCode == expectedPostCode;

                string failureMessage = "Certificate.CertificateData.{0} did not match; expected {1} actual value {2}";

                if (!contactNameMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactName), expectedContactName, certificateData.ContactName));
                else if (!departmentMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.Department), expectedDepartment, certificateData.Department));
                else if (!contactOrganisationMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactOrganisation), expectedContactOrganisation, certificateData.ContactOrganisation));
                else if (!contactAddLine1Matched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactAddLine1), expectedContactAddLine1, certificateData.ContactAddLine1));
                else if (!contactAddLine2Matched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactAddLine2), expectedContactAddLine2, certificateData.ContactAddLine2));
                else if (!contactAddLine3Matched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactAddLine3), expectedContactAddLine3, certificateData.ContactAddLine3));
                else if (!contactAddLine4Matched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactAddLine4), expectedContactAddLine4, certificateData.ContactAddLine4));
                else if (!contactPostCodeMatched)
                    TestContext.WriteLine(string.Format(failureMessage, nameof(CertificateData.ContactPostCode), expectedPostCode, certificateData.ContactPostCode));

                return contactNameMatched &&
                    departmentMatched &&
                    contactOrganisationMatched &&
                    contactAddLine1Matched &&
                    contactAddLine2Matched &&
                    contactAddLine3Matched &&
                    contactAddLine4Matched &&
                    contactPostCodeMatched;
            }

            private Certificate CloneCertificate(Certificate certificate)
            {
                return JsonConvert.DeserializeObject<Certificate>(JsonConvert.SerializeObject(certificate));
            }

            private CertificateData GetCertificateData(Certificate certificate)
            {
                return JsonConvert.DeserializeObject<CertificateData>(certificate.CertificateData);
            }
        }
    }
}
