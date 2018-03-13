using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    [TestFixture]
    public class Given_I_have_signed_in
    {
        private Mock<IHttpContextAccessor> _contextAccessor;
        private AccountController _accountController;
        private Mock<ILoginOrchestrator> _loginOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();

            _contextAccessor.Setup(a => a.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn"))
                .Returns(new Claim("http://schemas.portal.com/ukprn", "12345"));

            _loginOrchestrator = new Mock<ILoginOrchestrator>();

            _accountController = new AccountController(_contextAccessor.Object, _loginOrchestrator.Object,
                new Mock<ILogger<AccountController>>().Object);
        }

        [Test]
        public void And_I_do_not_have_correct_role_Then_redirect_to_InvalidRole_page()
        {
            _loginOrchestrator.Setup(o => o.Login(It.IsAny<HttpContext>())).ReturnsAsync(LoginResult.InvalidRole);
            
            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("InvalidRole");
        }

        [Test]
        public void And_I_am_not_registered_Then_redirect_to_NotRegistered_page()
        {
            _loginOrchestrator.Setup(o => o.Login(It.IsAny<HttpContext>())).ReturnsAsync(LoginResult.NotRegistered);

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("NotRegistered");
        }

        [Test]
        public void And_I_am_valid_Then_redirect_to_Organisation_page()
        {
            _loginOrchestrator.Setup(o => o.Login(It.IsAny<HttpContext>())).ReturnsAsync(LoginResult.Valid);

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Organisation");
            redirectResult.ActionName.Should().Be("Index");
        }

    }
}