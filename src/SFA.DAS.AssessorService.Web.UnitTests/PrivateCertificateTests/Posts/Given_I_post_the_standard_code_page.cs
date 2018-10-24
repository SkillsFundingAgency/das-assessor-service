using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Posts
{
    public class Given_I_post_the_standard_code_page : CertificatePostBase
    {
        private RedirectToActionResult _result;
        private CertificateFirstNameViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
             var distributedCacheMock = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockAssessmentOrgsApiClient,
                    new CacheHelper(distributedCacheMock.Object), 
                    MockCertificateApiClient,
                    MockSession.Object
                    );

            var vm = new CertificateStandardCodeListViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                SelectedStandardCode = "93",
                IsPrivatelyFunded = true
            };
            
            SetupSession();

            MockSession.Setup(q => q.Get("EndPointAsessorOrganisationId"))
                .Returns("EPA00001");

            var result = certificatePrivateStandardCodeController.StandardCode(vm).GetAwaiter().GetResult();

            _result = result as RedirectToActionResult;
        }

        [Test]
        public void ThenShouldReturnRedirectToController()
        {
            _result.ControllerName.Should().Be("CertificatePrivateLearnerStartDate");
        }

        [Test]
        public void ThenShouldReturnRedirectToAction()
        {
            _result.ActionName.Should().Be("LearnerStartDate");
        }
    }
}

