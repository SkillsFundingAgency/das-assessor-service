﻿using FluentAssertions;
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
using SFA.DAS.AssessorService.Web.ViewModels.Shared;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateVersionControllerTests
    {
        private Mock<IStandardVersionApiClient> _mockStandardVersionClient;
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
            _mockStandardVersionClient = new Mock<IStandardVersionApiClient>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSessionService = new Mock<ISessionService>();

            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", Ukprn.ToString()));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", Username));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")).Returns(new Claim("", EpaoId));
            _mockContextAccessor.Setup(s => s.HttpContext.Request.Query).Returns(Mock.Of<IQueryCollection>());
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(
                new Domain.Entities.Certificate
                {
                    Id = CertificateId,
                    StandardUId = StandardUId,
                    CertificateData = new CertificateData()
                });

            _certificateVersionController = new CertificateVersionController(
                Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockStandardVersionClient.Object,
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
            session.Versions = new List<StandardVersionViewModel>();
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
        public async Task WhenSelectingAVersion_WhenLoadingModel_WithOneVersion_WithNoOptions_RedirectsToDeclaration(CertificateSession session, StandardVersionViewModel standardVersion)
        {
            standardVersion.StandardUId = session.StandardUId;
            session.Versions = new List<StandardVersionViewModel> { standardVersion };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(session.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenSelectingAVersion_WhenLoadingModel_WithOneVersion_WithOptions_RedirectsToOptionPage(CertificateSession session, StandardVersionViewModel standardVersion, StandardOptions options)
        {
            standardVersion.StandardUId = session.StandardUId;
            session.Versions = new List<StandardVersionViewModel> { standardVersion };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(session.StandardUId)).ReturnsAsync(options);

            var result = await _certificateVersionController.Version(false) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");

            _mockSessionService.Verify(s => s.Set(nameof(CertificateSession), It.Is<CertificateSession>(c => c.Options.SequenceEqual(options.CourseOption.ToList()) && c.StandardUId == standardVersion.StandardUId)));
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_AndEpaoIsNotApprovedForSelectedVersion_ThenRedirectToNotApprovedToAssessPage(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet("RedirectToCheck", out expectedValue)).Returns(true);
            var approvedVersions = new List<StandardVersion>();

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ActionName.Should().Be("NotApprovedToAssess");
            result.ControllerName.Should().Be("CertificateVersionNotApproved");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_UpdatesCertificateWithStandardUId(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(v => v.UpdateCertificate(It.Is<UpdateCertificateRequest>(c =>
                c.Certificate.StandardUId == vm.StandardUId)));

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task When_SelectingVersion_And_UpdateCertificateFails_Then_RedirectToError(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            _mockCertificateApiClient.Setup(c => c.UpdateCertificate(It.IsAny<UpdateCertificateRequest>())).ThrowsAsync(new Exception());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("Home");
            result.ActionName.Should().Be("Error");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasOptions_RedirectToOptionsPage(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Once);
            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasOptions_AndOnlyOneOption_RedirectToDeclarePage(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, string option, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            options.CourseOption = new List<string> { option };
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Never);
            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModelWithRedirectToCheck_IfVersionHasOptions_ButVersionWasNotChanged_RedirectToCheckPage(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            vm.StandardUId = StandardUId;
            standardVersion.StandardUId = StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out expectedValue)).Returns(true);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
            result.RouteValues.Should().BeNull();
            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Never);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionHasOptions_RedirectToOptionsPageWithRedirectToCheck(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);

            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out expectedValue)).Returns(true);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
            result.RouteValues.Should().ContainKey("RedirectToCheck");
            result.RouteValues.Should().ContainValue(true);
            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_IfVersionNotChanged_AndRedirectToCheckSet_AndVersionHasOptions_ButOptionsNotSet_RedirectToOptionsPageWithRedirectToCheck(CertificateVersionViewModel vm, StandardVersion standardVersion, StandardOptions options, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            vm.StandardUId = StandardUId;
            standardVersion.StandardUId = StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(options);

            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out expectedValue)).Returns(true);

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
            result.RouteValues.Should().ContainKey("RedirectToCheck");
            result.RouteValues.Should().ContainValue(true);
            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task WhenPostingToSelectAVersion_WhenSavingModel_ClearOptionSessionCache(CertificateVersionViewModel vm, StandardVersion standardVersion, CertificateSession session, List<StandardVersion> approvedVersions)
        {
            standardVersion.StandardUId = vm.StandardUId;
            approvedVersions.Add(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());


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
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionById(vm.StandardUId)).ReturnsAsync(standardVersion);
            _mockStandardVersionClient.Setup(s => s.GetEpaoRegisteredStandardVersions(EpaoId, session.StandardCode)).ReturnsAsync(approvedVersions);
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(vm.StandardUId)).ReturnsAsync(new StandardOptions());

            var result = await _certificateVersionController.Version(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
            _mockSessionService.Verify(s => s.Set("RedirectedFromVersion", true), Times.Never);
        }

        public async Task When_RequestingSelectVersion_And_AttemptedStandardVersion_Then_SetStandardUIdToAttemptedStandardVersion()
        {
            _mockSessionService.Setup(s => s.Get("AttemptedStandardVersion")).Returns("ST0001_1.2");

            var result = await _certificateVersionController.Version() as ViewResult;

            var model = result.Model as CertificateVersionViewModel;

            model.StandardUId.Should().Be("ST0001_1.2");
        }

        public async Task When_RequestingSelectVersion_And_AttemptedStandardVersion_Then_RemoveFromSession()
        {
            _mockSessionService.Setup(s => s.Get("AttemptedStandardVersion")).Returns("ST0001_1.2");

            await _certificateVersionController.Version();

            _mockSessionService.Verify(s => s.Remove("AttemptedStandardVersion"), Times.Once);
        }
    }
}
