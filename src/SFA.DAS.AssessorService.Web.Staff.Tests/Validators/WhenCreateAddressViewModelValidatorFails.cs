using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Staff.Validators;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Validators
{
    public class WhenCreateAddressViewModelValidatorFails
    {
        private static ValidationResult _validationResult;

        [SetUp]
        public void Arrange()
        {
            var certificateAddressViewModel = new Builder()
                .CreateNew<CertificateAddressViewModel>()
                .With(q => q.Employer = "")
                .With(q => q.AddressLine1 = "")
                .With(q => q.AddressLine2 = "")
                .With(q => q.AddressLine3 = "")
                .With(q => q.City = "")
                .With(q => q.Postcode = "")
                .Build();

            _validationResult = new CertificateAddressViewModelValidator().Validate(certificateAddressViewModel);
        }

        [Test]
        public void ThenItShouldFail()
        {
            _validationResult.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenAddress1ShouldDisplayErrorWhenEmpty()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q =>
                q.PropertyName == "AddressLine1" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenEmployeeShouldDisplayErrorWhenEmpty()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q =>
                q.PropertyName == "Employer" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }
        
        [Test]
        public void ThenCityShouldDisplayErrorWhenEmpty()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q =>
                q.PropertyName == "City" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenPostCodeShouldDisplayErrorWhenEmpty()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q =>
                q.PropertyName == "Postcode" && q.ErrorCode == "NotEmptyValidator");
            errors.Should().NotBeNull();
        }

        [Test]
        public void ThenPostCodeShouldDisplayErrorOnFormatError()
        {
            var errors = _validationResult.Errors.FirstOrDefault(q =>
                q.PropertyName == "Postcode" && q.ErrorCode == "RegularExpressionValidator");
            errors.Should().NotBeNull();
        }


    }
}



