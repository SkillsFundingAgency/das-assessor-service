using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Validators
{
    public class Given_I_request_the_ukprn_page_with_non_numeric_ukprn : CertificateValidatorMockBase
    {
        private ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            var certificatePrivateProviderUkprnController =
                new CertificatePrivateProviderUkprnController(MockLogger.Object,
                    MockHttpContextAccessor.Object,
                    MockCertificateApiClient,
                    MockSession.Object
                    );

            var assessmentOrgsApiClientMock = new Mock<IAssessmentOrgsApiClient>();

            SetupSession();

            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();

            var localiser = mockStringLocaliserBuildernew
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<CertificateUkprnViewModelValidator>();

            var certificateUkprnViewModelValidator
                = new CertificateUkprnViewModelValidator(localiser.Object,
                    assessmentOrgsApiClientMock.Object);

            var vm = new CertificateUkprnViewModel
            {
                Id = Certificate.Id,
                Ukprn = "AAAAAA",
                IsPrivatelyFunded = true
            };

            _validationResult = certificateUkprnViewModelValidator.Validate(vm);
        }

        [Test]
        public void ThenValidationShoulFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenThereShouldBeOneError()
        {
            _validationResult.Errors.Count.Should().Be(1);
        }

        [Test]
        public void ThenTErrorTypeShouldBePredicateValue()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q => q.PropertyName == "Ukprn" && q.ErrorCode == "PredicateValidator");
            errors.Should().NotBeNull();
        }
    }
}

