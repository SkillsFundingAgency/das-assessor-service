using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_Search_Is_Called
    {
        private StandardController _sut;
        private Mock<IApplicationApiClient> _mockApiClient;
        private Mock<IOrganisationsApiClient> _mockOrgApiClient;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IContactsApiClient> _mockContactsApiClient;
        private Mock<IStandardVersionClient> _mockStandardVersionApiClient;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [SetUp]
        public void Arrange()
        {
            _mockApiClient = new Mock<IApplicationApiClient>();
            _mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockStandardVersionApiClient = new Mock<IStandardVersionClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            _mockApiClient
             .Setup(r => r.GetApplication(It.IsAny<Guid>()))
             .ReturnsAsync(new ApplicationResponse());

            _mockOrgApiClient
             .Setup(r => r.GetEpaOrganisation(It.IsAny<String>()))
             .ReturnsAsync(new EpaOrganisation());

            _mockOrgApiClient
             .Setup(r => r.GetOrganisationStandardsByOrganisation(It.IsAny<String>()))
             .ReturnsAsync(new List<OrganisationStandardSummary>());

            _sut = new StandardController(_mockApiClient.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
               _mockContactsApiClient.Object, _mockStandardVersionApiClient.Object, _mockHttpContextAccessor.Object);
        }

        [Test]
        public async Task Then_The_Results_Are_Grouped()
        {
            // Arrange
            _mockStandardVersionApiClient
               .Setup(r => r.GetAllStandardVersions())
               .ReturnsAsync(new List<StandardVersion> { 
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.0"},
                   new StandardVersion { IFateReferenceNumber = "ST0001", Title = "Title 1", Version = "1.1"},
                   new StandardVersion { IFateReferenceNumber = "ST0002", Title = "Title 2", Version = "1.0"},
               });

            // Act
            var results = (await _sut.Search(new StandardVersionViewModel { StandardToFind = "Title" })) as ViewResult;

            // Assert
            var vm = results.Model as StandardVersionViewModel;
            Assert.AreEqual(2, vm.Results.Count);
            Assert.AreEqual("ST0001", vm.Results[0].IFateReferenceNumber);
            Assert.AreEqual("ST0002", vm.Results[1].IFateReferenceNumber);
        }
    }
}
