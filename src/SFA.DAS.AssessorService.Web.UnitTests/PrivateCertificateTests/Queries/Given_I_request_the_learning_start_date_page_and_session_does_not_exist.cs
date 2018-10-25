using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Queries
{
    public class Given_I_request_the_learning_start_date_page_and_session_does_not_exist : CertificateQueryBase
    {
        private RedirectToActionResult _result;
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

            var result = certificatePrivateLearnerStartDateController.LearnerStartDate(false).GetAwaiter().GetResult();

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

