using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_ukprn_with_later_start_date : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase(null, "31-12-9999", 99, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", null, 99, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", null, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", 99, null, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", 99, 9, null, "New")]
        [TestCase("EPA009999", "31-12-9999", 99, 9, "31-12-1111", null)]
        public async Task Then_an_existing_learner_record_is_replaced(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode,
                LearnerOne.FundingModel, LearnerOne.GivenNames, LearnerOne.FamilyName, epaOrgId,
                LearnerOne.LearnStartDate.AddDays(10), // add 10 days to the start date
                LearnerOne.PlannedEndDate, LearnerOne.CompletionStatus, LearnerOne.LearnRefNumber, LearnerOne.DelLocPostCode,
                learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate),
                withdrawReason, outcome,
                achDate == null ? (DateTime?)null : DateTime.Parse(achDate),
                outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(Request.Source, Request.Ukprn.Value, Request.Uln.Value, Request.StdCode.Value,
                Request.FundingModel, Request.GivenNames, Request.FamilyName, Request.EpaOrgId, Request.LearnStartDate, 
                Request.PlannedEndDate, Request.CompletionStatus, Request.LearnRefNumber, Request.DelLocPostCode,
                Request.LearnActEndDate, Request.WithdrawReason, Request.Outcome, Request.AchDate, Request.OutGrade), Times.Once);
        }

        [TestCase(null, "31-12-9999", 99, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", null, 99, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", null, 9, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", 99, null, "31-12-1111", "New")]
        [TestCase("EPA009999", "31-12-9999", 99, 9, null, "New")]
        [TestCase("EPA009999", "31-12-9999", 99, 9, "31-12-1111", null)]
        public async Task Then_result_is_replace(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode,
                LearnerOne.FundingModel, LearnerOne.GivenNames, LearnerOne.FamilyName, epaOrgId, 
                LearnerOne.LearnStartDate.AddDays(10), // add 10 days to the start date
                LearnerOne.PlannedEndDate, LearnerOne.CompletionStatus, LearnerOne.LearnRefNumber, LearnerOne.DelLocPostCode,
                learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate),
                withdrawReason, outcome,
                achDate == null ? (DateTime?)null : DateTime.Parse(achDate),
                outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ReplacedLearnerDetail");
        }
    }
}