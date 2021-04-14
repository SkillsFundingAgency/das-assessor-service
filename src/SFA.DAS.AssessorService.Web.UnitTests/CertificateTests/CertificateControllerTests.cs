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
        private Mock<IStandardServiceClient> _mockStandardServiceClient;
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
            _mockStandardServiceClient = new Mock<IStandardServiceClient>();
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
                _mockStandardServiceClient.Object,
                _mockSessionService.Object);

        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithSingleVersionNoOptions_GoesToDeclaration(CertificateStartViewModel model, StandardVersion standard)
        {
            CertificateSession setSession = new CertificateSession();
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(new StandardOptions());
            _mockSessionService.Setup(c => c.Set("CertificateSession", It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == "CertificateSession" && session is CertificateSession)
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
            setSession.Options.Should().BeNull();
            setSession.Versions.Should().BeEquivalentTo(new List<ViewModels.Shared.StandardVersion> { standard });

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithMultipleVersions_GoesToVersionPage(CertificateStartViewModel model, IEnumerable<StandardVersion> standards)
        {
            CertificateSession setSession = new CertificateSession();
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(standards);
            _mockSessionService.Setup(c => c.Set("CertificateSession", It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                 {
                     if (key == "CertificateSession" && session is CertificateSession)
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
            setSession.Options.Should().BeNull();
            setSession.Versions.Should().BeEquivalentTo(standards.Select(a => (ViewModels.Shared.StandardVersion)a));
        }

        [Test, MoqAutoData]
        public async Task WhenStartingANewCertificate_WithOneVersion_GoesToOptionPage(CertificateStartViewModel model, StandardVersion standard, StandardOptions options)
        {
            CertificateSession setSession = new CertificateSession();
            _mockStandardVersionClient.Setup(s => s.GetStandardVersionsByLarsCode(model.StdCode)).ReturnsAsync(new List<StandardVersion> { standard });
            _mockStandardServiceClient.Setup(s => s.GetStandardOptions(standard.StandardUId)).ReturnsAsync(options);
            _mockSessionService.Setup(c => c.Set("CertificateSession", It.IsAny<object>()))
                .Callback<string, object>((key, session) =>
                {
                    if (key == "CertificateSession" && session is CertificateSession)
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
            setSession.Versions.Should().BeEquivalentTo(new List<ViewModels.Shared.StandardVersion> { standard });

            result.ControllerName.Should().Be("CertificateOption");
            result.ActionName.Should().Be(CertificateActions.Option);
        }
    }
}
