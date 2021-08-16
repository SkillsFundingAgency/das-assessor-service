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
    public class When_called_exist_same_source_diff_ukprn_fund_99 : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(LearnerWithoutCertificate);
            ImportLearnerDetail.Ukprn = ImportLearnerDetail.Ukprn + 1;
            ImportLearnerDetail.FundingModel = 99;

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
        public async Task Then_learner_records_are_not_updated()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }


        [Test]
        public async Task Then_result_is_ignore_ukprn_changed_for_funding_model_99()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(1);
            Response.LearnerDetailResults[0].Outcome.Should().Be("IgnoreFundingModelChangedTo99WhenPrevioulsyNot99");
        }
    }
}