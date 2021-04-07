using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateOptionsTests
{
    [TestFixture]
    public class WhenRequestingCertificateOptionsPage
    {
        private Certificate _certificate;
        private CertificateSession _certificateSession;

        private Mock<ISessionService> _mockSessionService;
        private Mock<IStandardServiceClient> _mockStandardServiceClient;

        private CertificateOptionController _controller;

        [SetUp]
        public void Arrange()
        {
            _certificate = SetupCertificate();

            _mockSessionService = new Mock<ISessionService>();
            _mockStandardServiceClient = new Mock<IStandardServiceClient>();

            _controller = new CertificateOptionController(Mock.Of<ILogger<CertificateController>>(),
                SetupHttpContextAssessor(),
                SetUpCertificateApiClient(),
                _mockStandardServiceClient.Object,
                _mockSessionService.Object);
        }

       [Test]
        public async Task And_StandardHasMultipleOptions_Then_ReturnOptionsView()
        {
            SetupSessionOptions(withOptions: true);

            var result = await _controller.Option() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Option.cshtml");
        }

        [Test]
        public async Task And_StandardHasNoOptions_Then_RedirectToCertificateDeclaration()
        {
            SetupSessionOptions();
            SetupStandardServiceOptions();

            var result = await _controller.Option() as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test]
        public async Task And_CertificateSessionContainsNoOptions_Then_GetStandardOptionsFromApi()
        {
            SetupSessionOptions();
            SetupStandardServiceOptions(withOptions: true);

            await _controller.Option();

            _mockStandardServiceClient.Verify(client => client.GetStandardOptions(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task And_StandardVersionHasOptions_Then_UpdateCertificateSession()
        {
            SetupSessionOptions();
            SetupStandardServiceOptions(withOptions: true);

            await _controller.Option();

            _mockSessionService.Verify(session => session.Set("CertificateSession", It.IsAny<CertificateSession>()), Times.Once);
        }

        private IHttpContextAccessor SetupHttpContextAssessor()
        {
            return MockedHttpContextAccessor.Setup().Object;
        }
        private ICertificateApiClient SetUpCertificateApiClient()
        {
            return MockedCertificateApiClient.Setup(_certificate, new Mock<ILogger<CertificateApiClient>>());
        }

        private void SetupSessionOptions(bool withOptions = false)
        {
            _certificateSession = new Builder().CreateNew<CertificateSession>()
                .With(x => x.CertificateId = _certificate.Id)
                .With(x => x.Options = new List<string>())
                .Build();

            if (withOptions)
                _certificateSession.Options = new List<string> { "Option1", "Option2", "Option3" };

            var certificateSessionString = JsonConvert.SerializeObject(_certificateSession);

            _mockSessionService.Setup(session => session.Get("CertificateSession"))
                .Returns(certificateSessionString);
        }

        private void SetupStandardServiceOptions(bool withOptions = false)
        {
            var standardOptions = new Builder().CreateNew<StandardOptions>()
                .Build();

            if (withOptions)
                standardOptions.CourseOption = new List<string> { "Option1", "Option2", "Option3" };

            _mockStandardServiceClient.Setup(client => client.GetStandardOptions(_certificateSession.StandardUId))
                .ReturnsAsync(standardOptions);
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = null)
                    .With(x => x.AchievementDate = null)
                    .Build()))
                .Build();

            certificate.OrganisationId = Guid.NewGuid();

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }
    }
}
