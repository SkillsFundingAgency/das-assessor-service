using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_learning_startdate_page : CertificatePostBase
    {
        private RedirectToActionResult _result;      

        [SetUp]
        public void Arrange()
        {
            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();

            var localiser = mockStringLocaliserBuildernew
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<CertificateLearnerStartDateViewModel>();

            CertificateLearnerStartDateViewModelValidator validator = 
                new CertificateLearnerStartDateViewModelValidator(localiser.Object);

            var certificatePrivateLearnerStartDateController =
                new CertificatePrivateLearnerStartDateController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    validator,
                    MockSession.Object
                    );

            var vm = new CertificateLearnerStartDateViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                Day = "12",
                Month = "12",
                Year = "2017",
                IsPrivatelyFunded = true
            };
            
            SetupSession();

            var result = certificatePrivateLearnerStartDateController.LearnerStartDate(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificatePrivateProviderUkprn");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Ukprn");
        }
    }
}

