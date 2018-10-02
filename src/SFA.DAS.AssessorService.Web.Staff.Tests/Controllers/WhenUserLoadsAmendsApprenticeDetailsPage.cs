using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.Controllers;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers
{
    public class WhenUserLoadsAmendsApprenticeDetailsPage : ContractAmendQueryBase
    {
        private IActionResult _result;
        private CertificateApprenticeDetailsViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var certificateApprenticeDetailsController =
                new CertificateApprenticeDetailsController(MockedLogger.Object, MockHttpContextAccessor.Object,
                    ApiClient);
            _result = certificateApprenticeDetailsController.ApprenticeDetail(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateApprenticeDetailsViewModel;
        }

        [Test]
        public void ThenShouldReturnValidLearnerFamilyName()
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.FamilyName.Should().Be(CertificateData.LearnerFamilyName);

        }
    }
}
