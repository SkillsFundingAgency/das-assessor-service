using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_FirstName_Page_And_Session_Does_Not_Exist : CertificateQueryBase
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

            var result = certificatePrivateFirstNameController.FirstName(false).GetAwaiter().GetResult();

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

