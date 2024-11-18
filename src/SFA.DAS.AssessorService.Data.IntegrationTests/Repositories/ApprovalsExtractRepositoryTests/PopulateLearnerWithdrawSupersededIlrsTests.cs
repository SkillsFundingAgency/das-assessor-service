using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.ApprovalsExtractRepositoryTests
{
    public class PopulateLearnerWithdrawSupersededIlrsTests : TestBase
    {
        [TestCase(456, 123, 123, 3, 2)]
        [TestCase(456, 123, 456, 2, 3)]
        public async Task PopulatedLearner_WithdrawSupersededIlrs_WhenCalled_Then_NonCurrentIlrIsWithdrawn(
            int firstStdCode, int secondStdCode, int approvalExtractTrainingCode, int firstCompletionStatus, int secondCompletionStatus)
        {
            var currentDateTime = DateTime.Now;

            var plannedEndDateTime = currentDateTime.AddMonths(12);
            var learnStartDateTime = currentDateTime.AddDays(50);

            var uln = 111111111;
            var ukprn = 22222222;
            var apprenticeshipId = 33333333;

            var ilrIdOne = Guid.NewGuid();
            var ilrIdTwo = Guid.NewGuid();

            using (var fixture = new PopulateLearnerWithdrawSupersededIlrsTestsFixture()
                .WithStandard("Standard 1", "ST0001", firstStdCode, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithStandard("Standard 2", "ST0002", secondStdCode, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithIlr(ilrIdOne, uln, "Alice", "Broom", ukprn, firstStdCode, learnStartDateTime, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime)
                .WithIlr(ilrIdTwo, uln, "Alice", "Broom", ukprn, secondStdCode, learnStartDateTime, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime)
                .WithLearner(ilrIdOne, uln, "Alice", "Broom", apprenticeshipId, firstStdCode, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow))
                .WithLearner(ilrIdTwo, uln, "Alice", "Broom", apprenticeshipId, secondStdCode, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow))
                .WithApprovalsExtract(apprenticeshipId, "Alice", "Broom", uln.ToString(), approvalExtractTrainingCode, "1.0", false, null, "ST0001_1.0", currentDateTime, null, currentDateTime, currentDateTime, null, null, null, ukprn, "LEARN123", 1, 12345, "Bob"))
            {
                var results = fixture.ExecPopulateLearner_WithdrawSupersededIlrs();

                var expectedIlrOne = IlrHandler.Create(ilrIdOne, uln, "Alice", "Broom", ukprn, firstStdCode, learnStartDateTime,
                    36, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, firstCompletionStatus, plannedEndDateTime);

                var expectedIlrTwo = IlrHandler.Create(ilrIdTwo, uln, "Alice", "Broom", ukprn, secondStdCode, learnStartDateTime,
                    36, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, secondCompletionStatus, plannedEndDateTime);

                await results.VerifyIlrRowCount(2);
                await results.VerifyIlrExists(expectedIlrOne);
                await results.VerifyIlrExists(expectedIlrTwo);
            }
        }

        [Test]
        public async Task PopulatedLearner_WithdrawSupersededIlrs_WhenCalled_Then_CurrentIlrIsUnaffected()
        {
            var currentDateTime = DateTime.Now;

            var plannedEndDateTime = currentDateTime.AddMonths(12);
            var learnStartDateTime = currentDateTime.AddDays(50);

            var uln = 121212121;
            var ukprn = 32323232;
            var apprenticeshipId = 44444444;

            var ilrIdOne = Guid.NewGuid();

            using (var fixture = new PopulateLearnerWithdrawSupersededIlrsTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithIlr(ilrIdOne, uln, "Carol", "Duster", ukprn, 123, learnStartDateTime, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime)
                .WithLearner(ilrIdOne, uln, "Carol", "Duster", apprenticeshipId, 123, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow))
                .WithApprovalsExtract(apprenticeshipId, "Carol", "Duster", uln.ToString(), 123, "1.0", false, null, "ST0001_1.0", currentDateTime, null, currentDateTime, currentDateTime, null, null, null, ukprn, "LEARN123", 1, 12345, "Bob"))
            {
                var results = fixture.ExecPopulateLearner_WithdrawSupersededIlrs();

                var expectedIlrOne = IlrHandler.Create(ilrIdOne, uln, "Carol", "Duster", ukprn, 123, learnStartDateTime,
                    36, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime);

                await results.VerifyIlrRowCount(1);
                await results.VerifyIlrExists(expectedIlrOne);
            }
        }

        private class PopulateLearnerWithdrawSupersededIlrsTestsFixture : FixtureBase<PopulateLearnerWithdrawSupersededIlrsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();

            public PopulateLearnerWithdrawSupersededIlrsTestsFixture()
            {
            }

            public PopulateLearnerWithdrawSupersededIlrsTestsFixture ExecPopulateLearner_WithdrawSupersededIlrs()
            {
                _databaseService.Execute("EXEC [dbo].[PopulateLearner_WithdrawSupersededIlrs]");
                return this;
            }

            public override void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
            }
        }
    }
}
