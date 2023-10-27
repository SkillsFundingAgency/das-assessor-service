using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using SFA.DAS.Testing.AutoFixture;
using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Standard
{
    public class OptOutStandardVersionViewModelValidatorTests
    {
        [Test]
        public void MultipleApprovedStandardVersions_Passes()
        {
            var viewModel = new OptOutStandardVersionViewModel() { Version = "1.1", StandardReference = "ST999" };
            var standardVersionOne = new StandardVersion() { Version = "1.1" };
            var standardVersionTwo = new StandardVersion() { Version = "1.2" };

            var httpContextAccessor = GetMockHttpContextAccessor();
            var apiClient = GetMockApiClient(standardVersionOne, standardVersionTwo);

            var sut = new OptOutStandardVersionViewModelValidator(httpContextAccessor.Object, apiClient.Object);

            var result = sut.Validate(viewModel);

            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void OnlyOneApprovedStandardVersion_Fails_WithErrorMessage()
        {
            var viewModel = new OptOutStandardVersionViewModel() { Version = "1.1", StandardReference = "ST999" };
            var standardVersion = new StandardVersion() { Version = "1.1" };

            var httpContextAccessor = GetMockHttpContextAccessor();
            var apiClient = GetMockApiClient(standardVersion);

            var sut = new OptOutStandardVersionViewModelValidator(httpContextAccessor.Object, apiClient.Object);

            var result = sut.Validate(viewModel);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsValid, Is.False);
                Assert.That(result.Errors.Exists(e => e.ErrorMessage == OptOutStandardVersionViewModelValidator.OneApprovedVersionRequiredMessage));
            });
        }

        [Test]
        public void EmptyStandardReference_FailsValidation()
        {
            var sut = new OptOutStandardVersionViewModelValidator(GetMockHttpContextAccessor().Object, new Mock<IStandardVersionClient>().Object);
            var viewModel = new OptOutStandardVersionViewModel() { StandardReference = "" };
            var result = sut.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void OptingOutOfOptedOutVersion_FailsValidation()
        {
            var viewModel = new OptOutStandardVersionViewModel() { Version = "1.2", StandardReference = "ST999" };
            var standardVersion = new StandardVersion() { Version = "1.1" }; // different from the view model's version

            var apiClientMock = GetMockApiClient(standardVersion);

            var sut = new OptOutStandardVersionViewModelValidator(GetMockHttpContextAccessor().Object, apiClientMock.Object);

            var result = sut.Validate(viewModel);

            Assert.That(result.IsValid, Is.False);
        }

        private static Mock<IHttpContextAccessor> GetMockHttpContextAccessor()
        {
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(a => a.HttpContext.User.FindFirst(It.IsAny<string>()))
                    .Returns(new Claim("type", "value"));
            return accessor;
        }

        private static Mock<IStandardVersionClient> GetMockApiClient(params StandardVersion[] standardVersionsReturned)
        {
            var apiClient = new Mock<IStandardVersionClient>();
            apiClient.Setup(a => a.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync(standardVersionsReturned);
            return apiClient;
        }
    }
}
