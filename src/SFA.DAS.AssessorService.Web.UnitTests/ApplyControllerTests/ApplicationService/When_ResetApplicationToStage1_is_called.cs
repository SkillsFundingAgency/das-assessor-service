using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyControllerTests.ApplyForWithdrawal
{
    [TestFixture]
    public class When_ResetApplicationToStage1_is_called
    {
        private ApplicationService _sut;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IApplicationApiClient> _mockApplicationApiClient;
        private Mock<ILearnerDetailsApiClient> _mockLearnerDetailsApiClient;
        private Mock<IOrganisationsApiClient> _mockOrganisationsApiClient;

        [SetUp]
        public void Arrange()
        {
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockApplicationApiClient = new Mock<IApplicationApiClient>();
            _mockLearnerDetailsApiClient = new Mock<ILearnerDetailsApiClient>();
            _mockOrganisationsApiClient = new Mock<IOrganisationsApiClient>();

            _sut = new ApplicationService(_mockQnaApiClient.Object, _mockApplicationApiClient.Object,
                _mockLearnerDetailsApiClient.Object, _mockOrganisationsApiClient.Object);
        }
        
        [Test]
        public async Task Then_ResetApplicationToStage1_calls_ApiToResetApplicationToStage1()
        {
            // Arrange
            _mockApplicationApiClient
                .Setup(r => r.GetApplication(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationResponse { Id = Guid.NewGuid(), ApplicationId = Guid.NewGuid() });

            _mockApplicationApiClient
                .Setup(r => r.ResetApplicationToStage1(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _mockQnaApiClient
                .Setup(r => r.ResetSectionAnswers(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResetPageAnswersResponse { ValidationPassed = true, HasPageAnswersBeenReset = true });

            // Act
            var result = await _sut.ResetApplicationToStage1(
                Guid.NewGuid());

            // Assert
            _mockApplicationApiClient.Verify(p => p.ResetApplicationToStage1(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task Then_ResetApplicationToStage1_calls_QnaApiToResetSectionAnswers()
        {
            // Arrange
            _mockApplicationApiClient
                .Setup(r => r.GetApplication(It.IsAny<Guid>()))
                .ReturnsAsync(new ApplicationResponse { Id = Guid.NewGuid(), ApplicationId = Guid.NewGuid() });

            _mockApplicationApiClient
                .Setup(r => r.ResetApplicationToStage1(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _mockQnaApiClient
                .Setup(r => r.ResetSectionAnswers(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new ResetPageAnswersResponse { ValidationPassed = true, HasPageAnswersBeenReset = true });

            // Act
            var result = await _sut.ResetApplicationToStage1(
                Guid.NewGuid());

            // Assert
            _mockQnaApiClient.Verify(p => p.ResetSectionAnswers(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task Then_ResetApplicationToStage1_does_not_calls_QnaApiToResetSectionAnswers()
        {
            // Arrange
            _mockApplicationApiClient
                .Setup(r => r.ResetApplicationToStage1(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.ResetApplicationToStage1(
                Guid.NewGuid());

            // Assert
            _mockQnaApiClient.Verify(p => p.ResetSectionAnswers(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }
    }
}