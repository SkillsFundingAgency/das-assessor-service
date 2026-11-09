using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateCheckControllerTests
    {
        private Mock<IHttpContextAccessor> _mockContextAccessor;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IStringLocalizer<CertificateCheckViewModelValidator>> _mockLocalizer;
        private Mock<IValidator<CertificateCheckViewModel>> _validator;

        private CertificateCheckController _certificateCheckController;
            
        private Builder _builder;
        private Certificate _certificate = new Certificate();
        private const int Ukprn = 123456;
        private const string Username = "TestProviderUsername";
        private const string EpaoId = "EPAO123";

        [SetUp]
        public void Setup()
        {
            _builder = new Builder();

            _mockContextAccessor = new Mock<IHttpContextAccessor>();
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _mockSessionService = new Mock<ISessionService>();
            _mockLocalizer = new Mock<IStringLocalizer<CertificateCheckViewModelValidator>>();

            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn")).Returns(new Claim("", Ukprn.ToString()));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Returns(new Claim("", Username));
            _mockContextAccessor.Setup(s => s.HttpContext.User.FindFirst("http://schemas.portal.com/epaoid")).Returns(new Claim("", EpaoId));
            _mockContextAccessor.Setup(s => s.HttpContext.Request.Query).Returns(Mock.Of<IQueryCollection>());

            _mockLocalizer.Setup(localizer => localizer[It.IsAny<string>()]).Returns(new LocalizedString("Key", "Error"));
            
            _validator = new Mock<IValidator<CertificateCheckViewModel>>();

            var certSessionString = JsonConvert.SerializeObject(_builder.CreateNew<CertificateSession>()
                .With(x => x.Options = null).Build());

            _mockSessionService.Setup(session => session.Get(nameof(CertificateSession)))
                .Returns(certSessionString);

            _certificateCheckController = new CertificateCheckController(
                Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _validator.Object,
                _mockSessionService.Object);

            _certificateCheckController.TempData = new TempDataDictionary(_mockContextAccessor.Object.HttpContext, Mock.Of<ITempDataProvider>());

            _certificate = SetupValidCertificate();

            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(_certificate);
        }

        [Test, MoqAutoData]
        public async Task When_CertificateCheckViewModelIsInvalid(CertificateCheckViewModel vm)
        {
            _validator.Setup(s => s.Validate(It.IsAny<CertificateCheckViewModel>())).Returns(new ValidationResult(new List<ValidationFailure> {
                new ValidationFailure("Error", "Error message")
            }));

            var result = await _certificateCheckController.Check(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Check.cshtml");
        }

        [Test, MoqAutoData]
        public async Task When_CertificateCheckViewModelIsValid_Then_CallUpdateCertificate(CertificateCheckViewModel vm)
        {
            _validator.Setup(s => s.Validate(It.IsAny<CertificateCheckViewModel>())).Returns(new ValidationResult());

            await _certificateCheckController.Check(vm);

            _mockCertificateApiClient.Verify(client => client.UpdateCertificate(It.IsAny<UpdateCertificateRequest>()), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task When_CertificateCheckViewModelIsValid_Then_ReturnRedirectToCertificateConfirmation(CertificateCheckViewModel vm)
        {
            _validator.Setup(s => s.Validate(It.IsAny<CertificateCheckViewModel>())).Returns(new ValidationResult());

            var result = await _certificateCheckController.Check(vm) as RedirectToActionResult;

            result.ActionName.Should().Be("Confirm");
            result.ControllerName.Should().Be("CertificateConfirmation");
        }
        private Certificate SetupValidCertificate()
        {
            return _builder.CreateNew<Certificate>()
                .With(q => q.CertificateData = _builder.CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = CertificateGrade.Pass)
                    .With(x => x.AchievementDate = DateTime.Now)
                    .Build())
                .Build();
        }
    }
}
