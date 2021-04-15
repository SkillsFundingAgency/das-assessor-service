using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateOptionsTests
{
    [TestFixture]
    public class WhenRequestingCertificateDeclarationPage
    {
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateDeclarationController _certificateDeclarationController;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private Guid CertificateId = Guid.NewGuid();
        private const string EpaoId = "EPAO123";

        [SetUp]
        public void SetUp()
        {
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSessionService = new Mock<ISessionService>();

            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", Ukprn.ToString()));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", Username));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")).Returns(new Claim("", EpaoId));
            _mockContextAccessor.Setup(s => s.HttpContext.Request.Query).Returns(Mock.Of<IQueryCollection>());
            var certData = new CertificateData();
            var certDataString = JsonConvert.SerializeObject(certData);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>())).ReturnsAsync(
                new Certificate
                {
                    Id = CertificateId,
                    CertificateData = certDataString
                });

            _certificateDeclarationController = new CertificateDeclarationController(Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockSessionService.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsDeclarationView(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateDeclarationController.Declare() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Declaration.cshtml");
            result.Model.Should().BeOfType<CertificateDeclarationViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingDeclaration_RedirectsToGrade(CertificateDeclarationViewModel vm)
        {
            var result = await _certificateDeclarationController.Declare(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateGrade");
            result.ActionName.Should().Be("Grade");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToSearchIndex_IfNoSession()
        {
            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToSearchResult_IfHasOnlyOneOption(CertificateSession session)
        {
            session.Options = new List<string> { "1 Option" };
            session.Versions = null;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Result");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToCertificateOptionPage_IfHasOptions(CertificateSession session)
        {
            session.Options = new List<string> { "Many Options", "Many Options" };
            session.Versions = null;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToSearchResult_IfNoOptionsWith1Version(CertificateSession session, ViewModels.Shared.StandardVersion standardVersion)
        {
            session.Options = null;
            session.Versions = new List<ViewModels.Shared.StandardVersion> { standardVersion };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Result");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToVersionSelectPage_IfNoOptionsWithManyVersions(CertificateSession session, List<ViewModels.Shared.StandardVersion> standardVersions)
        {
            session.Options = null;
            session.Versions = standardVersions;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateVersion");
            result.ActionName.Should().Be("Version");
        }

        [Test, MoqAutoData]
        public void AndClickingBack_RedirectsToSearchIndex_IfNoOptionsOrVersions()
        {
            var result = _certificateDeclarationController.Back() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }
    }
}
