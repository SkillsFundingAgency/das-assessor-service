using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Private;
using SFA.DAS.AssessorService.Web.Staff.Tests.Helpers;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Private;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Controllers.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_learning_start_date_page : CertificateQueryBase
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
                new CertificateLearnerStartDateViewModelValidator();

            var certificatePrivateLearnerStartDateController =
                new CertificatePrivateLearnerStartDateController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockApiClient,
                    certificateLearnerStartDateViewModelValidator                    
                    );            

            _result = certificatePrivateLearnerStartDateController.LearnerStartDate(Certificate.Id, "", 1).GetAwaiter().GetResult();

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

