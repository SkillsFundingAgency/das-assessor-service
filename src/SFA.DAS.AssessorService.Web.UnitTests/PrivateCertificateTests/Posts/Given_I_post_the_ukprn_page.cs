using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_ukprn_page : CertificatePostBase
    {
        private RedirectToActionResult _result;        

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateProviderUkprnController =
                new CertificatePrivateProviderUkprnController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
                    );

            var vm = new CertificateUkprnViewModel
            {
                Id = Certificate.Id,
                Ukprn = "10000008",                            
                IsPrivatelyFunded = true
            };
            
            SetupSession();

            var result = certificatePrivateProviderUkprnController.Ukprn(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificateGrade");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("Grade");
        }
    }
}

