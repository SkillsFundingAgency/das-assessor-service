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

        [SetUp]
        public void Arrange()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();

            _contextAccessor.Setup(a => a.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn"))
                .Returns(new Claim("http://schemas.portal.com/ukprn", "12345"));

            _organisationsApiClient = new Mock<IOrganisationsApiClient>();
            _contactsApiClient = new Mock<IContactsApiClient>();

            _config = new Mock<IWebConfiguration>();

            _accountController = new AccountController(_contextAccessor.Object, _organisationsApiClient.Object,
                _config.Object, _contactsApiClient.Object);
        }

        [Test]
        public void RoleNotFoundReturnsRedirectToInvalidRolePage()
        {
            _contextAccessor.Setup(a => a.HttpContext.User.HasClaim("http://schemas.portal.com/service", "EPA")).Returns(false);

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("InvalidRole");
        } 
        
    }
}