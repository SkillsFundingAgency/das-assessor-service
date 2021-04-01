using FizzWare.NBuilder;
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
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateOptionsTests
{
    [TestFixture]
    public class WhenRequestingCertificateOptionsPage
    {
        private Certificate _certificate;

        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ISessionService> _mockSessionService;

        private CertificateOptionController _controller;

        [SetUp]
        public void Arrange()
        {
            _certificate = SetupCertificate();

            _mockSessionService = new Mock<ISessionService>();
              _mockHttpContextAccessor = SetupHttpContextAssessor();

            _controller = new CertificateOptionController(Mock.Of<ILogger<CertificateController>>(),
                _mockHttpContextAccessor.Object,
                SetUpCertificateApiClient(),
                _mockSessionService.Object);
        }

        [Test]
        public async Task And_StandardHasMultipleOptions_Then_ReturnOptionsView()
        {
            SetupSessionOptions(new List<string> { "Option1", "Option2", "Option3" });

            var result = await _controller.Option() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/Option.cshtml");
        }

        [Test]
        public async Task And_StandardHasNoOptions_Then_RedirectToCertificateDeclaration()
        {
            SetupSessionOptions(new List<string>());

            var result = await _controller.Option() as RedirectToActionResult;

            result.ControllerName.Should().Be("CertificateDeclaration");
            result.ActionName.Should().Be("Declare");
        }

        private Mock<IHttpContextAccessor> SetupHttpContextAssessor()
        {
            return MockedHttpContextAccessor.Setup();
        }
        private ICertificateApiClient SetUpCertificateApiClient()
        {
            return MockedCertificateApiClient.Setup(_certificate, new Mock<ILogger<CertificateApiClient>>());
        }

        private void SetupSessionOptions(List<string> options)
        {
            var certificateSession = new Builder().CreateNew<CertificateSession>()
                .With(x => x.CertificateId = _certificate.Id)
                .With(x => x.Options = options)
                .Build();

            var certificateSessionString = JsonConvert.SerializeObject(certificateSession);

            _mockSessionService.Setup(session => session.Get("CertificateSession"))
                .Returns(certificateSessionString);
        }

        private Certificate SetupCertificate()
        {
            var certificate = new Builder().CreateNew<Certificate>()
                .With(q => q.CertificateData = JsonConvert.SerializeObject(new Builder()
                    .CreateNew<CertificateData>()
                    .With(x => x.OverallGrade = null)
                    .With(x => x.AchievementDate = null)
                    .Build()))
                .Build();

            certificate.OrganisationId = Guid.NewGuid();

            var organisation = new Builder().CreateNew<Organisation>().Build();

            certificate.Organisation = organisation;

            return certificate;
        }
    }
}
