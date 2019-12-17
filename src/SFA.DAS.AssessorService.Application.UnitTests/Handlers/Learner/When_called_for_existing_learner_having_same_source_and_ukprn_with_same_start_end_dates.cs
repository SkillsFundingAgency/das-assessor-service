using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_ukprn_with_same_start_end_dates : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase(null, "12/31/9999", 99, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", null, 99, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", null, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", 99, null, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", 99, 9, null, "New")]
        [TestCase("EPA009999", "12/31/9999", 99, 9, "12/31/1111", null)]
        public async Task Then_an_existing_learner_record_is_updated(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode, 
                55, "NewFirstName", "NewFamilyName", epaOrgId, LearnerOne.LearnStartDate, 
                LearnerOne.PlannedEndDate, 55, "55555555", "55POST55",
                learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate, CultureInfo.InvariantCulture),
                withdrawReason, outcome,
                achDate == null ? (DateTime?)null : DateTime.Parse(achDate, CultureInfo.InvariantCulture),
                outGrade);

            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            VerifyIlrUpdated(ImportLearnerDetail.Source, ImportLearnerDetail.Ukprn.Value, ImportLearnerDetail.Uln.Value, ImportLearnerDetail.StdCode.Value,
                ImportLearnerDetail.FundingModel, ImportLearnerDetail.GivenNames, ImportLearnerDetail.FamilyName, 
                epaOrgId == null ? LearnerOne.EpaOrgId : epaOrgId, // keep current when null
                ImportLearnerDetail.LearnStartDate, ImportLearnerDetail.PlannedEndDate, ImportLearnerDetail.CompletionStatus, ImportLearnerDetail.LearnRefNumber, ImportLearnerDetail.DelLocPostCode, 
                learnActEndDate == null ? LearnerOne.LearnActEndDate : DateTime.Parse(learnActEndDate, CultureInfo.InvariantCulture), // keep current when null
                withdrawReason == null ? LearnerOne.WithdrawReason : withdrawReason, // keep current when null
                outcome == null ? LearnerOne.Outcome : outcome, // keep current when null
                achDate == null ? LearnerOne.AchDate : DateTime.Parse(achDate, CultureInfo.InvariantCulture), // keep current when null
                outGrade == null ? LearnerOne.OutGrade : outGrade, // keep current when null
                Times.Once);
        }

        [TestCase(null, "12/31/9999", 99, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", null, 99, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", null, 9, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", 99, null, "12/31/1111", "New")]
        [TestCase("EPA009999", "12/31/9999", 99, 9, null, "New")]
        [TestCase("EPA009999", "12/31/9999", 99, 9, "12/31/1111", null)]
        public async Task Then_result_is_update(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode,
                LearnerOne.FundingModel, LearnerOne.GivenNames, LearnerOne.FamilyName, epaOrgId, LearnerOne.LearnStartDate,
                LearnerOne.PlannedEndDate, LearnerOne.CompletionStatus, LearnerOne.LearnRefNumber, LearnerOne.DelLocPostCode,
                learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate, CultureInfo.InvariantCulture),
                withdrawReason, outcome,
                achDate == null ? (DateTime?)null : DateTime.Parse(achDate, CultureInfo.InvariantCulture),
                outGrade);

            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("UpdatedLearnerDetail");
        }
    }
}