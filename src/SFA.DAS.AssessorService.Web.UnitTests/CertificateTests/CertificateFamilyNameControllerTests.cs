using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using SFA.DAS.AssessorService.Domain.Entities;
using FluentAssertions;
using SFA.DAS.AssessorService.Domain.JsonData;
using FluentValidation;
using System.Collections.Generic;
using FluentValidation.Results;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    [TestFixture]
    public class CertificateFamilyNameControllerTests
    {
        private CertificateFamilyNameController _controller;
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<IValidator<CertificateNamesViewModel>> _validator;

        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private readonly Guid CertificateId = Guid.NewGuid();
        private const string EpaoId = "EPAO123";


        [SetUp]
        public void Setup()
        {
            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockSessionService = new Mock<ISessionService>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _validator = new Mock<IValidator<CertificateNamesViewModel>>();

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

            _controller = new CertificateFamilyNameController(Mock.Of<ILogger<CertificateController>>(), _mockContextAccessor.Object, _mockCertificateApiClient.Object, _mockSessionService.Object, _validator.Object);
        }

        [Test, MoqAutoData]
        public async Task WhenRequestingFamilyNameView_ThenLoadsFamilyNameViewSuccessfully(CertificateSession session)
        {
            var sessionString = JsonConvert.SerializeObject(session);
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns(sessionString);

            var result = await _controller.FamilyName(true) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/FamilyName.cshtml");
            result.Model.Should().BeOfType<CertificateNamesViewModel>();
        }

        [Test]
        public async Task WhenRequestingFamilyNameView_AndThereIsNoSessionFound_ThenRedirectsToSearch()
        {
            var result = await _controller.FamilyName() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingFamilyNameView_AndInputIsValid_ThenRedirectsToCheckPage(CertificateNamesViewModel mockViewModel)
        {
            _validator.Setup(s => s.Validate(It.IsAny<CertificateNamesViewModel>())).Returns(new ValidationResult());

            var result = await _controller.FamilyName(mockViewModel) as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateCheck");
            result.ActionName.Should().Be("Check");
        }

        [Test, MoqAutoData]
        public async Task WhenPostingFamilyNameView_AndInputIsNotValid_ThenRefreshesWithErrors(CertificateNamesViewModel mockViewModel)
        {
            _validator.Setup(s => s.Validate(It.IsAny<CertificateNamesViewModel>())).Returns(new ValidationResult(new List<ValidationFailure> {
                new ValidationFailure("Error", "Error message")
            }));

            var result = await _controller.FamilyName(mockViewModel) as ViewResult;

            result.ViewName.Should().Be("/Views/Certificate/FamilyName.cshtml");
            result.ViewData.ModelState.ErrorCount.Should().BeGreaterThanOrEqualTo(1);
        }

    }
}
