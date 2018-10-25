using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.Tests.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_learning_startdate_page : CertificatePostBase
    {
        private RedirectToActionResult _result;      

        [SetUp]
        public void Arrange()
        {
            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();
            
            var validator = 
                new CertificateLearnerStartDateViewModelValidator();

            var certificatePrivateLearnerStartDateController =
                new CertificatePrivateLearnerStartDateController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient,
                    validator                    
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
                    

            var result = certificatePrivateLearnerStartDateController.LearnerStartDate(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificateAmend");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Check");
        }
    }
}

