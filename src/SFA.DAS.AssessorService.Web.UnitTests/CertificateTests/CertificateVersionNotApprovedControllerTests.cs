
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    public class CertificateVersionNotApprovedControllerTests
    {
        private Mock<ISessionService> _mockSessionService;
        private Mock<IStandardVersionClient> _mockStandardVersionClient;

        private CertificateVersionNotApprovedController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockStandardVersionClient = new Mock<IStandardVersionClient>();

            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns("string");

            _mockSessionService.Setup(s => s.Get("AttemptedStandardVersion")).Returns("ST0001_1.2");

            _mockStandardVersionClient.Setup(c => c.GetStandardVersionByStandardUId("ST0001_1.2"))
                .ReturnsAsync(new StandardVersion { Version = "1.2" });

            _controller = new CertificateVersionNotApprovedController(
                Mock.Of<ILogger<CertificateController>>(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<ICertificateApiClient>(),
                _mockStandardVersionClient.Object,
                _mockSessionService.Object);
        }

        [Test]
        public async Task When_RequestingCertificateVersionNotApproved_Then_ReturnView()
        {
            var result = await _controller.NotApprovedToAssess() as ViewResult;

            result.ViewName.Should().Be("~/Views/Certificate/VersionNotApproved.cshtml");
        }

        [Test]
        public async Task When_RequestingCertificateVersionNotApproved_Then_VersionIsSet()
        {
            var result = await _controller.NotApprovedToAssess() as ViewResult;

            var model = result.Model as CertificateVersionNotApprovedViewModel;

            model.AttemptedVersion.Should().Be("1.2");
        }

        [Test]
        public async Task When_RequestingCertificateVersionNotApproved_And_NoCertificateStoredInSession_Then_RedirectToRecordAGradeSearch()
        {
            _mockSessionService.Setup(s => s.Get(nameof(CertificateSession))).Returns((string)null);

            var result = await _controller.NotApprovedToAssess() as RedirectToActionResult;

            result.ControllerName.Should().Be("Search");
            result.ActionName.Should().Be("Index");
        }
    }
}
