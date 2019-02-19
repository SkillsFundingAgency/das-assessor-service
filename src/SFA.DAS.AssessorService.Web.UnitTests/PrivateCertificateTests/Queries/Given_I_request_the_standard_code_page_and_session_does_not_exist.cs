using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_standard_code_page_and_session_does_not_exist : CertificateQueryBase
    {
        private RedirectToActionResult _result;
        private CertificateStandardCodeListViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockAssessmentOrgsApiClient,
                    new CacheHelper(mockDistributedCache.Object),
                    MockCertificateApiClient,
                    MockSession.Object,
                    MockStandardService.Object
                    );
          
            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId"))
                .Returns("EPA00001");
           
            var result = certificatePrivateStandardCodeController.StandardCode(false).GetAwaiter().GetResult();

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

