using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Learner
{
    [TestFixture]
    public class When_called_for_existing_learner_having_previous_source : ImportLearnerDetailHandlerTestsBase
    {
        [SetUp]
        public void Arrange()
        {
            BaseArrange();

            Request = CreateImportLearnerDetailRequest(LearnerThree);
            Request.Source = "1920"; // cannot revert from 2021 to 1920
        }

        [Test]
        public async Task Then_learner_records_are_unchanged()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            IlrRepository.Verify(r => r.Create(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);

            // Assert
            IlrRepository.Verify(r => r.Update(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int?>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<DateTime?>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Then_result_is_ignore_Lower_source()
        {
            // Act
            Response = await Sut.Handle(Request, new CancellationToken());

            // Assert
            Response.Result.Should().Be("IgnoreSourcePriorToCurrentSource");
        }
    }
}