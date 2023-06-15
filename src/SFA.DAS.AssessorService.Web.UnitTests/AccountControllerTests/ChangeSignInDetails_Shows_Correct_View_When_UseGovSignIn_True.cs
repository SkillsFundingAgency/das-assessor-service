using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    public class ChangeSignInDetails_When_UseGovSignIn_True
    {
        [Test, MoqAutoData]
        public void Then_The_View_Is_Returned_With_Model(
            [Frozen] Mock<IConfiguration> configuration,
            [Frozen] Mock<IWebConfiguration> webConfiguration,
            [Greedy] AccountController controller)
        {
            //
            configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
            webConfiguration.Setup(args => args.UseGovSignIn).Returns(true);

            var actual = controller.ChangeSignInDetails() as ViewResult;

            actual.Should().NotBeNull();
            var actualModel = actual?.Model as ChangeSignInDetailsViewModel;
            Assert.AreEqual("https://home.integration.account.gov.uk/settings", actualModel?.SettingsLink);
        }
    }
}
