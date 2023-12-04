using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Standard;

public class OptOutStandardConfirmViewModelValidatorTests
{
    [Test, RecursiveMoqAutoData]
    public void MultipleApprovedStandardVersions_Passes(OptOutStandardVersionViewModel viewModel,
                                                        StandardVersion versionOne,
                                                        StandardVersion versionTwo)
    {
        var httpContextAccessor = GetMockHttpContextAccessor();
        var apiClient = GetMockApiClient(versionOne, versionTwo);

        var sut = new OptOutStandardConfirmViewModelValidator(httpContextAccessor.Object, apiClient.Object);

        var result = sut.Validate(viewModel);

        Assert.That(result.IsValid, Is.True);
    }

    [Test, RecursiveMoqAutoData]
    public void OnlyOneApprovedStandardVersion_Fails_WithErrorMessage(OptOutStandardVersionViewModel viewModel,
                                                                      StandardVersion version)
    {
        var httpContextAccessor = GetMockHttpContextAccessor();
        var apiClient = GetMockApiClient(version);

        var sut = new OptOutStandardConfirmViewModelValidator(httpContextAccessor.Object, apiClient.Object);

        var result = sut.Validate(viewModel);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Any(e => e.ErrorMessage == OptOutStandardConfirmViewModelValidator.OneApprovedVersionRequiredMessage));
        });
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
