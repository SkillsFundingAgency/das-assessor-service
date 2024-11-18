using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.ApprovalsExtractRepositoryTests
{
    public class PopulateLearnerRemoveSupersededLearnersTests : TestBase
    {
        [TestCase(456, 123, 123, 123)]
        [TestCase(456, 123, 456, 456)]
        public async Task PopulatedLearner_RemoveSupersededLearners_WhenCalled_Then_NonCurrentLearnerIsRemoved(
            int firstStdCode, int secondStdCode, int approvalExtractStdCode, int expectedStdCode)
        {
            var currentDateTime = DateTime.Now;

            var plannedEndDateTime = currentDateTime.AddMonths(12);
            var learnStartDateTime = currentDateTime.AddDays(50);

            var uln = 111111111;
            var givenNames = "Alice";
            var familyName = "Broom";
            var ukprn = 22222222;
            var apprenticeshipId = 33333333;

            var standards = new Dictionary<int, (string title, string referenceNumber, string version, string standardUid)>()
            {
                { firstStdCode, ("Standard 1", "ST00001", "1.0", "ST00001_1.0") },
                { secondStdCode, ("Standard 2", "ST00002", "1.0", "ST00002_1.0") }
            };

            var ilrId = new Dictionary<int, Guid>
            {
                { firstStdCode, Guid.NewGuid() },
                { secondStdCode, Guid.NewGuid() }
            };

            using (var fixture = new PopulateLearnerRemoveSupersededLearnerTestsFixture()
                .WithStandard(standards[firstStdCode].title, standards[firstStdCode].referenceNumber, firstStdCode, standards[firstStdCode].version, currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithStandard(standards[secondStdCode].title, standards[secondStdCode].referenceNumber, secondStdCode, standards[secondStdCode].version, currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithIlr(ilrId[firstStdCode], uln, givenNames, familyName, ukprn, firstStdCode, learnStartDateTime, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime)
                .WithIlr(ilrId[secondStdCode], uln, givenNames, familyName, ukprn, secondStdCode, learnStartDateTime, HandlerBase.GetAcademicYear(DateTime.UtcNow), currentDateTime, 2, plannedEndDateTime)
                .WithLearner(ilrId[firstStdCode], uln, givenNames, familyName, ukprn, firstStdCode, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow))
                .WithLearner(ilrId[secondStdCode], uln, givenNames, familyName, ukprn, secondStdCode, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow))
                .WithApprovalsExtract(apprenticeshipId, givenNames, familyName, uln.ToString(), approvalExtractStdCode, standards[approvalExtractStdCode].version, false, null, standards[approvalExtractStdCode].standardUid, currentDateTime, null, currentDateTime, currentDateTime, null, null, null, ukprn, "LEARN123", 1, 12345, "Bob"))
            {
                var results = fixture.ExecPopulateLearner_RemoveSupersededLearners();

                var expectedLearner = LearnerHandler.Create(ilrId[expectedStdCode], uln, givenNames, familyName, ukprn, expectedStdCode, apprenticeshipId, HandlerBase.GetAcademicYear(DateTime.UtcNow));

                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expectedLearner);
            }
        }

        private class PopulateLearnerRemoveSupersededLearnerTestsFixture : FixtureBase<PopulateLearnerRemoveSupersededLearnerTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();

            public PopulateLearnerRemoveSupersededLearnerTestsFixture ExecPopulateLearner_RemoveSupersededLearners()
            {
                _databaseService.Execute("EXEC [dbo].[PopulateLearner_RemoveSupersededLearners]");
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
