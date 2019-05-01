﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Orchestrators.Login;
using SFA.DAS.AssessorService.Web.Validators;

namespace SFA.DAS.AssessorService.Web.UnitTests.AccountControllerTests
{
    [TestFixture]
    public class Given_I_have_signed_in
    {
        private Mock<IHttpContextAccessor> _contextAccessor;
        private AccountController _accountController;
        private Mock<ILoginOrchestrator> _loginOrchestrator;
        private Mock<IWebConfiguration> _webConfigurationMock;
        private Mock<CreateAccountValidator> _validatorMock;
        private Mock<IContactsApiClient> _contactsApiClientMock;

        [SetUp]
        public void Arrange()
        {
            _contextAccessor = new Mock<IHttpContextAccessor>();
            _webConfigurationMock = new Mock<IWebConfiguration>();
            _validatorMock = new Mock<CreateAccountValidator>();
            _contextAccessor.Setup(a => a.HttpContext.User.FindFirst("http://schemas.portal.com/ukprn"))
                .Returns(new Claim("http://schemas.portal.com/ukprn", "12345"));
            _contactsApiClientMock = new Mock<IContactsApiClient>();

            var mockSession = new Mock<ISession>();

            _contextAccessor.SetupGet(a => a.HttpContext.Session).Returns(mockSession.Object);

            _loginOrchestrator = new Mock<ILoginOrchestrator>();

            _accountController = new AccountController(new Mock<ILogger<AccountController>>().Object,
                _loginOrchestrator.Object, new Mock<ISessionService>().Object, _webConfigurationMock.Object, _contactsApiClientMock.Object,
                _validatorMock.Object);
        }

        [Test]
        public void And_I_do_not_have_correct_role_Then_redirect_to_InvalidRole_page()
        {
            _loginOrchestrator.Setup(o => o.Login())
                .ReturnsAsync(new LoginResponse() {Result = LoginResult.InvalidRole});

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("InvalidRole");
        }

        [Test]
        public void And_I_am_not_registered_Then_redirect_to_Organisation_Search_page()
        {
            _loginOrchestrator.Setup(o => o.Login())
                .ReturnsAsync(new LoginResponse() { Result = LoginResult.NotRegistered });

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("OrganisationSearch");
            redirectResult.ActionName.Should().Be("Index");
        }

        [Test]
        public void And_I_am_not_activated_Then_redirect_to_NotActivated_page()
        {
            _loginOrchestrator.Setup(o => o.Login())
                .ReturnsAsync(new LoginResponse() { Result = LoginResult.NotActivated, EndPointAssessorName = "EPA01", EndPointAssessorOrganisationId = "EPA0001" });

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Home");
            redirectResult.ActionName.Should().Be("NotActivated");
        }

        [Test]
        public void And_I_am_valid_Then_redirect_to_Search_page()
        {
            _loginOrchestrator.Setup(o => o.Login())
                .ReturnsAsync(new LoginResponse() { Result = LoginResult.Valid, EndPointAssessorName = "EPA01", EndPointAssessorOrganisationId = "EPA0001"});

            var result = _accountController.PostSignIn().Result;

            result.Should().BeOfType<RedirectToActionResult>();

            var redirectResult = result as RedirectToActionResult;
            redirectResult.ControllerName.Should().Be("Dashboard");
            redirectResult.ActionName.Should().Be("Index");
        }
    }
}