using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators
{
    public class CertificateCheckViewModelValidatorTests
    {
        private const string VersionError = "Version error message";
        private const string OptionError = "Option error message";
        private const string GradeError = "Grade error message";
        private const string DateError = "Date error message";
        private const string PassDateError = "Pass date error message";
        private const string FailDateError = "Fail date error message";

        private CertificateCheckViewModel _viewModel;
        private CertificateCheckViewModelValidator _sut;

        [SetUp]
        public void SetUp()
        {
            var localizer =
                new Mock<IStringLocalizer<CertificateCheckViewModelValidator>>();

            localizer.Setup(x => x["VersionCannotBeNull"])
                .Returns(new LocalizedString("VersionCannotBeNull", VersionError));

            localizer.Setup(x => x["OptionCannotBeNull"])
                .Returns(new LocalizedString("OptionCannotBeNull", OptionError));

            localizer.Setup(x => x["GradeCannotBeNull"])
                .Returns(new LocalizedString("GradeCannotBeNull", GradeError));

            localizer.Setup(x => x["DateCannotBeEmpty"])
                .Returns(new LocalizedString("DateCannotBeEmpty", DateError));

            localizer.Setup(x => x["AchievementDateCannotBeEmpty"])
                .Returns(new LocalizedString(
                    "AchievementDateCannotBeEmpty", PassDateError));

            localizer.Setup(x => x["FailDateCannotBeEmpty"])
                .Returns(new LocalizedString("FailDateCannotBeEmpty", FailDateError));

            _viewModel = new CertificateCheckViewModel
            {
                Version = "1.0",
                StandardHasOptions = true,
                Option = "Option A",
                SelectedGrade = CertificateGrade.Pass,
                AchievementDate = new DateTime(2026, 10, 1)
            };

            _sut = new CertificateCheckViewModelValidator(localizer.Object);
        }

        [TestCase(CertificateGrade.Pass)]
        [TestCase(CertificateGrade.PassWithExcellence)]
        [TestCase(CertificateGrade.Merit)]
        [TestCase(CertificateGrade.Outstanding)]
        [TestCase(CertificateGrade.Distinction)]
        [TestCase(CertificateGrade.NoGradeAwarded)]
        [TestCase(CertificateGrade.Fail)]
        public void When_AssessmentDetailsAreValid_And_DeliveryDetailsAreMissing_Then_Valid(
            string grade)
        {
            _viewModel.SelectedGrade = grade;

            var result = _sut.Validate(_viewModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(null)]
        [TestCase("")]
        public void When_VersionIsNullOrEmpty_Then_Invalid(string version)
        {
            _viewModel.Version = version;

            var result = _sut.TestValidate(_viewModel);

            result.ShouldHaveValidationErrorFor(x => x.Version)
                .WithErrorMessage(VersionError);
        }

        [Test]
        public void When_GradeIsNull_Then_Invalid()
        {
            _viewModel.SelectedGrade = null;

            var result = _sut.TestValidate(_viewModel);

            result.ShouldHaveValidationErrorFor(x => x.SelectedGrade)
                .WithErrorMessage(GradeError);
        }

        [Test]
        public void When_GradeAndDateAreNull_Then_GenericDateMessageIsUsed()
        {
            _viewModel.SelectedGrade = null;
            _viewModel.AchievementDate = null;

            var result = _sut.Validate(_viewModel);

            result.Errors
                .Single(x => x.PropertyName == nameof(_viewModel.AchievementDate))
                .ErrorMessage.Should().Be(DateError);
        }

        [Test]
        public void When_GradeIsFail_And_DateIsNull_Then_FailDateMessageIsUsed()
        {
            _viewModel.SelectedGrade = CertificateGrade.Fail;
            _viewModel.AchievementDate = null;

            var result = _sut.TestValidate(_viewModel);

            result.ShouldHaveValidationErrorFor(x => x.AchievementDate)
                .WithErrorMessage(FailDateError);
        }

        [TestCase(CertificateGrade.Pass)]
        [TestCase(CertificateGrade.PassWithExcellence)]
        [TestCase(CertificateGrade.Merit)]
        [TestCase(CertificateGrade.Outstanding)]
        [TestCase(CertificateGrade.Distinction)]
        [TestCase(CertificateGrade.NoGradeAwarded)]
        public void When_GradeIsNotFail_And_DateIsNull_Then_AchievementDateMessageIsUsed(
            string grade)
        {
            _viewModel.SelectedGrade = grade;
            _viewModel.AchievementDate = null;

            var result = _sut.TestValidate(_viewModel);

            result.ShouldHaveValidationErrorFor(x => x.AchievementDate)
                .WithErrorMessage(PassDateError);
        }

        [Test]
        public void When_StandardHasNoOptions_And_OptionIsNull_Then_Valid()
        {
            _viewModel.StandardHasOptions = false;
            _viewModel.Option = null;

            var result = _sut.Validate(_viewModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void When_StandardHasOptions_And_OptionIsNull_Then_Invalid()
        {
            _viewModel.Option = null;

            var result = _sut.TestValidate(_viewModel);

            result.ShouldHaveValidationErrorFor(x => x.Option)
                .WithErrorMessage(OptionError);
        }
    }
}