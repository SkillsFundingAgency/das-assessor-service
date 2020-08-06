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
    public class When_called_for_existing_learner_having_same_source_and_different_ukprn_remove_learn_act_value : ImportLearnerDetailHandlerTestsBase
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
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerFive);
            ImportLearnerDetail.Ukprn = 444444444;

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
            VerifyIlrReplaced(ImportLearnerDetail, Times.Once);
        }

        [Test]
        public async Task Then_result_is_replace()
        {
            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerFive);
            ImportLearnerDetail.Ukprn = 444444444;

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