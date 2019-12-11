using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_with_dummy_uln : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();
        }

        [TestCase(9999999999)]
        [TestCase(1000000000)]
        public async Task Then_no_learner_records_are_created(long uln)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne);
            Request.Uln = uln;

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCase(9999999999)]
        [TestCase(1000000000)]
        public async Task Then_no_learner_records_are_updated(long uln)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne);
            Request.Uln = uln;

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<Ilr>()), Times.Never);
        }

        [TestCase(9999999999)]
        [TestCase(1000000000)]
        public async Task Then_result_is_ignore_dummy_uln(long uln)
        {
            // Arrange
            Request = CreateImportLearnerDetailRequest(LearnerOne);
            Request.Uln = uln;

            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("IgnoreUlnDummyValue");
        }
    }
}