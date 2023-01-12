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
    public class When_called_exist_same_source_and_ukprn_later_start_date : ImportLearnerDetailHandlerTestsBase
    {
        public void Arrange(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithCertificate);
            ImportLearnerDetail.EpaOrgId = epaOrgId;
            ImportLearnerDetail.LearnStartDate = ImportLearnerDetail.LearnStartDate.Value.AddDays(10);
            ImportLearnerDetail.LearnActEndDate = learnActEndDate == null ? (DateTime?)null : DateTime.Parse(learnActEndDate, CultureInfo.InvariantCulture);
            ImportLearnerDetail.WithdrawReason = withdrawReason;
            ImportLearnerDetail.Outcome = outcome;
            ImportLearnerDetail.AchDate = achDate == null ? (DateTime?)null : DateTime.Parse(achDate, CultureInfo.InvariantCulture);
            ImportLearnerDetail.OutGrade = outGrade;

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_an_existing_learner_record_is_replaced(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Arrange(epaOrgId, learnActEndDate, withdrawReason, outcome, achDate, outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(ImportLearnerDetail, Times.Once);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_result_is_replace(string epaOrgId, string learnActEndDate, int? withdrawReason,
            int? outcome, string achDate, string outGrade)
        {
            // Arrange
            Arrange(epaOrgId, learnActEndDate, withdrawReason, outcome, achDate, outGrade);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("ReplacedLearnerDetail");
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