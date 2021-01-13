using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_and_no_existing_learner : ImportLearnerDetailHandlerTestsBase
    {
        private ImportLearnerDetail _secondImportLearnerDetail;
        
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            ImportLearnerDetail = CreateImportLearnerDetail(44444);
            _secondImportLearnerDetail = CreateImportLearnerDetail(55555);
        }

        [Test]
        public async Task Then_a_new_learner_record_is_created()
        {
            // Arrange
            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail,
                    _secondImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.Is<Ilr>(p => p.Uln == ImportLearnerDetail.Uln.Value && p.StdCode == ImportLearnerDetail.StdCode.Value)), Times.Once);
            IlrRepository.Verify(r => r.Create(It.Is<Ilr>(p => p.Uln == _secondImportLearnerDetail.Uln.Value && p.StdCode == _secondImportLearnerDetail.StdCode.Value)), Times.Once);
        }

        [Test]
        public async Task Then_result_is_create()
        {
            // Arrange
            ImportLearnerDetailRequest request = new ImportLearnerDetailRequest
            {
                ImportLearnerDetails = new List<ImportLearnerDetail>
                {
                    ImportLearnerDetail,
                    _secondImportLearnerDetail
                }
            };

            // Act
            Response = await Sut.Handle(request, new CancellationToken());

            // Assert
            Response.LearnerDetailResults.Count.Should().Be(2);
            Response.LearnerDetailResults[0].Outcome.Should().Be("CreatedLearnerDetail");
            Response.LearnerDetailResults[1].Outcome.Should().Be("CreatedLearnerDetail");
        }
    }
}