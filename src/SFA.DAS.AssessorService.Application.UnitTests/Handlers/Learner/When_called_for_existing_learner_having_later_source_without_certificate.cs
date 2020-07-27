using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_later_source_without_certificate : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            ImportLearnerDetail = CreateImportLearnerDetail(LearnerTwo);

            // a later source to which should trigger a replacement, the replacement values are null
            ImportLearnerDetail.Source = "2021";
            ImportLearnerDetail.EpaOrgId = null;
            ImportLearnerDetail.LearnActEndDate = null;
            ImportLearnerDetail.WithdrawReason = null;
            ImportLearnerDetail.Outcome = null;
            ImportLearnerDetail.AchDate = null;
            ImportLearnerDetail.OutGrade = null;
        }

        [Test]
        public async Task Then_an_existing_learner_record_is_replaced()
        {
            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            VerifyIlrReplaced(request.ImportLearnerDetails[0], Times.Once);
        }

        [Test]
        public async Task Then_result_is_replacement()
        {
            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("ReplacedLearnerDetail");
        }
    }
}