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
    public class When_called_and_no_existing_learner : ImportLearnerDetailHandlerTestsBase
    {
        private ImportLearnerDetail SecondImportLearnerDetail;
        
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            // Arrange
            ImportLearnerDetail = CreateImportLearnerDetail(4444, 44444444444, 44);
            SecondImportLearnerDetail = CreateImportLearnerDetail(5555, 55555555555, 55);

            Request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail,
                    SecondImportLearnerDetail
                }
            };
        }

        [Test]
        public async Task Then_a_new_learner_record_is_created()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.Is<Ilr>(p => p.Uln == ImportLearnerDetail.Uln.Value && p.StdCode == ImportLearnerDetail.StdCode.Value)), Times.Once);
            IlrRepository.Verify(r => r.Create(It.Is<Ilr>(p => p.Uln == SecondImportLearnerDetail.Uln.Value && p.StdCode == SecondImportLearnerDetail.StdCode.Value)), Times.Once);
        }

        [Test]
        public async Task Then_result_is_create()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(2);
            Response.LearnerDetailResults[0].Outcome.Should().Be("CreatedLearnerDetail");
            Response.LearnerDetailResults[1].Outcome.Should().Be("CreatedLearnerDetail");
        }
    }
}