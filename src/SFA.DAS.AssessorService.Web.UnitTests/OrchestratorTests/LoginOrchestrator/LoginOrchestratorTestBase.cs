﻿using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrchestratorTests.LoginOrchestrator
{
    public class LoginOrchestratorTestBase
    {
        protected Orchestrators.Login.LoginOrchestrator LoginOrchestrator;
        protected Mock<IOrganisationsApiClient> OrganisationsApiClient;
        protected Mock<ILoginApiClient> LoginApiClient;

        [SetUp]
        protected void Setup()
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();

            contextAccessor.SetupGet(a => a.HttpContext.User.Claims).Returns(new List<Claim>
            {
                new Claim("http://schemas.portal.com/ukprn", "12345678"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "username"),
                new Claim("http://schemas.portal.com/mail", "email@domain.com"),
                new Claim("http://schemas.portal.com/displayname", "Mr Jones"),
                new Claim("http://schemas.portal.com/service", "EPA"),
                new Claim("http://schemas.portal.com/service", "CSA")
            });

            OrganisationsApiClient = new Mock<IOrganisationsApiClient>();
            LoginApiClient = new Mock<ILoginApiClient>();
            //LoginOrchestrator = new Orchestrators.Login.LoginOrchestrator(new Mock<ILogger<Orchestrators.Login.LoginOrchestrator>>().Object, contextAccessor.Object,
            //    LoginApiClient.Object);
        }
    }
}