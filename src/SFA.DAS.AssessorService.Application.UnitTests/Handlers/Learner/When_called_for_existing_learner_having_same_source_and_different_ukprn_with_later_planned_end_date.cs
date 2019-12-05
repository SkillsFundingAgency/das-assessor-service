using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_with_later_planned_end_date : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 4444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                LearnerTwo.FundingModel, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate.Value.AddDays(10), LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(Request.Source, Request.Ukprn.Value, Request.Uln.Value, Request.StdCode.Value,
                Request.FundingModel, Request.GivenNames, Request.FamilyName, Request.EpaOrgId, Request.LearnStartDate, 
                Request.PlannedEndDate, Request.CompletionStatus, Request.LearnRefNumber, Request.DelLocPostCode,
                Request.LearnActEndDate, Request.WithdrawReason, Request.Outcome, Request.AchDate, Request.OutGrade), Times.Once);
        }

        [Test]
        public async Task Then_result_is_replace()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 4444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                LearnerTwo.FundingModel, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate.Value.AddDays(10), LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);


            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ReplacedLearnerDetail");
        }
    }
}