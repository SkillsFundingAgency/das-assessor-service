using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_ukprn_with_invalid_model : CertificatePostBase
    {
        private ViewResult _result;

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
                Ukprn = "",
                IsPrivatelyFunded = true
            };

            SetupSession();

            certificatePrivateProviderUkprnController.ModelState.AddModelError("", "Error");

            var result = certificatePrivateProviderUkprnController.Ukprn(vm).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldReturnInvalidModelWithOneError()
        {
            _result.ViewData.ModelState.ErrorCount.Should().Be(1);
        }
    }
}

