using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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
    public class CertificateControllerTests
    {
        private Mock<IStandardVersionClient> _mockStandardVersionClient;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateController _certificateController;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private Guid CertificateId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StandardVersionViewModel, StandardVersion>();
            });

            _mockStandardVersionClient = new Mock<IStandardVersionClient>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
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
                _mockCertificateApiClient.Object,
                _mockStandardVersionClient.Object,
                _mockSessionService.Object);

        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithSingleVersionNoOptions_GoesToDeclaration(CertificateStartViewModel model, StandardVersion standard)
        {
            CertificateSession setSession = new CertificateSession();
            model.StandardUId = string.Empty;
            model.Option = string.Empty;
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(new StandardOptions());
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
        public async Task WhenStartingANewCertificate_WithMultipleVersions_GoesToVersionPage(CertificateStartViewModel model, IEnumerable<StandardVersion> standards)
        {
            CertificateSession setSession = new CertificateSession();
            model.StandardUId = string.Empty;
            model.Option = string.Empty;
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(standards);
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
        public async Task WhenStartingANewCertificate_WithOneVersion_WithOptions_GoesToOptionPage(CertificateStartViewModel model, StandardVersion standard, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            model.StandardUId = string.Empty;
            model.Option = string.Empty;
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
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
        public async Task WhenStartingANewCertificate_WithOneVersion_WithOneOption_GoesToDeclarationPage(CertificateStartViewModel model, StandardVersion standard, StandardOptions options, string option)
        {
            CertificateSession setSession = new CertificateSession();
            model.StandardUId = string.Empty;
            model.Option = string.Empty;
            options.CourseOption = new List<string> { option };
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
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

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionAndOptionSetFromApprovals_GoesToDeclarationPage(CertificateStartViewModel model)
        {
            CertificateSession setSession = new CertificateSession();
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
                && m.Username == Username && m.UkPrn == Ukprn 
                && m.StandardUId == model.StandardUId && m.CourseOption == model.Option)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEmpty();
            setSession.Versions.Should().BeEmpty();

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithVersionSetFromApprovals_OptionRequiredButNotSet_GoesToOptionsPage(CertificateStartViewModel model, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            model.Option = string.Empty;
            _mockStandardVersionClient.Setup(s => s.GetStandardOptions(model.StandardUId)).ReturnsAsync(options);
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
                && m.Username == Username && m.UkPrn == Ukprn
                && m.StandardUId == model.StandardUId && m.CourseOption == string.Empty)));

            setSession.CertificateId.Should().Be(CertificateId);
            setSession.Uln.Should().Be(model.Uln);
            setSession.StandardCode.Should().Be(model.StdCode);
            setSession.Options.Should().BeEquivalentTo(options.CourseOption.ToList());
            setSession.Versions.Should().BeEmpty();

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be("Option");
        }
    }
}
