using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_ukprn_page_and_session_does_not_exist : CertificateQueryBase
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

            var result = certificatePrivateProviderUkprnController.Ukprn(false).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;            
        }

        [Test]
        public void ThenShouldRedirectToIndexController()
        {
            _result.ActionName.Should().Be("Index");
        }

        [Test]
        public void ThenShouldRedirectToIndexAction()
        {
            _result.ControllerName.Should().Be("Search");
        }        
    }
}

