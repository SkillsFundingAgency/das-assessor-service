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

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateAddressTests
{
    [TestFixture]
    public class WhenRequestingCertificateAddressPage
    {
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private CertificateAddressController _certificateAddressController;

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
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = CertificateId,
                    CertificateData = certDataString
                });

            _certificateAddressController = new CertificateAddressController(Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockSessionService.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsAddressView(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.Address() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Address.cshtml");
            result.Model.Should().BeOfType<CertificateAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingModelError_LoadsAddressView(CertificateAddressViewModel vm)
        {
            _certificateAddressController.ModelState.AddModelError("Key", "Message");

            var result = await _certificateAddressController.Address(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Address.cshtml");
            result.Model.Should().BeOfType<CertificateAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingAddressForSendToApprentice_RedirectsToConfirmAddress(CertificateAddressViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Apprentice;

            var result = await _certificateAddressController.Address(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("ConfirmAddress");
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsConfirmAddressView(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.ConfirmAddress() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/ConfirmAddress.cshtml");
            result.Model.Should().BeOfType<CertificateRecipientViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingConfirmAddressForSendToApprentice_RedirectsToCheck(CertificateRecipientViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Apprentice;

            var result = await _certificateAddressController.ConfirmAddress(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }
    }
}
