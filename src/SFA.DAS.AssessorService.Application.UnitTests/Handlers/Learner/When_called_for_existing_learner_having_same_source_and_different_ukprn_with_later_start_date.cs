using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_with_later_start_date : ImportLearnerDetailHandlerTestsBase
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
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                LearnerTwo.FundingModel, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate.AddDays(10), LearnerTwo.PlannedEndDate, LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(Request, Times.Once);
        }

        [Test]
        public async Task Then_result_is_replace()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                LearnerTwo.FundingModel, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate.AddDays(10), LearnerTwo.PlannedEndDate, LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("ReplacedLearnerDetail");
        }
    }
}