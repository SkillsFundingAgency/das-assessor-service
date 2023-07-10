using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation.TestHelper;
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
    [TestFixture]
    public class CertificateGivenNamesViewModelValidatorTests
    {
        private CertificateGivenNamesViewModelValidator _validator;
        private Mock<ICertificateApiClient> _mockCertificateApiClient;

        [SetUp]
        public void Arrange()
        {
            _mockCertificateApiClient = new Mock<ICertificateApiClient>();
            _validator = new CertificateGivenNamesViewModelValidator(_mockCertificateApiClient.Object);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenInputIsValid_ThenReturnsValid(CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupValidViewModel(_baseViewModel);

            var result = await _validator.Validate(_viewModel);

            result.IsValid.Should().Be(true);
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenGivenNamesFieldIsEmpty_ThenReturnsInvalid(CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupInvalidViewModel(string.Empty, _baseViewModel);

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("GivenNames");
            }
        }

        [Test, RecursiveMoqAutoData]
        public async Task WhenGivenNamesFieldIsNotEqualToPreviousGivenNamesValue_ThenReturnsInvalid(string invalidGivenNames, CertificateBaseViewModel _baseViewModel)
        {
            var _viewModel = SetupInvalidViewModel(invalidGivenNames, _baseViewModel);

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("GivenNames");
            }
        }

        private CertificateGivenNamesViewModel SetupValidViewModel(CertificateBaseViewModel _baseViewModel)
        {
            var certData = new CertificateData() { LearnerGivenNames = "GivenNames" };
            var certDataString = JsonConvert.SerializeObject(certData);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = new Guid(),
                    CertificateData = certDataString,
                });

            _baseViewModel.GivenNames = certData.LearnerGivenNames;
            return new CertificateGivenNamesViewModel() { GivenNames = _baseViewModel.GivenNames };
        }

        private CertificateGivenNamesViewModel SetupInvalidViewModel(string invalidGivenNames, CertificateBaseViewModel _baseViewModel)
        {
            var certData = new CertificateData() { LearnerGivenNames = "GivenNames" };
            var certDataString = JsonConvert.SerializeObject(certData);
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = new Guid(),
                    CertificateData = certDataString,
                });

            return new CertificateGivenNamesViewModel() { GivenNames = invalidGivenNames };
        }

    }
}
