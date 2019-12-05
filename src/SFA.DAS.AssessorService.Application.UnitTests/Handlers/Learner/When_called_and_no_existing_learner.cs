using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Learner;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_and_no_existing_learner : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            Request = CreateImportLearnerDetailRequest(null);
        }

        [Test]
        public async Task Then_a_new_learner_record_is_created()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<string>(), It.IsAny<long>(), Request.Uln.Value, Request.StdCode.Value, It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Then_result_is_create()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("CreatedLearnerDetail");
        }
    }
}