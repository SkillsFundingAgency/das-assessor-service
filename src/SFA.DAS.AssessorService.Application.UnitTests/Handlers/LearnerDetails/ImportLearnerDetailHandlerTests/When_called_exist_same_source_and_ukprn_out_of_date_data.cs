using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_exist_same_source_and_ukprn_out_of_date_data : ImportLearnerDetailHandlerTestsBase
    {
        public void Arrange(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithCertificate);
            ImportLearnerDetail.LearnStartDate = ImportLearnerDetail.LearnStartDate.Value.AddDays(learnStartDateAddDays);
            ImportLearnerDetail.PlannedEndDate = ImportLearnerDetail.PlannedEndDate.Value.AddDays(plannedEndDateAddDays);

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_learner_records_are_not_created(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            // Arrange
            Arrange(learnStartDateAddDays, plannedEndDateAddDays);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_learner_records_are_not_updated(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            // Arrange
            Arrange(learnStartDateAddDays, plannedEndDateAddDays);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCaseSource(nameof(TestSource))]
        public async Task Then_result_is_ignore_out_date(int learnStartDateAddDays, int plannedEndDateAddDays)
        {
            // Arrange
            Arrange(learnStartDateAddDays, plannedEndDateAddDays);

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("IgnoreOutOfDate");
        }

        static IEnumerable<object[]> TestSource()
        {
            return new[]
            {
                new object[] { -1, 0 },
                new object[] { -1, 1 },
                new object[] { -1, -1 },
                new object[] { 0, -1 },
                new object[] { 0, 1 }
            };
        }
    }
}