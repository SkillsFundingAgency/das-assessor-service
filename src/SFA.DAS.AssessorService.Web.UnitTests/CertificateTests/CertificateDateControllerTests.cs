using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.UnitTests.MockedObjects;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.UnitTests.CertificateTests
{
    [TestFixture]
    public class CertificateDateControllerTests
    {
        private Mock<ICertificateApiClient> _certificateApiClient;
        private CertificateDateController _sut;
        private Certificate _certificate;

        [SetUp]
        public void SetUp()
        {
            _certificate = new Certificate
            {
                Id = Guid.NewGuid(),
                Status = CertificateStatus.Draft,
                CertificateData = new CertificateData
                {
                    LearnerGivenNames = "Test",
                    LearnerFamilyName = "Apprentice",
                    OverallGrade = CertificateGrade.Pass,
                    SendTo = CertificateSendTo.None
                }
            };

            _certificateApiClient = new Mock<ICertificateApiClient>();

            _certificateApiClient
                .Setup(client => client.GetCertificate(_certificate.Id, false))
                .ReturnsAsync(_certificate);

            _certificateApiClient
                .Setup(client => client.UpdateCertificate(
                    It.IsAny<UpdateCertificateRequest>()))
                .ReturnsAsync(_certificate);

            var sessionService = new Mock<ISessionService>();

            var localiser = new MockStringLocaliserBuilder()
                .Build<CertificateDateViewModelValidator>();

            _sut = new CertificateDateController(
                Mock.Of<ILogger<CertificateController>>(),
                MockedHttpContextAccessor.Setup().Object,
                _certificateApiClient.Object,
                new CertificateDateViewModelValidator(localiser.Object),
                sessionService.Object);
        }

        [Test]
        public async Task WhenPostingDate_WithSuccessfulGradeAndNoDeliveryDetails_RedirectsToCheck()
        {
            // Arrange
            var achievementDate = new DateTime(2020, 1, 2);

            var viewModel = new CertificateDateViewModel();
            viewModel.FromCertificate(_certificate);

            viewModel.Day = achievementDate.Day.ToString();
            viewModel.Month = achievementDate.Month.ToString();
            viewModel.Year = achievementDate.Year.ToString();

            // Act
            var result = await _sut.Date(viewModel);

            // Assert
            var redirect = result.Should()
                .BeOfType<RedirectToActionResult>()
                .Subject;

            redirect.ControllerName.Should().Be("CertificateCheck");
            redirect.ActionName.Should().Be("Check");

            _certificateApiClient.Verify(
                client => client.UpdateCertificate(
                    It.Is<UpdateCertificateRequest>(request =>
                        request.Action == CertificateActions.Date &&
                        request.Certificate.Id == _certificate.Id &&
                        request.Certificate.CertificateData.AchievementDate
                            == achievementDate &&
                        request.Certificate.CertificateData.SendTo
                            == CertificateSendTo.None &&
                        request.Certificate.CertificateData.ContactName == null &&
                        request.Certificate.CertificateData.ContactAddLine1 == null &&
                        request.Certificate.CertificateData.ContactPostCode == null)),
                Times.Once);
        }
    }
}