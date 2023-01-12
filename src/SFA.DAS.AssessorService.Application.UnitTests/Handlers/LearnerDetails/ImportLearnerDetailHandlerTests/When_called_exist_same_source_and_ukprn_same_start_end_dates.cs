using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_exist_same_source_and_ukprn_same_start_end_dates : ImportLearnerDetailHandlerTestsBase
    {
        public void Arrange(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(
                LearnerWithCertificate.Source,
                LearnerWithCertificate.UkPrn,
                LearnerWithCertificate.Uln,
                LearnerWithCertificate.StdCode,
                55,
                "NewFirstName",
                "NewFamilyName",
                epaOrgId,
                LearnerWithCertificate.LearnStartDate,
                LearnerWithCertificate.PlannedEndDate,
                55,
                "55555555",
                "55POST55",
                learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate, CultureInfo.InvariantCulture),
                withdrawReason,
                outcome,
                achDate == null ? (DateTime?)null : DateTime.Parse(achDate, CultureInfo.InvariantCulture),
                outGrade);

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_an_existing_learner_record_is_updated(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Arrange(epaOrgId, learnActEndDate, withdrawReason, outcome, achDate, outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrUpdated(
                ImportLearnerDetail.Source,
                ImportLearnerDetail.Ukprn.Value,
                ImportLearnerDetail.Uln.Value,
                ImportLearnerDetail.StdCode.Value,
                ImportLearnerDetail.FundingModel,
                ImportLearnerDetail.GivenNames,
                ImportLearnerDetail.FamilyName,
                epaOrgId == null ? LearnerWithCertificate.EpaOrgId : ImportLearnerDetail.EpaOrgId, // keep current when null
                ImportLearnerDetail.LearnStartDate,
                ImportLearnerDetail.PlannedEndDate,
                ImportLearnerDetail.CompletionStatus,
                ImportLearnerDetail.LearnRefNumber,
                ImportLearnerDetail.DelLocPostCode,
                learnActEndDate == null ? LearnerWithCertificate.LearnActEndDate : ImportLearnerDetail.LearnActEndDate.Value, // keep current when null
                withdrawReason == null ? LearnerWithCertificate.WithdrawReason : ImportLearnerDetail.WithdrawReason, // keep current when null
                outcome == null ? LearnerWithCertificate.Outcome : ImportLearnerDetail.Outcome, // keep current when null
                achDate == null ? LearnerWithCertificate.AchDate : ImportLearnerDetail.AchDate, // keep current when null
                outGrade == null ? LearnerWithCertificate.OutGrade : ImportLearnerDetail.OutGrade, // keep current when null
                Times.Once);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_result_is_update(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Arrange(epaOrgId, learnActEndDate, withdrawReason, outcome, achDate, outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("UpdatedLearnerDetail");
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { null, "12/31/9999", 99, 9, "12/31/1111", "New" },
                new object[] { "EPA009999", null, 99, 9, "12/31/1111", "New" },
                new object[] { "EPA009999", "12/31/9999", null, 9, "12/31/1111", "New" },
                new object[] { "EPA009999", "12/31/9999", 99, null, "12/31/1111", "New" },
                new object[] { "EPA009999", "12/31/9999", 99, 9, null, "New" },
                new object[] { "EPA009999", "12/31/9999", 99, 9, "12/31/1111", null }
            };
        }
    }
}