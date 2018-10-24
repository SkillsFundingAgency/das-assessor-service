using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_firstName_page_on_redirect_to_check_page : CertificateQueryBase
    {
        private ViewResult _result;

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateFirstNameController =
                new CertificatePrivateFirstNameController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
                    );

            SetupSession();
            AddRedirectCheck();

            var result = certificatePrivateFirstNameController.FirstName(false).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldHaveBackCheckFlagSet()
        {
            (_result.Model as CertificateFirstNameViewModel).BackToCheckPage.Should().Be(true);
        }
    }
}

