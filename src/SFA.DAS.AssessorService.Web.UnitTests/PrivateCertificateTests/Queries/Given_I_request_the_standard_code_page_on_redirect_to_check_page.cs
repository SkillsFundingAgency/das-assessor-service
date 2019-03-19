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
    public class Given_I_request_the_standard_code_page_on_redirect_to_check_page : CertificateQueryBase
    {
        private ViewResult _result;
        private CertificateStandardCodeListViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            var mockDistributedCache = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockAssessmentOrgsApiClient,
                    new CacheHelper(mockDistributedCache.Object),
                    MockCertificateApiClient,
                    MockSession.Object,
                    MockStandardServiceClient.Object
                    );

            SetupSession();
            AddRedirectCheck();

            var result = certificatePrivateStandardCodeController.StandardCode(false).GetAwaiter().GetResult();

            _result = result as ViewResult;
        }

        [Test]
        public void ThenShouldHaveBackCheckFlagSet()
        {
            (_result.Model as CertificateStandardCodeListViewModel).BackToCheckPage.Should().Be(true);
        }
    }
}

