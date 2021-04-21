using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
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
using System.Linq;
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
        private CertificateCheckViewModelValidator _validator;

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
            
            _validator = new CertificateCheckViewModelValidator(_mockLocalizer.Object);

            var certSessionString = JsonConvert.SerializeObject(_builder.CreateNew<CertificateSession>()
                .With(x => x.Options = null).Build());

            _mockSessionService.Setup(session => session.Get(nameof(CertificateSession)))
                .Returns(certSessionString);

            _certificateCheckController = new CertificateCheckController(
                Mock.Of<ILogger<CertificateController>>(),
                _mockContextAccessor.Object,
                _mockCertificateApiClient.Object,
                _validator,
                _mockSessionService.Object);

            _certificateCheckController.TempData = new TempDataDictionary(_mockContextAccessor.Object.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [Test, MoqAutoData]
        public async Task WhenSubmittingCertificate_AndOptionIsNotSet_ThenShouldRedirectToCheckPage(CertificateCheckViewModel vm)
        {
            _certificate = _builder.CreateNew<Certificate>()
              .With(q => q.CertificateData = JsonConvert.SerializeObject(_builder.CreateNew<CertificateData>()
                  .With(x => x.OverallGrade = CertificateGrade.Pass)
                  .With(x => x.AchievementDate = DateTime.Now)
                  .With(x => x.CourseOption = null)
                  .Build()))
              .Build();

            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>())).ReturnsAsync(_certificate);

            vm.StandardHasOptions = true;

            var result = await _certificateCheckController.Check(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Check.cshtml");
        }

        [Test, MoqAutoData]
        public async Task WhenSubmittingPass_AndNoRecipientIsEntered_ThenRedirectToCheckPage(CertificateCheckViewModel vm)
        {
            _certificate = _builder.CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(_builder.CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = CertificateGrade.Pass)
                    .With(x => x.AchievementDate = DateTime.Now)
                    .With(x => x.ContactName = null)
                    .With(x => x.ContactAddLine1 = null)
                    .With(x => x.ContactAddLine2 = null)
                    .With(x => x.ContactAddLine3 = null)
                    .With(x => x.ContactAddLine4 = null)
                    .With(x => x.ContactPostCode = null)
                    .Build()))
                .Build();

            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>())).ReturnsAsync(_certificate);
            
            var result = await _certificateCheckController.Check(vm) as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Check.cshtml");
        }
    }
}
