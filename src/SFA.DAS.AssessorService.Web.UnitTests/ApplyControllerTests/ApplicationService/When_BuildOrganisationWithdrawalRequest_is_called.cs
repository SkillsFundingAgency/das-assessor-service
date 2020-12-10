using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
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
    public class When_BuildOrganisationWithdrawalRequest_is_called
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
                .ReturnsAsync(33);

            _sut = new ApplicationService(_mockQnaApiClient.Object, _mockLearnerDetailsApiClient.Object, _mockDateTimeHelper.Object);
        }
        
        [Test]
        public async Task Then_GetPipelinesCount_is_called()
        {
            // Arrange
            string endPointAssessorOrganisationId = "EPA0200";

            // Act
            await _sut.BuildOrganisationWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse 
                { 
                    Id = Guid.NewGuid(),
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, string.Empty);

            // Assert
            _mockLearnerDetailsApiClient
                .Verify(r => r.GetPipelinesCount(endPointAssessorOrganisationId, null), Times.Once);
        }

        [Test]
        public async Task Then_ApplicationData_contains_PipelinesCount()
        {
            // Arrange
            string endPointAssessorOrganisationId = "EPA0200";
            int standardCode = 287;

            // Act
            var result = await _sut.BuildStandardWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, standardCode, string.Empty);

            // Assert
            _mockQnaApiClient
                .Verify(r => r.StartApplications(It.Is<StartApplicationRequest>(p => JsonConvert.DeserializeObject<ApplicationData>(p.ApplicationData).PipelinesCount == 33)));
        }


        [Test]
        public async Task Then_CreateApplication_returns_OrganisationWithdrawal()
        {
            // Arrange
            string endPointAssessorOrganisationId = "EPA0200";

            // Act
            var result = await _sut.BuildOrganisationWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, string.Empty);

            // Assert
            result.ApplicationType.Should().Be(ApplicationTypes.OrganisationWithdrawal);
        }
    }
}