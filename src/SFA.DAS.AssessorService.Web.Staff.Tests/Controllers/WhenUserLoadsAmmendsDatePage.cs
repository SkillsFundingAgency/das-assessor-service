using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers
{
    public class WhenUserLoadsAmmendsDatePage : ContractAmmendQueryBase
    {
        private IActionResult _result;
        private CertificateDateViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificateDateViewModelValidator = new CertificateDateViewModelValidator();
            var certificateApprenticeDetailsController =
                new CertificateDateController(MockedLogger.Object,
                    MockHttpContextAccessor.Object,
                    ApiClient,
                    certificateDateViewModelValidator);
            _result = certificateApprenticeDetailsController.Date(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateDateViewModel;
        }

        [Test]
        public void ThenShouldReturnValidDay()
        {
            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.Day.PadLeft(2, '0').Should().Be(CertificateData.AchievementDate.Value.ToString("dd"));

        }

        [Test]
        public void ThenShouldReturnValidMonth()
        {
            _viewModelResponse.Month.PadLeft(2, '0').Should().Be(CertificateData.AchievementDate.Value.ToString("MM"));
        }

        [Test]
        public void ThenShouldReturnValidYear()
        {
            _viewModelResponse.Year.Should().Be(CertificateData.AchievementDate.Value.ToString("yyyy"));
        }
    }
}

