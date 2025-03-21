﻿using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Web.UnitTests;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateControllerTests : MapperBase
    {
        private Mock<IStandardVersionApiClient> _mockStandardVersionClient;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IApprovalsLearnerApiClient> _mockLearnerApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateController _certificateController;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private Guid CertificateId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {       
            _mockStandardVersionClient = new Mock<IStandardVersionApiClient>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _mockLearnerApiClient = new Mock<IApprovalsLearnerApiClient>();
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSessionService = new Mock<ISessionService>();

            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", Ukprn.ToString()));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", Username));

            _mockCertificateApiClient.Setup(s => s.Start(It.IsAny<StartCertificateRequest>())).ReturnsAsync(
                new Domain.Entities.Certificate
                {
                    Id = CertificateId
                });

            _certificateController = new CertificateController(
                Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                Mapper,
                _mockCertificateApiClient.Object,
                _mockStandardVersionClient.Object,
                _mockSessionService.Object,
                _mockLearnerApiClient.Object);

        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithoutApprovalsData_WithSingleVersionNoOptions_GoesToDeclaration(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion standard)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = null;
            learner.CourseOption = null;

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(new StandardOptions());
            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(model.StdCode, model.Uln)).ReturnsAsync(learner);


            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode && m.Uln == model.Uln
                && m.Username == Username && m.UkPrn == Ukprn)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEmpty();
            setSession.Versions.Should().BeEquivalentTo(new List<StandardVersionViewModel> { Mapper.Map<StandardVersionViewModel>(standard) });

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithoutApprovalsData_WithMultipleVersions_GoesToVersionPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, IEnumerable<StandardVersion> standards)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = null;
            learner.CourseOption = null;

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(standards);
            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(model.StdCode, model.Uln)).ReturnsAsync(learner);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                 {
                     if (key == nameof(CertificateSession) && session is CertificateSession)
                     {
                         setSession = (CertificateSession)session;
                     }
                 });
            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode && m.Uln == model.Uln
                && m.Username == Username && m.UkPrn == Ukprn)));

            result.ControllerName.Should().Be("CertificateVersion");
            result.ActionName.Should().Be("Version");

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEmpty();
            setSession.Versions.Should().BeEquivalentTo(Mapper.Map<List<StandardVersionViewModel>>(standards));
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithoutApprovalsData_WithOneVersion_WithOptions_GoesToOptionPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion standard, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = null;
            learner.CourseOption = null;

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(model.StdCode, model.Uln)).ReturnsAsync(learner);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });
            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode && m.Uln == model.Uln
                && m.Username == Username && m.UkPrn == Ukprn)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(options.CourseOption.ToList());
            setSession.Versions.Should().BeEquivalentTo(new List<StandardVersionViewModel> { Mapper.Map<StandardVersionViewModel>(standard) });

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be(CertificateActions.Option);
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithoutApprovalsData_WithOneVersion_WithOneOption_GoesToDeclarationPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion standard, StandardOptions options, string option)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = null;
            learner.CourseOption = null;

            options.CourseOption = new List<string> { option };

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(model.StdCode, model.Uln)).ReturnsAsync(learner);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode
                    && m.Uln == model.Uln
                    && m.Username == Username
                    && m.UkPrn == Ukprn
                    && m.StandardUId == standard.StandardUId)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(options.CourseOption.ToList());
            setSession.Versions.Should().BeEquivalentTo(new List<StandardVersionViewModel> { Mapper.Map<StandardVersionViewModel>(standard) });

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionSetFromApprovalsButNotConfirmed_GoesToVersionPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, IEnumerable<StandardVersion> standards)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = standards.First().Version;
            learner.CourseOption = null;
            learner.VersionConfirmed = false;

            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(It.IsAny<int>(), It.IsAny<long>()))
                .ReturnsAsync(learner);

            _mockStandardVersionClient.Setup(c => c.GetStandardVersionsByLarsCode(It.IsAny<int>())).ReturnsAsync(standards);
                        
            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode
                    && m.Uln == model.Uln
                    && m.Username == Username
                    && m.UkPrn == Ukprn)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEmpty();
            setSession.Versions.Should().BeEquivalentTo(Mapper.Map<List<StandardVersionViewModel>>(standards));

            result.ControllerName.Should().Be("CertificateVersion");
            result.ActionName.Should().Be("Version");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionAndOptionSetFromApprovalsButNotConfirmed_WithOneVersion_WithOptions_GoesToOptionPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion standard, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = null;
            learner.CourseOption = null;
            learner.VersionConfirmed = false;

            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(model.StdCode, model.Uln)).ReturnsAsync(learner);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode && m.Uln == model.Uln
                && m.Username == Username && m.UkPrn == Ukprn)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(options.CourseOption.ToList());
            setSession.Versions.Should().BeEquivalentTo(new List<StandardVersionViewModel> { Mapper.Map<StandardVersionViewModel>(standard) });

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be(CertificateActions.Option);
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionAndOptionSetFromApprovals_GoesToDeclarationPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion standardVersion, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            learner.Version = standardVersion.Version;
            learner.VersionConfirmed = true;

            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(It.IsAny<int>(), It.IsAny<long>()))
                .ReturnsAsync(learner);

            _mockStandardVersionClient.Setup(c => c.GetStandardVersionsByLarsCode(It.IsAny<int>()))
                .ReturnsAsync(new List<StandardVersion> { standardVersion });

            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standardVersion.StandardUId))
                .ReturnsAsync(options);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode
                    && m.Uln == model.Uln
                    && m.Username == Username
                    && m.UkPrn == Ukprn
                    && m.StandardUId == standardVersion.StandardUId
                    && m.CourseOption == learner.CourseOption)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(new List<string> { learner.CourseOption });
            setSession.Versions.Should().BeEquivalentTo(new List<StandardVersionViewModel> { Mapper.Map<StandardVersionViewModel>(standardVersion) });

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionSetFromApprovals_OptionRequiredButNotSet_GoesToOptionsPage(CertificateStartViewModel model, ApprovalsLearnerResult learner, StandardVersion version, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            version.LarsCode = model.StdCode;
            learner.Version = version.Version;
            learner.VersionConfirmed = true;
            learner.CourseOption = string.Empty;

            _mockLearnerApiClient.Setup(c => c.GetLearnerRecord(It.IsAny<int>(), It.IsAny<long>()))
                .ReturnsAsync(learner);

            _mockStandardVersionClient.Setup(c => c.GetStandardVersionsByLarsCode(It.IsAny<int>()))
                .ReturnsAsync(new List<StandardVersion> { version });

            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(version.StandardUId))
                .ReturnsAsync(options);

            _mockSessionService.Setup(c => c.Set(nameof(CertificateSession), It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == nameof(CertificateSession) && session is CertificateSession)
                    {
                        setSession = (CertificateSession)session;
                    }
                });

            var result = await _certificateController.Start(model) as RedirectToActionResult;

            _mockCertificateApiClient.Verify(s => s.Start(It.Is<StartCertificateRequest>(
                m => m.StandardCode == model.StdCode
                    && m.Uln == model.Uln
                    && m.Username == Username
                    && m.UkPrn == Ukprn
                    && m.StandardUId == version.StandardUId
                    && m.CourseOption == null)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(options.CourseOption.ToList());
            setSession.Versions.Count().Should().Be(1);
            setSession.Versions.Single().StandardUId.Should().Be(version.StandardUId);

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
        }
    }
}
