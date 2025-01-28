using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.ApprovalsExtractRepositoryTests
{
    public class PopulateLearnerWithdrawOverlappingIlrsAndLearnersTests : TestBase
    {
        [TestCase(1, 6, null, 1, 1)]
        [TestCase(1, 6, null, 1, 0, true)]
        [TestCase(1, 6, null, -1, 1)]
        [TestCase(1, 6, null, -1, 0, true)]
        [TestCase(1, 6, -7, -7, 2)]
        [TestCase(1, 6, -7, -7, 0, true)]
        [TestCase(1, 6, -6, -6, 2)]
        [TestCase(1, 6, -6, -6, 0, true)]
        [TestCase(1, 6, -5, -5, 2)]
        [TestCase(1, 6, -5, -5, 0, true)]
        [TestCase(1, 6, -4, -4, 2)]
        [TestCase(1, 6, -4, -4, 0, true)]
        [TestCase(1, null, -1, 1, 3)]
        [TestCase(1, null, -1, 1, 0, true)]
        [TestCase(1, null, 1, -1, 3)]
        [TestCase(1, null, 1, -1, 0, true)]
        [TestCase(1, 6, 4, 4, 4)]
        [TestCase(1, 6, 4, 4, 0, true)]
        [TestCase(1, 6, 5, 5, 4)]
        [TestCase(1, 6, 5, 5, 0, true)]
        [TestCase(1, 6, 6, 6, 4)]
        [TestCase(1, 6, 6, 6, 0, true)]
        [TestCase(1, 6, 7, 7, 4)]
        [TestCase(1, 6, 7, 7, 0, true)]
        [TestCase(1, 6, 0, -1, 5)]
        [TestCase(1, 6, 0, -1, 0, true)]
        [TestCase(1, 6, 0, 1, 5)]
        [TestCase(1, 6, 0, 1, 0, true)]
        [TestCase(1, 6, -1, -1, 6)]
        [TestCase(1, 6, -1, -1, 0, true)]
        [TestCase(1, 6, 1, 1, 7)]
        [TestCase(1, 6, 1, 1, 0, true)]
        [TestCase(1, 6, -1, 1, 6)]
        [TestCase(1, 6, -1, 1, 0, true)]
        [TestCase(1, 6, 1, -1, 7)]
        [TestCase(1, 6, 1, -1, 0, true)]
        public async Task PopulatedLearner_WithdrawOverlappingIlrsAndLearners_WhenCalled(int learnStartOffset, int? endOffset, int? nextLearnStartOffset, int nextEndOffset, int possibleOverlap, bool hasCertificate = false)
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            var learnStart = currentDateTime.AddMonths(learnStartOffset);
            var end = endOffset != null ? currentDateTime.AddMonths(endOffset.Value) : (DateTime?)null;

            var nextLearnStart = nextLearnStartOffset != null ? learnStart.AddMonths(nextLearnStartOffset.Value) : (DateTime?)null;
            var nextEnd = end.HasValue ? end.Value.AddMonths(nextEndOffset) : (DateTime?)null;
            
            var uln = 111111111;
            var givenNames = "Alice";
            var familyName = "Broom";
            var ukprn = 22222222;

            var ilrId = Guid.NewGuid();
            var ilrIdNext = Guid.NewGuid();

            using (var fixture = new PopulateLearnerWithdrawOverlappingIlrsAndLearnersTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithStandard("Standard 1", "ST00001", 111, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithStandard("Standard 2", "ST00002", 222, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithIlr(ilrId, uln, givenNames, familyName, ukprn, 111, learnStart, HandlerBase.GetAcademicYear(currentDateTime), currentDateTime, 1, end)
                .WithIlr(ilrIdNext, uln, givenNames, familyName, ukprn, 222, nextLearnStart, HandlerBase.GetAcademicYear(currentDateTime), currentDateTime.AddDays(1), 1, nextEnd))
            {
                if (hasCertificate)
                {
                    fixture.WithCertificate(Guid.NewGuid(), currentDateTime, uln, 111, "EPA0001");
                }
                
                var results = await fixture.PopulateLearner_WithdrawOverlappingIlrsAndLearners();
                results.VerifyOverlappingIlrsRowCount(2);
                results.VerifyOverlappingIlrs(ilrId, possibleOverlap);
                results.VerifyOverlappingIlrs(ilrIdNext, 1);
            }
        }

        private class PopulateLearnerWithdrawOverlappingIlrsAndLearnersTestsFixture : FixtureBase<PopulateLearnerWithdrawOverlappingIlrsAndLearnersTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private List<OverlappingIlrs> _overlappingIlrsResults;

            public async Task<PopulateLearnerWithdrawOverlappingIlrsAndLearnersTestsFixture> PopulateLearner_WithdrawOverlappingIlrsAndLearners()
            {
                _overlappingIlrsResults = await _databaseService.ExecuteStoredProcedure<OverlappingIlrs>(
                    "[dbo].[PopulateLearner_WithdrawOverlappingIlrsAndLearners]",
                    new { TestMode = true }
                );

                return this;
            }

            public void VerifyOverlappingIlrsRowCount(int rowCount)
            {
                _overlappingIlrsResults.Count.Should().Be(rowCount);
            }

            public void VerifyOverlappingIlrs(Guid id, int possibleOverlap)
            {
                _overlappingIlrsResults.Should().Contain(p => p.Id == id && p.PossibleOverlap == possibleOverlap);
            }
            public override void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
            }

            private class OverlappingIlrs
            {
                public Guid Id { get; } = Guid.Empty;
                public int PossibleOverlap { get; } = 0;
            }
        }
    }
}
