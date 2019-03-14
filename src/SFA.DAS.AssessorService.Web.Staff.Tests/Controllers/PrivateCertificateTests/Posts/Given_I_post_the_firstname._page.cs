using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_firstname_page : CertificatePostBase
    {
        private RedirectToActionResult _result;        

        [SetUp]
        public void WhenValidModelContainsNoErrors()
        {
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient                
                    );

            var vm = new CertificateFirstNameViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                FirstName = "James",
                FamilyName = "Corley",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true,
                ReasonForChange = "Required reason for change"
            };                      

            var result = certificatePrivateFirstNameController.FirstName(vm).GetAwaiter().GetResult();

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

