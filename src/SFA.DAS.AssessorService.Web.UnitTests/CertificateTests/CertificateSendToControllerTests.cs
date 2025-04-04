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
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateSendToTests
{
    [TestFixture]
    public class WhenRequestingCertificateSendToPage
    {
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateSendToController _certificateSendToController;

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
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = CertificateId,
                    CertificateData = new CertificateData()
                });

            _certificateSendToController = new CertificateSendToController(Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockSessionService.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsSendToView(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateSendToController.SendTo() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/SendTo.cshtml");
            result.Model.Should().BeOfType<CertificateSendToViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingModelError_LoadsSendToView(CertificateSendToViewModel vm)
        {
            _certificateSendToController.ModelState.AddModelError("Key", "Message");

            var result = await _certificateSendToController.SendTo(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/SendTo.cshtml");
            result.Model.Should().BeOfType<CertificateSendToViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingSendToApprentice_RedirectsToAddress(CertificateSendToViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Apprentice;

            var result = await _certificateSendToController.SendTo(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("Address");
        }

        [Test, MoqAutoData]
        public async Task AndPostingSendToEmployer_RedirectsToPreviousAddressWhenPreviousAddressFound(CertificateSendToViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;

            var result = await _certificateSendToController.SendTo(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("PreviousAddress");
        }
    }
}
