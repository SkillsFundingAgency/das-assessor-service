using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_standard_code_page_on_redirect_to_check_page : CertificatePostBase
    {
        private RedirectToActionResult _result;        

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
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
                IsPrivatelyFunded = true
            };
            
            SetupSession();
            AddRedirectCheck();

            var result = certificatePrivateFirstNameController.FirstName(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificateCheck");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Check");
        }
    }
}

