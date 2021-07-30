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

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplicationServiceTests
{
    [TestFixture]
    public class When_BuildStandardWithdrawalRequest_is_called
    {
        private ApplicationService _sut;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IApplicationApiClient> _mockApplicatonApiClient;
        private Mock<ILearnerDetailsApiClient> _mockLearnerDetailsApiClient;
        private Mock<IOrganisationsApiClient> _mockOrganisationsApiClient;

        private DateTime _earliestWithdrawalDate = DateTime.Now.AddMonths(6);
        private int _pipelinesCount = 77;

        [SetUp]
        public void Arrange()
        {
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockApplicatonApiClient = new Mock<IApplicationApiClient>();
            _mockLearnerDetailsApiClient = new Mock<ILearnerDetailsApiClient>();
            _mockOrganisationsApiClient = new Mock<IOrganisationsApiClient>();

            _mockQnaApiClient
                .Setup(r => r.StartApplications(It.IsAny<StartApplicationRequest>()))
                .ReturnsAsync(new StartApplicationResponse { });

            _mockQnaApiClient
                .Setup(r => r.GetAllApplicationSequences(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Sequence> { });

            _mockLearnerDetailsApiClient
                .Setup(r => r.GetPipelinesCount(It.IsAny<string>(), It.IsAny<int?>()))
                .ReturnsAsync(_pipelinesCount);

            _mockOrganisationsApiClient
                .Setup(r => r.GetEarliestWithdrawalDate(It.IsAny<Guid>(), It.IsAny<int?>()))
                .ReturnsAsync(_earliestWithdrawalDate);

            _sut = new ApplicationService(_mockQnaApiClient.Object, _mockApplicatonApiClient.Object, _mockLearnerDetailsApiClient.Object, _mockOrganisationsApiClient.Object);
        }
        
        [Test]
        public async Task Then_GetPipelinesCount_is_called()
        {
            // Arrange
            string endPointAssessorOrganisationId = "EPA0200";
            int standardCode = 287;

            // Act
            await _sut.BuildStandardWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse 
                { 
                    Id = Guid.NewGuid(),
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, standardCode, string.Empty, string.Empty);

            // Assert
            _mockLearnerDetailsApiClient
                .Verify(r => r.GetPipelinesCount(endPointAssessorOrganisationId, standardCode), Times.Once);
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
                }, standardCode, string.Empty, string.Empty);

            // Assert
            _mockQnaApiClient
                .Verify(r => r.StartApplications(It.Is<StartApplicationRequest>(p => 
                JsonConvert.DeserializeObject<ApplicationData>(p.ApplicationData).PipelinesCount == _pipelinesCount)));
        }

        [Test]
        public async Task Then_GetEarliestWithdrawalDate_is_called()
        {
            // Arrange
            Guid organisationId = Guid.NewGuid();
            string endPointAssessorOrganisationId = "EPA0200";
            int standardCode = 287;

            // Act
            await _sut.BuildStandardWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse
                {
                    Id = organisationId,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, standardCode, string.Empty, string.Empty);

            // Assert
            _mockOrganisationsApiClient
                .Verify(r => r.GetEarliestWithdrawalDate(organisationId, standardCode), Times.Once);
        }

        [Test]
        public async Task Then_ApplicationData_contains_EarliestWithdrawalDate()
        {
            // Arrange
            Guid organisationId = Guid.NewGuid();
            string endPointAssessorOrganisationId = "EPA0200";
            int standardCode = 287;

            // Act
            var result = await _sut.BuildStandardWithdrawalRequest(
                new ContactResponse { Id = Guid.NewGuid() },
                new OrganisationResponse
                {
                    Id = organisationId,
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId,
                    EndPointAssessorName = "Organisation Limited"
                }, standardCode, string.Empty, string.Empty);

            // Assert
            _mockQnaApiClient
                .Verify(r => r.StartApplications(It.Is<StartApplicationRequest>(p => 
                JsonConvert.DeserializeObject<ApplicationData>(p.ApplicationData).EarliestDateOfWithdrawal == _earliestWithdrawalDate)));
        }

        [Test]
        public async Task Then_CreateApplication_returns_StandardWithdrawal()
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
                }, standardCode, string.Empty, string.Empty);

            // Assert
            result.ApplicationType.Should().Be(ApplicationTypes.StandardWithdrawal);
        }
    }
}