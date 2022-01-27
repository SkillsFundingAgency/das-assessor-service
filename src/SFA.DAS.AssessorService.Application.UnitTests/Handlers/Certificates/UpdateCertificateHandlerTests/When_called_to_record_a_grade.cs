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
    
    public class When_called_to_record_a_grade
    {
        private Mock<ICertificateRepository> CertificateRepository;
        private Mock<IMediator> Mediator;
        private Guid CertificateId = Guid.NewGuid();
        private string CertificateReference = "00010000";
        private UpdateCertificateHandler Sut;

        [TestCase(null, null, null, CertificateGrade.Pass, CertificateStatus.Draft, null, null, null, null, null, null, null, null, 0)]
        [TestCase(2019, 10, 1, CertificateGrade.Pass, CertificateStatus.Draft, null, null, null, null, null, null, null, null, 0)]
        [TestCase(2019, 10, 1, CertificateGrade.Pass, CertificateStatus.Submitted, null, null, null, null, 2019, 10, 1, EpaOutcome.Pass, 1)]
        [TestCase(2019, 10, 1, CertificateGrade.Pass, CertificateStatus.ToBeApproved, null, null, null, null, 2019, 10, 1, EpaOutcome.Pass, 1)]
        [TestCase(null, null, null, CertificateGrade.Fail, CertificateStatus.Draft, null, null, null, null, null, null, null, null, 0)]
        [TestCase(2019, 10, 2, CertificateGrade.Fail, CertificateStatus.Draft, null, null, null, null, null, null, null, null, 0)]
        [TestCase(2019, 10, 2, CertificateGrade.Fail, CertificateStatus.Submitted, null, null, null, null, 2019, 10, 2, EpaOutcome.Fail, 1)]
        [TestCase(2019, 10, 2, CertificateGrade.Fail, CertificateStatus.ToBeApproved, null, null, null, null, 2019, 10, 2, EpaOutcome.Fail, 1)]
        [TestCase(2019, 10, 3, CertificateGrade.Pass, CertificateStatus.Submitted, 2019, 10, 2, EpaOutcome.Fail, 2019, 10, 3, EpaOutcome.Pass, 2)]
        [TestCase(2019, 10, 3, CertificateGrade.Pass, CertificateStatus.ToBeApproved, 2019, 10, 2, EpaOutcome.Fail, 2019, 10, 3, EpaOutcome.Pass, 2)]
        [TestCase(2019, 10, 8, CertificateGrade.Pass, CertificateStatus.Submitted, 2019, 10, 8, EpaOutcome.Fail, 2019, 10, 8, EpaOutcome.Pass, 2)]
        [TestCase(2019, 10, 8, CertificateGrade.Pass, CertificateStatus.ToBeApproved, 2019, 10, 8, EpaOutcome.Fail, 2019, 10, 8, EpaOutcome.Pass, 2)]
        [TestCase(2019, 10, 2, CertificateGrade.Pass, CertificateStatus.Submitted, 2019, 10, 2, "pass", 2019, 10, 2, EpaOutcome.Pass, 1)]
        [TestCase(2019, 10, 2, CertificateGrade.Pass, CertificateStatus.Submitted, 2019, 10, 2, EpaOutcome.Pass, 2019, 10, 2, "pass", 1)]
        [TestCase(2019, 10, 2, CertificateGrade.Pass, CertificateStatus.ToBeApproved, 2019, 10, 2, "pass", 2019, 10, 2, EpaOutcome.Pass, 1)]
        [TestCase(2019, 10, 2, CertificateGrade.Pass, CertificateStatus.ToBeApproved, 2019, 10, 2, EpaOutcome.Pass, 2019, 10, 2, "pass", 1)]
        public async Task Then_the_certificate_is_updated(
            int? updatedAchievementYear, int? updatedAchievementMonth, int? updatedAchievementDay, string updatedCertificateGrade, string updateCertificateStatus,
            int? currentLatestEpaYear, int? currentLatestEpaMonth, int? currentLastestEpaDay, string currentLatestEpaOutcome,
            int? updatedLatestEpaYear, int? updatedLatestEpaMonth, int? updatedLastestEpaDay, string updatedLatestEpaOutcome, int numberOfEpaDetails)
        {
            CertificateRepository = new Mock<ICertificateRepository>();
            CertificateRepository.Setup(r => r.GetCertificateLogsFor(CertificateId)).ReturnsAsync(new List<CertificateLog>());

            var updatedAchievementDate = updatedAchievementYear.HasValue && updatedAchievementMonth.HasValue && updatedAchievementDay.HasValue
                ? new DateTime(updatedAchievementYear.Value, updatedAchievementMonth.Value, updatedAchievementDay.Value)
                : (DateTime?)null;

            var certificateData = Builder<CertificateData>.CreateNew()
                .With(ecd => ecd.OverallGrade = updatedCertificateGrade)
                .With(ecd => ecd.AchievementDate = updatedAchievementDate)
                .Build();

            var currentLatestEpaDate = currentLatestEpaYear.HasValue && currentLatestEpaMonth.HasValue && currentLastestEpaDay.HasValue
                ? new DateTime(currentLatestEpaYear.Value, currentLatestEpaMonth.Value, currentLastestEpaDay.Value)
                : (DateTime?)null;

            if (currentLatestEpaDate.HasValue)
            {
                certificateData.EpaDetails = new EpaDetails
                {
                    LatestEpaDate = currentLatestEpaDate.Value,
                    LatestEpaOutcome = currentLatestEpaOutcome,
                    Epas = new List<EpaRecord>
                    {
                        new EpaRecord
                        {
                            EpaDate = currentLatestEpaDate.Value,
                            EpaOutcome = currentLatestEpaOutcome
                        }
                    }
                };
            }

            var certificate = Builder<Certificate>.CreateNew()
                .With(c => c.Id = CertificateId)
                .With(c => c.CertificateReference = CertificateReference)
                .With(c => c.Status = updateCertificateStatus)
                .With(c => c.CertificateData = JsonConvert.SerializeObject(certificateData))
                .Build();

            CertificateRepository.Setup(r => r.Update(It.Is<Certificate>(c => c.CertificateReference == CertificateReference), "user", null, true, null))
                .ReturnsAsync(certificate);

            Mediator.Setup(r => r.Send(It.IsAny<GetCertificateRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(certificate);

            var updatedLatestEpaDate = updatedLatestEpaYear.HasValue && updatedLatestEpaMonth.HasValue && updatedLastestEpaDay.HasValue
                ? new DateTime(updatedLatestEpaYear.Value, updatedLatestEpaMonth.Value, updatedLastestEpaDay.Value)
                : (DateTime?)null;

            Sut = new UpdateCertificateHandler(CertificateRepository.Object, Mediator.Object, new Mock<ILogger<UpdateCertificateHandler>>().Object);
            await Sut.Handle(new UpdateCertificateRequest(certificate) { Username = "user" }, new CancellationToken());

            CertificateRepository.Verify(r => r.Update(It.Is<Certificate>(updatedCertificate =>
                VerifyCertificateUpdated(certificate, updatedCertificate, updatedLatestEpaDate, updatedLatestEpaOutcome, numberOfEpaDetails)), "user", null, true, null));
        }

        public static bool VerifyCertificateUpdated(Certificate first, Certificate second, DateTime? latestEpaDate, string latestEpaOutcome, int numberOfEpaDetails)
        {
            var firstCertficateData = JsonConvert.DeserializeObject<CertificateData>(first.CertificateData);
            var secondCertficateData = JsonConvert.DeserializeObject<CertificateData>(second.CertificateData);

            bool achievementDateMached = firstCertficateData.AchievementDate == secondCertficateData.AchievementDate;
            bool overallGradeMatched = firstCertficateData.OverallGrade == secondCertficateData.OverallGrade;
            bool numberOfEpaDetailsMatched = (firstCertficateData.EpaDetails?.Epas?.Count ?? 0) == numberOfEpaDetails;
            bool latestEpaDateMatched = firstCertficateData.EpaDetails?.LatestEpaDate == latestEpaDate;
            bool latestEpaOutcomeMatched = (firstCertficateData.EpaDetails?.LatestEpaOutcome ?? string.Empty).Equals(latestEpaOutcome ?? string.Empty, StringComparison.InvariantCultureIgnoreCase);

            string failureMessage = string.Empty;
            if (!achievementDateMached)
                failureMessage = $"Certificate.AchievementDate did not match; expected {firstCertficateData.AchievementDate} received {secondCertficateData.AchievementDate}";
            else if (!overallGradeMatched)
                failureMessage = $"Certificate.CertificateData.OverallGrade did not match; expected {firstCertficateData.OverallGrade} received {secondCertficateData.OverallGrade}";
            else if(!numberOfEpaDetailsMatched)
                failureMessage = $"Certificate.CertificateData.EpaDetails.Epas.Count did not match; expected {numberOfEpaDetails} received {firstCertficateData.EpaDetails?.Epas?.Count ?? 0}";
            else if(!latestEpaDateMatched)
                failureMessage = $"Certificate.CertificateData.EpaDetails.LatestEpaDate did not match; expected {latestEpaDate} received {firstCertficateData.EpaDetails?.LatestEpaDate}";
            else if(!latestEpaOutcomeMatched)
                failureMessage = $"Certificate.CertificateData.EpaDetails.LatestEpaOutcome did not match; expected {latestEpaOutcome} received {firstCertficateData.EpaDetails?.LatestEpaOutcome}";

            if (!string.IsNullOrEmpty(failureMessage))
                TestContext.WriteLine(failureMessage);

            return achievementDateMached &&
                overallGradeMatched &&
                numberOfEpaDetailsMatched &&
                latestEpaDateMatched &&
                latestEpaOutcomeMatched;
        }
    }
}
