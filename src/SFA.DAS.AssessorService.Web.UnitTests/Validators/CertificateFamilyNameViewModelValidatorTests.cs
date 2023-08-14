using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators
{
    public class CertificateFamilyNameViewModelValidatorTests
    {
        private CertificateFamilyNameViewModelValidator _validator;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;

        [SetUp]
        public void Arrange()
        {
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _validator = new CertificateFamilyNameViewModelValidator(_mockCertificateApiClient.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenInputIsValid_ThenReturnsValid(CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupValidViewModel(_baseViewModel);

            var result = await _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);
        }


        [Test, RecursiveMoqAutoData]
        public async Task WhenFamilyNameFieldIsEmpty_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupInvalidViewModel(string.Empty, _baseViewModel);

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("FamilyName");
            }
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenFamilyNameFieldIsNotEqualToPreviousFamilyNameValue_ThenReturnsInvalid(string invalidFamilyName, CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupInvalidViewModel("NotOriginalFamilyNameValue", _baseViewModel);

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("FamilyName");
            }
        }

        private CertificateFamilyNameViewModel SetupValidViewModel(CertificateBaseViewModel _baseViewModel)
        {
            var certData = new CertificateData() { LearnerFamilyName = "FamilyName" };
            var certDataString = JsonConvert.SerializeObject(certData);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = new Guid(),
                    CertificateData = certDataString,
                });

            _baseViewModel.FamilyName = certData.LearnerFamilyName;
            return new CertificateFamilyNameViewModel() { FamilyName = _baseViewModel.FamilyName };
        }

        private CertificateFamilyNameViewModel SetupInvalidViewModel(string invalidFamilyName, CertificateBaseViewModel _baseViewModel)
        {
            var certData = new CertificateData() { LearnerFamilyName = "FamilyName" };
            var certDataString = JsonConvert.SerializeObject(certData);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = new Guid(),
                    CertificateData = certDataString,
                });

            return new CertificateFamilyNameViewModel() { FamilyName = invalidFamilyName };
        }
    }
}
