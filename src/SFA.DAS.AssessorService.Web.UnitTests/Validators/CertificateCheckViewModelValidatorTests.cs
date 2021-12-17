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
    public class CertificateCheckViewModelValidatorTests
    {
        private const string _versionError = "Version error message";
        private const string _optionError = "Option error message";
        private const string _gradeError = "Grade error message";
        private const string _dateError = "Date error message";
        private const string _passDateError = "Pass date error message";
        private const string _failDateError = "Fail date error message";
        private const string _postcodeError = "Postcode error message";
        private const string _cityError = "City error message";
        private const string _addressLine1Error = "Postcode error message";
        private const string _nameError = "Name error message";
        private const string _sendToError = "SentTo error message";

        private Mock<IStringLocalizer<CertificateCheckViewModelValidator>> _mockStringLocalizer;

        private CertificateCheckViewModel _viewModel;

        private CertificateCheckViewModelValidator _validator;

        [SetUp]
        public void Arange()
        {
            _mockStringLocalizer = new Mock<IStringLocalizer<CertificateCheckViewModelValidator>>();

            _mockStringLocalizer.Setup(l => l["VersionCannotBeNull"]).Returns(new LocalizedString("Version", _versionError));
            _mockStringLocalizer.Setup(l => l["OptionCannotBeNull"]).Returns(new LocalizedString("Option", _optionError));
            _mockStringLocalizer.Setup(l => l["GradeCannotBeNull"]).Returns(new LocalizedString("Option", _gradeError));
            _mockStringLocalizer.Setup(l => l["DateCannotBeEmpty"]).Returns(new LocalizedString("Date", _dateError));
            _mockStringLocalizer.Setup(l => l["AchievementDateCannotBeEmpty"]).Returns(new LocalizedString("Date", _passDateError));
            _mockStringLocalizer.Setup(l => l["FailDateCannotBeEmpty"]).Returns(new LocalizedString("Date", _failDateError));
            _mockStringLocalizer.Setup(l => l["PostcodeCannotBeEmpty"]).Returns(new LocalizedString("Postcode", _postcodeError));
            _mockStringLocalizer.Setup(l => l["CityCannotBeEmpty"]).Returns(new LocalizedString("City", _cityError));
            _mockStringLocalizer.Setup(l => l["AddressLine1CannotBeEmpty"]).Returns(new LocalizedString("Address", _addressLine1Error));
            _mockStringLocalizer.Setup(l => l["NameCannotBeEmpty"]).Returns(new LocalizedString("Name", _nameError));
            _mockStringLocalizer.Setup(l => l["SendToCannotBeNone"]).Returns(new LocalizedString("SendTo", _sendToError));

            _viewModel = CreateValidViewModel();

            _validator = new CertificateCheckViewModelValidator(_mockStringLocalizer.Object);
        }

        [Test]
        public void When_AllFieldsAreCorrect_Then_ValidatorReturnsValid()
        {
            var result = _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);       
        }

        [TestCase(null)]
        [TestCase("")]
        public void When_VersionIsNullOrEmpty_Then_ValidatorReturnsInvalid(string version)
        {
            _viewModel.Version = version;

            _validator.ShouldHaveValidationErrorFor(vm => vm.Version, _viewModel);
        }

        [Test]
        public void When_GradeIsNull_Then_ValidatorReturnsInvalid()
        {
            _viewModel.SelectedGrade = null;

            _validator.ShouldHaveValidationErrorFor(vm => vm.SelectedGrade, _viewModel);
        }

        [Test]
        public void When_AchievementDateIsNull_Then_ValidatorReturnsInvalid()
        {
            _viewModel.AchievementDate = null;

            _validator.ShouldHaveValidationErrorFor(vm => vm.AchievementDate, _viewModel);
        }

        [Test]
        public void When_OverallGradeIsNull_And_AchievementDateIsNull_Then_ValidatorReturnsInvalid_WithGenericErrorMessage()
        {
            _viewModel.SelectedGrade = null;
            _viewModel.AchievementDate = null;

            var result = _validator.Validate(_viewModel);

            result.Errors.FirstOrDefault(error => error.PropertyName == "AchievementDate").ErrorMessage.Should().Be(_dateError);
        }

        [Test]
        public void When_OverallGradeIsFail_And_AchievementDateIsNull_Then_ValidatorReturnsInvalid_WithFailErrorMessage()
        {
            _viewModel.SelectedGrade = CertificateGrade.Fail;
            _viewModel.AchievementDate = null;

            var result = _validator.Validate(_viewModel);

            result.Errors.Single().ErrorMessage.Should().Be(_failDateError);
        }

        [TestCase(CertificateGrade.Pass)]
        [TestCase(CertificateGrade.PassWithExcellence)]
        [TestCase(CertificateGrade.Merit)]
        [TestCase(CertificateGrade.Outstanding)]
        [TestCase(CertificateGrade.Distinction)]
        public void When_OverallGradeIsNotFail_And_AchievementDateIsNull_Then_ValidatorReturnsInvalid_WithGenericErrorMessage(string certificateGrade)
        {
            _viewModel.SelectedGrade = certificateGrade;
            _viewModel.AchievementDate = null;

            var result = _validator.Validate(_viewModel);

            result.Errors.Single().ErrorMessage.Should().Be(_passDateError);
        }

        [Test]
        public void When_StandardHasNoOptions_And_OptionIsNull_Then_ValidatorReturnsValid()
        {
            _viewModel.StandardHasOptions = false;
            _viewModel.Option = null;

            var result = _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);
        }

        [Test]
        public void When_StandardHasOptions_And_OptionIsNull_Then_ValidatorReturnInvalid()
        {
            _viewModel.StandardHasOptions = true;
            _viewModel.Option = null;

            _validator.ShouldHaveValidationErrorFor(vm => vm.Option, _viewModel);
        }

        [Test]
        public void When_SelectedGradeIsFail_And_AddressInformationIsNotGiven_Then_ValidatorReturnsValid()
        {
            _viewModel.SelectedGrade = CertificateGrade.Fail;
            _viewModel.Name = null;
            _viewModel.AddressLine1 = null;
            _viewModel.City = null;
            _viewModel.Postcode = null;

            var result = _validator.Validate(_viewModel);
            
            result.IsValid.Should().Be(true);
        }

        [TestCase(CertificateGrade.Pass)]
        [TestCase(CertificateGrade.PassWithExcellence)]
        [TestCase(CertificateGrade.Merit)]
        [TestCase(CertificateGrade.Outstanding)]
        [TestCase(CertificateGrade.Distinction)]
        public void When_SelectedGradeIsPass_And_AddressInformationIsNotGiven_Then_ValidatorReturnsInvalid(string certificateGrade)
        {
            _viewModel.SendTo = CertificateSendTo.Employer;
            _viewModel.SelectedGrade = certificateGrade;
            _viewModel.Name = null;
            _viewModel.AddressLine1 = null;
            _viewModel.City = null;
            _viewModel.Postcode = null;

            _validator.ShouldHaveValidationErrorFor(vm => vm.Name, _viewModel);
            _validator.ShouldHaveValidationErrorFor(vm => vm.AddressLine1, _viewModel);
            _validator.ShouldHaveValidationErrorFor(vm => vm.City, _viewModel);
            _validator.ShouldHaveValidationErrorFor(vm => vm.Postcode, _viewModel);
        }

        private CertificateCheckViewModel CreateValidViewModel()
        {
            return new Builder().CreateNew<CertificateCheckViewModel>()
                .With(p => p.SendTo = CertificateSendTo.Apprentice)
                .Build();
        }
    }
}
