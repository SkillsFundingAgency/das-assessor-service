using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators
{
    public class CertificateAddressViewModelValidatorTests
    {
        private const string _employerError = "Employer error message";
        private const string _addressLine1Error = "Postcode error message";
        private const string _cityError = "City error message";
        private const string _postcodeError = "Postcode error message";
        private const string _postcodeValidError = "Postcode valid error message";
        
        private Mock<IStringLocalizer<CertificateAddressViewModelValidator>> _mockStringLocalizer;

        private CertificateAddressViewModel _viewModel;

        private CertificateAddressViewModelValidator _validator;

        [SetUp]
        public void Arange()
        {
            _mockStringLocalizer = new Mock<IStringLocalizer<CertificateAddressViewModelValidator>>();

            _mockStringLocalizer.Setup(l => l["EmployerCannotBeEmpty"]).Returns(new LocalizedString("Employer", _employerError));
            _mockStringLocalizer.Setup(l => l["AddressLine1CannotBeEmpty"]).Returns(new LocalizedString("Address", _addressLine1Error));
            _mockStringLocalizer.Setup(l => l["CityCannotBeEmpty"]).Returns(new LocalizedString("City", _cityError));
            _mockStringLocalizer.Setup(l => l["PostcodeCannotBeEmpty"]).Returns(new LocalizedString("Postcode", _postcodeError));
            _mockStringLocalizer.Setup(l => l["PostcodeValid"]).Returns(new LocalizedString("Postcode", _postcodeValidError));

            _viewModel = CreateValidViewModel(CertificateSendTo.Apprentice);

            _validator = new CertificateAddressViewModelValidator(_mockStringLocalizer.Object);
        }

        [TestCase(CertificateSendTo.Apprentice)]
        [TestCase(CertificateSendTo.Employer)]
        public void When_AllFieldsAreCorrect_Then_ValidatorReturnsValid(CertificateSendTo sendTo)
        {
            _viewModel = CreateValidViewModel(sendTo);

            var result = _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);
        }

        [TestCase(null)]
        [TestCase("")]
        public void When_SendToIsEmployerAndEmployerIsNullOrEmpty_Then_ValidatorReturnsInvalid(string employer)
        {
            _viewModel = CreateValidViewModel(CertificateSendTo.Employer);
            _viewModel.Employer = employer;

            var result = _validator.TestValidate(_viewModel);
            result.ShouldHaveValidationErrorFor(x => x.Employer);
        }

        [TestCase(CertificateSendTo.Apprentice, null)]
        [TestCase(CertificateSendTo.Apprentice, "")]
        [TestCase(CertificateSendTo.Employer, null)]
        [TestCase(CertificateSendTo.Employer, "")]
        public void When_AddressLine1IsNullOrEmpty_Then_ValidatorReturnsInvalid(CertificateSendTo sendTo, string addressLine1)
        {
            _viewModel = CreateValidViewModel(sendTo);
            _viewModel.AddressLine1 = addressLine1;

            var result = _validator.TestValidate(_viewModel);
            result.ShouldHaveValidationErrorFor(x => x.AddressLine1);
        }

        [TestCase(CertificateSendTo.Apprentice, null)]
        [TestCase(CertificateSendTo.Apprentice, "")]
        [TestCase(CertificateSendTo.Employer, null)]
        [TestCase(CertificateSendTo.Employer, "")]
        public void When_CityIsNullOrEmpty_Then_ValidatorReturnsInvalid(CertificateSendTo sendTo, string city)
        {
            _viewModel = CreateValidViewModel(sendTo);
            _viewModel.City = city;

            var result = _validator.TestValidate(_viewModel);
            result.ShouldHaveValidationErrorFor(x => x.City);
        }

        [TestCase(CertificateSendTo.Apprentice, null)]
        [TestCase(CertificateSendTo.Apprentice, "")]
        [TestCase(CertificateSendTo.Employer, null)]
        [TestCase(CertificateSendTo.Employer, "")]
        public void When_PostcodeIsNullOrEmpty_Then_ValidatorReturnsInvalid(CertificateSendTo sendTo, string postcode)
        {
            _viewModel = CreateValidViewModel(sendTo);
            _viewModel.Postcode = postcode;

            var result = _validator.TestValidate(_viewModel);
            result.ShouldHaveValidationErrorFor(x => x.Postcode);
        }

        [TestCase(CertificateSendTo.Apprentice, "NOT VALID")]
        [TestCase(CertificateSendTo.Employer, "NOT VALID")]
        public void When_PostcodeIsInvalid_Then_ValidatorReturnsInvalid(CertificateSendTo sendTo, string postcode)
        {
            _viewModel = CreateValidViewModel(sendTo);
            _viewModel.Postcode = postcode;

            var result = _validator.TestValidate(_viewModel);
            result.ShouldHaveValidationErrorFor(x => x.Postcode);
        }

        private CertificateAddressViewModel CreateValidViewModel(CertificateSendTo sendTo)
        {
            return new Builder().CreateNew<CertificateAddressViewModel>()
                .With(p => p.SendTo = sendTo)
                .With(p => p.Postcode = "SW1A 2AA")
                .Build();
        }
    }
}
