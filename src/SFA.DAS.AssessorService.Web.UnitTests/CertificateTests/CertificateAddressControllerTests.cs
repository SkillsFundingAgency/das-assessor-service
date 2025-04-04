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
        private const string EpaoId = "EPAO123";

        private Guid CertificateIdWithoutPreviousAddress = Guid.NewGuid();
        private Guid CertificateIdWithPreviousAddress = Guid.NewGuid();

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

            var certificateWithoutPreviousAddress = new Certificate
            {
                Id = CertificateIdWithoutPreviousAddress,
                CertificateData = new CertificateData
                {
                    EmployerAccountId = 10
                }
            };

            var certificateWithPreviousAddress = new Certificate
            {
                Id = CertificateIdWithPreviousAddress,
                CertificateData = new CertificateData
                {
                    EmployerAccountId = 11
                }
            };

            _mockCertificateApiClient.Setup(s => s.GetCertificate(CertificateIdWithPreviousAddress, false)).ReturnsAsync(certificateWithPreviousAddress);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(CertificateIdWithoutPreviousAddress, false)).ReturnsAsync(certificateWithoutPreviousAddress);

            _mockCertificateApiClient.Setup(s => s.GetContactPreviousAddress(EpaoId, 10)).ReturnsAsync((CertificateAddress)null);
            _mockCertificateApiClient.Setup(s => s.GetContactPreviousAddress(EpaoId, 11)).ReturnsAsync(
                new CertificateAddress
                {
                    ContactOrganisation = "Previous Organisation Name"
                });

            _certificateAddressController = new CertificateAddressController(Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _mockSessionService.Object);
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsRedirectsToAddress_WhenCertificateEmployerWithoutPreviousAddress(CertificateSession session)
        {
            session.CertificateId = CertificateIdWithoutPreviousAddress;

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.PreviousAddress() as RedirectToActionResult;

            result.ActionName.Should().Be("Address");
            result.ControllerName.Should().Be("CertificateAddress");
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsPreviousAddressView_WhenCertificateEmployerWithPreviousAddress(CertificateSession session)
        {
            session.CertificateId = CertificateIdWithPreviousAddress;

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.PreviousAddress() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/PreviousAddress.cshtml");
            result.Model.Should().BeOfType<CertificatePreviousAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingModelErrorToPreviousAddress_LoadsPreviousAddressView(CertificatePreviousAddressViewModel vm)
        {
            vm.Id = CertificateIdWithPreviousAddress;
            _certificateAddressController.ModelState.AddModelError("Key", "Message");

            var result = await _certificateAddressController.PreviousAddress(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/PreviousAddress.cshtml");
            result.Model.Should().BeOfType<CertificatePreviousAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingUsePreviousAddress_RedirectsToRecipient(CertificatePreviousAddressViewModel vm)
        {
            vm.UsePreviousAddress = true;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.PreviousAddress(vm) as RedirectToActionResult;

            result.ActionName.Should().Be("Recipient");
            result.ControllerName.Should().Be("CertificateAddress");
        }

        [Test, MoqAutoData]
        public async Task AndPostingDoNotUsePreviousAddress_RedirectsToAddress(CertificatePreviousAddressViewModel vm, CertificateSession session)
        {
            vm.UsePreviousAddress = false;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.PreviousAddress(vm) as RedirectToActionResult;

            result.ActionName.Should().Be("Address");
            result.ControllerName.Should().Be("CertificateAddress");
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsAddressView(CertificateSession session)
        {
            session.CertificateId = CertificateIdWithPreviousAddress;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.Address() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Address.cshtml");
            result.Model.Should().BeOfType<CertificateAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingModelErrorToAddress_LoadsAddressView(CertificateAddressViewModel vm)
        {
            vm.Id = CertificateIdWithPreviousAddress;
            _certificateAddressController.ModelState.AddModelError("Key", "Message");

            var result = await _certificateAddressController.Address(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Address.cshtml");
            result.Model.Should().BeOfType<CertificateAddressViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingAddressForSendToApprentice_RedirectsToConfirmAddress(CertificateAddressViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Apprentice;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.Address(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("ConfirmAddress");
        }

        [Test, MoqAutoData]
        public async Task AndPostingAddressForSendToEmployer_RedirectsToRecipient(CertificateAddressViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.Address(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("Recipient");
        }

        [Test, MoqAutoData]
        public async Task AndPostingSameAddressForSendToEmployer_ReturnsToCheck_IfRedirectToCheckSet(CertificateSession session, CertificateAddressViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out expectedValue)).Returns(true);

            var result = await _certificateAddressController.Address(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }

        [Test, MoqAutoData]
        public async Task AndPostingDifferentAddressForSendToEmployer_RedirectsToRecipient_IfRedirectToCheckSet(CertificateSession session, CertificateAddressViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;
            vm.Employer = vm.Employer + "_Different";

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            
            var redirectToCheck = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out redirectToCheck)).Returns(true);
            
            _mockSessionService.Setup(s => s.Set("RedirectToCheck", It.IsAny<object>())).Callback((string key, object value) => {
                redirectToCheck = (bool)value;
                _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out redirectToCheck)).Returns(true);
            });

            var result = await _certificateAddressController.Address(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("Recipient");
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsRecipientView(CertificateSession session)
        {
            session.CertificateId = CertificateIdWithPreviousAddress;
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _certificateAddressController.Recipient() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Recipient.cshtml");
            result.Model.Should().BeOfType<CertificateRecipientViewModel>();
        }

        [Test, MoqAutoData]
        public async Task AndPostingRecipientSendToEmployer_RedirectsToConfirm(CertificateRecipientViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.Recipient(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("ConfirmAddress");
        }

        [Test, MoqAutoData]
        public async Task AndPostingSameRecipientForSendToEmployer_ReturnsToCheck_IfRedirectToCheckSet(CertificateSession session, CertificateRecipientViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);
            var expectedValue = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out expectedValue)).Returns(true);

            var result = await _certificateAddressController.Recipient(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }

        [Test, MoqAutoData]
        public async Task AndPostingDifferentRecipientForSendToEmployer_RedirectsToConfirm_IfRedirectToCheckSet(CertificateSession session, CertificateRecipientViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;
            vm.Name = vm.Name + "_Different";

            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var redirectToCheck = true;
            _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out redirectToCheck)).Returns(true);

            _mockSessionService.Setup(s => s.Set("RedirectToCheck", It.IsAny<object>())).Callback((string key, object value) => {
                redirectToCheck = (bool)value;
                _mockSessionService.Setup(s => s.TryGet<bool>("RedirectToCheck", out redirectToCheck)).Returns(true);
            });

            var result = await _certificateAddressController.Recipient(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateAddress");
            result.ActionName.Should().Be("ConfirmAddress");
        }

        [Test, MoqAutoData]
        public async Task Then_LoadsConfirmAddressView(CertificateSession session)
        {
            session.CertificateId = CertificateIdWithPreviousAddress;
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
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.ConfirmAddress(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }

        [Test, MoqAutoData]
        public async Task AndPostingConfirmAddressForSendToEmployer_RedirectsToCheck(CertificateRecipientViewModel vm)
        {
            vm.SendTo = CertificateSendTo.Employer;
            vm.Id = CertificateIdWithPreviousAddress;

            var result = await _certificateAddressController.ConfirmAddress(vm) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }
    }
}
