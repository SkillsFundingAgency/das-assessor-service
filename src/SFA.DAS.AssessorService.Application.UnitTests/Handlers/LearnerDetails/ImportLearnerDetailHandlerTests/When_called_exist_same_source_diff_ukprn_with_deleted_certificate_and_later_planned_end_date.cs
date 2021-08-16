using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_exist_same_source_diff_ukprn_with_deleted_certificate_and_later_planned_end_date : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithDeletedCertificate);
            ImportLearnerDetail.Ukprn = ImportLearnerDetail.Ukprn + 1;
            ImportLearnerDetail.LearnStartDate = ImportLearnerDetail.LearnStartDate.Value.AddDays(500);
            ImportLearnerDetail.PlannedEndDate = ImportLearnerDetail.PlannedEndDate.Value.AddDays(20);
            ImportLearnerDetail.LearnActEndDate = ImportLearnerDetail.LearnActEndDate.Value.AddDays(200);
            ImportLearnerDetail.CompletionStatus = 2;
            ImportLearnerDetail.Outcome = 1;
            ImportLearnerDetail.WithdrawReason = null;
            ImportLearnerDetail.AchDate = DateTime.Now.AddDays(-190);

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };
        }

        [Test]
        public async Task Then_learner_records_are_not_created()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Once);
        }


        [Test]
        public async Task Then_result_is_replace()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("ReplacedLearnerDetail");
        }
    }
}