using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.QnA;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.StandardControllerTests
{
    [TestFixture]
    public class When_OptInConfirmation_Is_Called
    {
        private ApplicationController _sut;
        private Mock<IApiValidationService> _mockApiValidationService;
        private Mock<IApplicationService> _mockApplicationService;
        private Mock<IOrganisationsApiClient> _mockOrgApiClient;
        private Mock<IQnaApiClient> _mockQnaApiClient;
        private Mock<IApplicationApiClient> _mockApiClient;
        private Mock<IContactsApiClient> _mockContactsApiClient;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<IWebConfiguration> _mockConfig;
        private Mock<ILogger<ApplicationController>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockApiValidationService = new Mock<IApiValidationService>();
            _mockApplicationService = new Mock<IApplicationService>();
            _mockOrgApiClient = new Mock<IOrganisationsApiClient>();
            _mockApiClient = new Mock<IApplicationApiClient>();
            _mockContactsApiClient = new Mock<IContactsApiClient>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockQnaApiClient = new Mock<IQnaApiClient>();
            _mockConfig = new Mock<IWebConfiguration>();
            _mockLogger = new Mock<ILogger<ApplicationController>>();

            _mockApiClient
             .Setup(r => r.GetApplication(It.IsAny<Guid>()))
             .ReturnsAsync(new ApplicationResponse()
             {
                 ApplicationStatus = ApplicationStatus.InProgress,
                 ApplyData = new ApplyData()
                 {
                     Sequences = new List<ApplySequence>()
                     {
                         new ApplySequence()
                         {
                             IsActive = true,
                             SequenceNo = ApplyConst.STANDARD_SEQUENCE_NO,
                             Status = ApplicationSequenceStatus.Draft
                         }
                    },
                     Apply = new Apply()
                     {
                         StandardReference = "ST0001",
                         StandardName = "TITLE 1",
                         Versions = new List<string>() { "1.2" }
                     }
                 }
             });


            _sut = new ApplicationController(_mockApiValidationService.Object, _mockApplicationService.Object, _mockOrgApiClient.Object, _mockQnaApiClient.Object,
                        _mockConfig.Object, _mockApiClient.Object, _mockContactsApiClient.Object, _mockHttpContextAccessor.Object, _mockLogger.Object);
        }

        [Test]
        public async Task Then_Use_exisiting_Application_if_exisits()
        {
            // Arrange

            // Act
            var results = (await _sut.OptInConfirmation(Guid.NewGuid())) as ViewResult;

            // Assert
            var model = results.Model as OptInConfirmationViewModel;
            Assert.AreEqual("TITLE 1", model.StandardTitle);
            Assert.AreEqual("1.2", model.Version);
        }
    }
}
