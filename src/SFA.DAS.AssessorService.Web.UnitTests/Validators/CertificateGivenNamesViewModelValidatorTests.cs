using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Certificate;
using SFA.DAS.Testing.AutoFixture;

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

        [Test]
        public async Task WhenGivenNamesFieldIsEmpty_ThenReturnsInvalid()
        {
            var _viewModel = SetupInvalidViewModel(string.Empty);

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("GivenNames");
            }
        }

        [Test]
        public async Task WhenGivenNamesFieldIsNotEqualToPreviousGivenNamesValue_ThenReturnsInvalid()
        {
            var _viewModel = SetupInvalidViewModel("NotOriginalGivenNamesValue");

            var result = await _validator.Validate(_viewModel);

            using (new AssertionScope())
            {
                result.IsValid.Should().Be(false);
                result.Errors[0].PropertyName.Should().Be("GivenNames");
            }
        }

        private CertificateGivenNamesViewModel SetupValidViewModel(CertificateBaseViewModel _baseViewModel)
        {
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = Guid.NewGuid(),
                    CertificateData = new CertificateData() { LearnerGivenNames = "GivenNames" },
                });

            _baseViewModel.GivenNames = "GivenNames";
            return new CertificateGivenNamesViewModel() { GivenNames = _baseViewModel.GivenNames };
        }

        private CertificateGivenNamesViewModel SetupInvalidViewModel(string invalidGivenNames)
        {
            _mockCertificateApiClient.Setup(s => s.GetCertificate(It.IsAny<Guid>(), false)).ReturnsAsync(
                new Certificate
                {
                    Id = Guid.NewGuid(),
                    CertificateData = new CertificateData() { LearnerGivenNames = "GivenNames" },
                });

            return new CertificateGivenNamesViewModel() { GivenNames = invalidGivenNames };
        }

    }
}
