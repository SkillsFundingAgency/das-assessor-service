using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Validators.Standard;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
using System.Security.Claims;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Standard
{
    public class OptInStandardVersionViewModelValidatorTests
    {
        [Test]
        public void OptingIntoOptedInVersion_FailsValidation()
        {
            var viewModel = new OptInStandardVersionViewModel() { Version = "1.2", StandardReference = "ST999" };
            var standardVersion = new StandardVersion() { Version = "1.2" }; // same as the view model's version

            var apiClientMock = new Mock<IStandardVersionClient>();
            apiClientMock.Setup(a => a.GetEpaoRegisteredStandardVersions(It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync(new[] {standardVersion});

            var sut = new OptInStandardVersionViewModelValidator(GetMockHttpContextAccessor().Object, apiClientMock.Object);

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
    }
}
