using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_without_certficate_and_funding_model_99 : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [Test]
        public async Task Then_learner_records_are_unchanged()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 44444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                99, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate,
                LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);
        }


        [Test]
        public async Task Then_result_is_ignore_ukprn_changed_for_funding_model_99()
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerTwo.Source, 4444444444, LearnerTwo.Uln, LearnerTwo.StdCode,
                99, LearnerTwo.GivenNames, LearnerTwo.FamilyName, LearnerTwo.EpaOrgId,
                LearnerTwo.LearnStartDate, LearnerTwo.PlannedEndDate,
                LearnerTwo.CompletionStatus, LearnerTwo.LearnRefNumber, LearnerTwo.DelLocPostCode,
                LearnerTwo.LearnActEndDate, LearnerTwo.WithdrawReason, LearnerTwo.Outcome, LearnerTwo.AchDate, LearnerTwo.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("IgnoreFundingModelChangedTo99WhenPrevioulsyNot99");
        }
    }
}