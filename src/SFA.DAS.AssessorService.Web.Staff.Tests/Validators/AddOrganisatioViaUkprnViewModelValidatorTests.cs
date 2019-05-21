using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Validators.Roatp;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Validators
{
    [TestFixture]
    public class AddOrganisatioViaUkprnViewModelValidatorTests
    {
        private AddOrganisatioViaUkprnViewModel _viewModel;
        private Mock<IRoatpOrganisationValidator> _roatpOrganisationValidator;
        private Mock<IRoatpApiClient> _apiClient;

        [SetUp]
        public void Before_each_test()
        {
            _apiClient = new Mock<IRoatpApiClient>();
            _apiClient.Setup(x => x.DuplicateUKPRNCheck(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse {DuplicateFound = false, DuplicateOrganisationName = null});
            _roatpOrganisationValidator = new Mock<IRoatpOrganisationValidator>();          
        }

        [Test]
        public void Validator_passes_valid_ukprn()
        {
            var errors = new List<ValidationErrorDetail>();
            _roatpOrganisationValidator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(errors);
            _viewModel = new AddOrganisatioViaUkprnViewModel {UKPRN = "11112222"};

            var validator = new AddOrganisatioViaUkprnViewModelValidator(_roatpOrganisationValidator.Object, _apiClient.Object);
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(0, validationResult.Errors.Count);
        }

        [Test]
        public void Validator_fails_invalid_ukprn()
        {
            var errors = new List<ValidationErrorDetail>
            {
                new ValidationErrorDetail
                {
                    Field = "ukprn",
                    ErrorMessage = "wrong length"
                }
            };
            _roatpOrganisationValidator.Setup(x => x.IsValidUKPRN(It.IsAny<string>())).Returns(errors);
            _viewModel = new AddOrganisatioViaUkprnViewModel { UKPRN = "111222" };

            var validator = new AddOrganisatioViaUkprnViewModelValidator(_roatpOrganisationValidator.Object, _apiClient.Object);
            var validationResult = validator.Validate(_viewModel);

            Assert.AreEqual(1, validationResult.Errors.Count);
        }
    }
}
