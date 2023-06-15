using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    public class ChangeSignInDetails_Redirect_When_UseGovSignIn_False
    {
        [Test, MoqAutoData]
        public void Then_The_Redirect_To_Home(
            [Frozen] Mock<IWebConfiguration> webConfiguration,
            [Greedy] AccountController controller)
        {
            webConfiguration.Setup(args => args.UseGovSignIn).Returns(false);

            var actual = controller.ChangeSignInDetails() as RedirectToActionResult;

            actual.Should().NotBeNull();
            actual?.ActionName.Should().Be("Index");
            actual?.ControllerName.Should().Be("Home");
        }
    }
}
