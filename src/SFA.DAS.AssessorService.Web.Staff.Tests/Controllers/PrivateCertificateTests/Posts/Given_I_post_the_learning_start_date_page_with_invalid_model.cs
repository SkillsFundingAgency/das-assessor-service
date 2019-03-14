using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.Tests.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_learning_startdate_page_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

        [SetUp]
        public void WhenInvalidModelContainsOneError()
        {
            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();            

            CertificateLearnerStartDateViewModelValidator validator =
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
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };

            // view model validation errors will not be trigged as they are attached via fluent
            // validation and these are not connected in tests however this test is actually testing
            // the correct view is returned so the actual error is irrelevant and can be forced
            certificatePrivateLearnerStartDateController.ModelState.AddModelError("", "Error");

            var result = certificatePrivateLearnerStartDateController.LearnerStartDate(vm).GetAwaiter().GetResult();
            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }

        [Test]
        public void ThenShouldReturnLearnerStartDateView()
        {
            _result.ViewName.Should().Be("~/Views/CertificateAmend/LearnerStartDate.cshtml");
        }
    }
}

