using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SharedModelTypes = SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateVersionControllerTests
    {
        private Mock<IStandardServiceClient> _mockStandardServiceClient;
        private Mock<IStandardVersionClient> _mockStandardVersionClient;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateVersionController _certificateVersionController;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private Guid CertificateId = Guid.NewGuid();
        private string StandardUId = "ST00123";
        private const string EpaoId = "EPAO123";

        [SetUp]
        public void SetUp()
        {
            _mockStandardServiceClient = new Mock<IStandardServiceClient>();
            _mockStandardVersionClient = new Mock<IStandardVersionClient>();
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
                new Domain.Entities.Certificate
                {
                    Id = CertificateId,
                    StandardUId = StandardUId,
                    CertificateData = certDataString
                });

            _certificateVersionController = new CertificateVersionController(
                Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockStandardVersionClient.Object,
                _mockStandardServiceClient.Object,
                _mockSessionService.Object);

        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenNoSession_RedirectsBackToSearch()
        {
            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenSessionVersionValueIsNull_RedirectsBackToSearch(CertificateSession session)
        {
            session.Versions = null;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenSessionVersionValueIsEmpty_RedirectsBackToSearch(CertificateSession session)
        {
            session.Versions = new List<SharedModelTypes.StandardVersion>();
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenLoadingModel_StoresStandardVersions(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateVersionController.Version(false) as ViewResult;

            var versionModel = ((CertificateVersionViewModel)result.Model);

            versionModel.Should().NotBeNull();
            versionModel.Id.Should().Be(CertificateId);
            versionModel.Versions.Should().BeEquivalentTo(session.Versions);
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenLoadingModel_WithOneVersion_WithNoOptions_RedirectsToDeclaration(CertificateSession session, SharedModelTypes.StandardVersion standardVersion)
        {
            standardVersion.StandardUId = session.StandardUId;
            session.Versions = new List<SharedModelTypes.StandardVersion> { standardVersion };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(session.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenLoadingModel_WithOneVersion_WithOptions_RedirectsToOptionPage(CertificateSession session, SharedModelTypes.StandardVersion standardVersion, StandardOptions options)
        {
            standardVersion.StandardUId = session.StandardUId;
            session.Versions = new List<SharedModelTypes.StandardVersion> { standardVersion };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(session.StandardUId)).ReturnsAsync(options);

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");

            _mockSessionService.Verify(s => s.Set(nameof(CertificateSession), It.Is<CertificateSession>(c => c.Options.SequenceEqual(options.CourseOption.ToList()) && c.StandardUId == standardVersion.StandardUId)));
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_AndEpaoIsNotApprovedToRecordAGrade_ErrorsAndReturnsToSelectVersions(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            var approvedVersions = new List<StandardVersion>();

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as ViewResult;
            result.ViewName.Should().Be("~/Views/Certificate/Version.cshtml");

            var model = result.Model as CertificateVersionViewModel;
            model.StandardUId.Should().Be(vm.StandardUId);

            _certificateVersionController.ModelState.ErrorCount.Should().Be(1);
            _certificateVersionController.ModelState.IsValid.Should().Be(false);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_UpdatesCertificateWithStandardUId(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(v => v.UpdateCertificate(It.Is<UpdateCertificateRequest>(c =>
                c.Certificate.StandardUId == vm.StandardUId)));

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasOptions_RedirectToOptionsPage(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockSessionService.Verify(s => s.Set("redirectedfromversion", true), Times.Once);
            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModelWithRedirectToCheck_IfVersionHasOptions_ButVersionWasNotChanged_RedirectToCheckPage(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            vm.StandardUId = StandardUId;
            standardVersion.StandardUId = StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);
            _mockSessionService.Setup(s => s.Exists("redirecttocheck")).Returns(true);
            _mockSessionService.Setup(s => s.Get("redirecttocheck")).Returns("true");

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
            result.RouteValues.Should().BeNull();
            _mockSessionService.Verify(s => s.Set("redirectedfromversion", true), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasOptions_RedirectToOptionsPageWithRedirectToCheck(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);
            _mockSessionService.Setup(s => s.Exists("redirecttocheck")).Returns(true);
            _mockSessionService.Setup(s => s.Get("redirecttocheck")).Returns("true");

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
            result.RouteValues.Should().ContainKey("redirecttocheck");
            result.RouteValues.Should().ContainValue(true);
            _mockSessionService.Verify(s => s.Set("redirectedfromversion", true), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_ClearOptionSessionCache(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());


            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockSessionService.Verify(s => s.Set(nameof(CertificateSession), It.Is<CertificateSession>(v => v.Options == null && v.StandardUId == vm.StandardUId)));
        }

        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasNoOptions_RedirectToDeclarationPage(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionByStandardUId(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
            _mockSessionService.Verify(s => s.Set("redirectedfromversion", true), Times.Never);
        }
    }
}
