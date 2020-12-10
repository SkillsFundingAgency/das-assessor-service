using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Helpers;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyControllerTests.ApplyForWithdrawal
{
    [TestFixture]
    public class When_BuildCombindedRequest_is_called
    {
        private ApplicationService _sut;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<ILearnerDetailsApiClient> _mockLearnerDetailsApiClient;
        private Mock<IDateTimeHelper> _mockDateTimeHelper;

        [SetUp]
        public void Arrange()
        {
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockLearnerDetailsApiClient = new Mock<ILearnerDetailsApiClient>();
            _mockDateTimeHelper = new Mock<IDateTimeHelper>();

            _mockQnaApiClient
                .Setup(r => r.StartApplications(It.IsAny<StartApplicationRequest>()))
                .ReturnsAsync(new StartApplicationResponse { });

            _mockQnaApiClient
                .Setup(r => r.GetAllApplicationSequences(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Sequence> { });

            _mockLearnerDetailsApiClient
                .Setup(r => r.GetPipelinesCount(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(10);

            _sut = new ApplicationService(_mockQnaApiClient.Object, _mockLearnerDetailsApiClient.Object, _mockDateTimeHelper.Object);
        }
        
        [Test]
        public async Task Then_CreateApplication_returns_Combinded()
        {
            // Arrange
            string endPointAssessorOrganisationId = "EPA0200";

            // Act
            var result = await _sut.BuildCombinedRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, string.Empty);

            // Assert
            result.ApplicationType.Should().Be(ApplicationTypes.Combined);
        }
    }
}