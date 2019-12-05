using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_same_source_and_ukprn_with_out_of_date_data : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase(-1, 0)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        [TestCase(0, -1)]
        [TestCase(0, 1)]
        public async Task Then_learner_records_are_unchanged(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode,
                LearnerOne.FundingModel, LearnerOne.GivenNames, LearnerOne.FamilyName, LearnerOne.EpaOrgId,
                LearnerOne.LearnStartDate.AddDays(learnStartDateAddDays),
                LearnerOne.PlannedEndDate.Value.AddDays(plannedEndDateAddDays),
                LearnerOne.CompletionStatus, LearnerOne.LearnRefNumber, LearnerOne.DelLocPostCode,
                LearnerOne.LearnActEndDate, LearnerOne.WithdrawReason, LearnerOne.Outcome, LearnerOne.AchDate, LearnerOne.OutGrade);

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


        [TestCase(-1, 0)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        [TestCase(0, -1)]
        [TestCase(0, 1)]
        public async Task Then_result_is_ignore_out_date(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne.Source, LearnerOne.UkPrn, LearnerOne.Uln, LearnerOne.StdCode,
                LearnerOne.FundingModel, LearnerOne.GivenNames, LearnerOne.FamilyName, LearnerOne.EpaOrgId,
                LearnerOne.LearnStartDate.AddDays(learnStartDateAddDays),
                LearnerOne.PlannedEndDate.Value.AddDays(plannedEndDateAddDays),
                LearnerOne.CompletionStatus, LearnerOne.LearnRefNumber, LearnerOne.DelLocPostCode,
                LearnerOne.LearnActEndDate, LearnerOne.WithdrawReason, LearnerOne.Outcome, LearnerOne.AchDate, LearnerOne.OutGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("IgnoreOutOfDate");
        }
    }
}