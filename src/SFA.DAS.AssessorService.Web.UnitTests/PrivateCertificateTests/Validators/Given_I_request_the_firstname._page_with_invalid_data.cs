using System;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Web.Controllers.Private;
using SFA.DAS.AssessorService.Web.UnitTests.Helpers;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate.Private;

namespace SFA.DAS.AssessorService.Web.UnitTests.PrivateCertificateTests.Validators
{
    public class Given_I_request_the_firstname_page_with_invalid_data : CertificateValidatorMockBase
    {
        private ValidationResult _validationResult;

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

            var mockStringLocaliserBuildernew = new MockStringLocaliserBuilder();

            var localiser = mockStringLocaliserBuildernew
                .WithKey(ResourceMessageName.NoAssesmentProviderFound)
                .WithKeyValue("100000000")
                .Build<CertificateFirstNameViewModel>();

            var certificateFirstNameViewModelValidator
                = new CertificateFirstNameViewModelValidator(localiser.Object);


            var vm = new CertificateFirstNameViewModel
            {
                Id = Certificate.Id,
                FullName = "James Corley",
                FirstName = String.Empty,
                FamilyName = "",
                GivenNames = "James",
                Level = 2,
                Standard = "91",
                IsPrivatelyFunded = true
            };

            _validationResult = certificateFirstNameViewModelValidator.Validate(vm);
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
    }
}

