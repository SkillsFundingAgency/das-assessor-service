using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_learning_tart_date_page : CertificateQueryBase
    {
        private IActionResult _result;
        private CertificateLearnerStartDateViewModel _viewModelResponse;

        [SetUp]
        public void Arrange()
        {         
            MockStringLocaliserBuilder mockStringLocaliserBuilder;
            mockStringLocaliserBuilder = new MockStringLocaliserBuilder();

            Mock<IStringLocalizer<CertificateLearnerStartDateViewModelValidator>>
                mockLocaliser = new Mock<IStringLocalizer<CertificateLearnerStartDateViewModelValidator>>();

            mockLocaliser = mockStringLocaliserBuilder
                .WithKey("XXXX")
                .WithKeyValue("100000000")
                .Build<CertificateLearnerStartDateViewModelValidator>();

            var certificateLearnerStartDateViewModelValidator =
                new CertificateLearnerStartDateViewModelValidator(mockLocaliser.Object);

            var certificatePrivateLearnerStartDateController =
                new CertificatePrivateLearnerStartDateController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    certificateLearnerStartDateViewModelValidator,
                    MockSession.Object
                    );

            SetupSession();

            _result = certificatePrivateLearnerStartDateController.LearnerStartDate(false).GetAwaiter().GetResult();

            var result = _result as ViewResult;
            _viewModelResponse = result.Model as CertificateLearnerStartDateViewModel;
        }

        [Test]
        public void ThenShouldReturnFirstName()
        {
            var certificateData = JsonConvert.DeserializeObject<CertificateData>(Certificate.CertificateData);

            _viewModelResponse.Id.Should().Be(Certificate.Id);
            _viewModelResponse.StartDate.Should().Be(CertificateData.LearningStartDate);

        }
    }
}

