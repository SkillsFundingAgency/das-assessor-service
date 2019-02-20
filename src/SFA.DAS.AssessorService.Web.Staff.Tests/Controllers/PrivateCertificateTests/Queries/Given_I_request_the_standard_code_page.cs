using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.ExternalApis.Services;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_standard_code_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateStandardCodeListViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();

            var certificatePrivateStandardCodeController =
                new CertificatePrivateStandardCodeController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockAssessmentOrgsApiClient,
                    new CacheService(mockDistributedCache.Object),
                    MockApiClient,
                    MockStandardService.Object
                    );      

            _result = certificatePrivateStandardCodeController.StandardCode(Certificate.Id).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateStandardCodeListViewModel;
        }

        [Test]
        public void ThenShouldReturnStandardCode()
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.SelectedStandardCode.Should().Be(Certificate.StandardCode.ToString());
        }
    }
}

