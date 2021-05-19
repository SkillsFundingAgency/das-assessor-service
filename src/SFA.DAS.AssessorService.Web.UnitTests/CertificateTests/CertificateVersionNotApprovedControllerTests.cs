
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateVersionNotApprovedControllerTests
    {
        private Mock<ISessionService> _mockSessionService;

        private CertificateVersionNotApprovedController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockSessionService = new Mock<ISessionService>();

            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns("string");

            _controller = new CertificateVersionNotApprovedController(
                Mock.Of<ILogger<CertificateController>>(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<ICertificateApiClient>(),
                Mock.Of<IStandardVersionClient>(),
                _mockSessionService.Object);
        }

        [Test]
        public void When_RequestingCertificateVersionNotApproved_Then_ReturnView()
        {
            var result = _controller.NotApprovedToAssess() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/NotApprovedForVersion.cshtml");
        }

        [Test]
        public void When_RequestingCertificateVersionNotApproved_Then_VersionIsSet()
        {
            _mockSessionService.Setup(s => s.Get("AttemptedStandardVersion")).Returns("1.2");

            var result = _controller.NotApprovedToAssess() as ViewResult;

            var model = result.Model as CertificateVersionNotApprovedViewModel;

            model.AttemptedVersion.Should().Be("1.2");
        }

        [Test]
        public void When_RequestingCertificateVersionNotApproved_And_NoCertificateStoredInSession_Then_RedirectToRecordAGradeSearch()
        {
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns((string)null);

            var result = _controller.NotApprovedToAssess() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }
    }
}
