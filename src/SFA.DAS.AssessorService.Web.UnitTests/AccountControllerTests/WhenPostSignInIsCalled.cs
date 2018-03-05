using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Orchestrators;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    [TestFixture]
    public class WhenPostSignInIsCalled
    {
        private Mock<IHttpContextAccessor> _contextAccessor;
        private Mock<IOrganisationsApiClient> _organisationsApiClient;
        private AccountController _accountController;
        private Mock<IContactsApiClient> _contactsApiClient;
        private Mock<IWebConfiguration> _config;
        private Mock<ILoginOrchestrator> _loginOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();

            _contextAccessor.Setup(a => a.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn"))
                .Returns(new Claim("http://schemas.portal.com/ukprn", "12345"));

            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _contactsApiClient = new Mock<IContactsApiClient>();

            _config = new Mock<IWebConfiguration>();

            _loginOrchestrator = new Mock<ILoginOrchestrator>();

            _accountController = new AccountController(_contextAccessor.Object, _loginOrchestrator.Object);
        }

        [Test]
        public void RoleNotFoundReturnsRedirectToInvalidRolePage()
        {
            _loginOrchestrator.Setup(o => o.Login(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(LoginResult.InvalidRole);
            
            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("InvalidRole");
        } 
        
    }
}