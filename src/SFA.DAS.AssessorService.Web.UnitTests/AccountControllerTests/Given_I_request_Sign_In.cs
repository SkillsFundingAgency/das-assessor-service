using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.Validators;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    [TestFixture]
    public class Given_I_request_Sign_In
    {
        private AccountController _accountController;
        private Mock<IWebConfiguration> _webConfigurstionMock;
        private Mock<CreateAccountValidator> _validatorMock;

        [SetUp]
        public void Arrange()
        {
            _validatorMock = new Mock<CreateAccountValidator>();
            _webConfigurstionMock = new Mock<IWebConfiguration>();
            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(
                    x => x.Action(
                        It.IsAny<UrlActionContext>()
                    )
                )
                .Returns("callbackUrl")
                .Verifiable();

            _accountController = new AccountController(new Mock<ILogger<AccountController>>().Object, new Mock<ILoginOrchestrator>().Object, new Mock<ISessionService>().Object, _webConfigurstionMock.Object,_validatorMock.Object);

            _accountController.Url = mockUrlHelper.Object;
        }

        [Test]
        public void Then_I_receive_a_ChallengeResult()
        {
            var result = _accountController.SignIn();

            result.Should().BeOfType<ChallengeResult>();
        }
    }
}