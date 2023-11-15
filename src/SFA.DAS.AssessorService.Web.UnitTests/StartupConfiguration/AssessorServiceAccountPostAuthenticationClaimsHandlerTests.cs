using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;

namespace SFA.DAS.AssessorService.Web.UnitTests.StartupConfiguration
{
    public class AssessorServiceAccountPostAuthenticationClaimsHandlerTests
    {

        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_Non_Gov_User(
            string nameIdentifier,
            string emailAddress,
            string sub,
            ContactResponse contactResponse,
            EpaOrganisation epaOrganisation)
        {
            var contactsApiClient = new Mock<IContactsApiClient>();
            var organisationApiClient = new Mock<IOrganisationsApiClient>();
            var sessionService = new Mock<ISessionService>();
            var webConfiguration = new Mock<IWebConfiguration>();
            var handler =
                new AssessorServiceAccountPostAuthenticationClaimsHandler(
                    Mock.Of<ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler>>(), contactsApiClient.Object,
                    organisationApiClient.Object, sessionService.Object, webConfiguration.Object);
            
            var tokenValidatedContext = ArrangeTokenValidatedContext("", emailAddress, sub);
            contactsApiClient.Setup(x => x.GetContactBySignInId(sub)).ReturnsAsync(contactResponse);
            organisationApiClient.Setup(x => x.GetEpaOrganisationById(contactResponse.OrganisationId.ToString()))
                .ReturnsAsync(epaOrganisation);
        
            var actual = await handler.GetClaims(tokenValidatedContext);
        
            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(contactResponse.Email); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString()); 
        }
        
        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_Gov_User(
            string nameIdentifier,
            string emailAddress,
            ContactResponse contactResponse,
            EpaOrganisation epaOrganisation)
        {
            var contactsApiClient = new Mock<IContactsApiClient>();
            var organisationApiClient = new Mock<IOrganisationsApiClient>();
            var sessionService = new Mock<ISessionService>();
            var webConfiguration = new Mock<IWebConfiguration>();
            var handler =
                new AssessorServiceAccountPostAuthenticationClaimsHandler(
                    Mock.Of<ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler>>(), contactsApiClient.Object,
                    organisationApiClient.Object, sessionService.Object, webConfiguration.Object);
            
            var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, emailAddress, "");
            contactsApiClient.Setup(x => x.GetContactByEmail(emailAddress)).ReturnsAsync(contactResponse);
            organisationApiClient.Setup(x => x.GetEpaOrganisationById(contactResponse.OrganisationId.ToString()))
                .ReturnsAsync(epaOrganisation);
        
            var actual = await handler.GetClaims(tokenValidatedContext);
        
            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(contactResponse.Email);
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString()); 
        }

        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_Gov_User_When_GovSignIn_Email_Changes
        (
            string nameIdentifier,
            string emailAddress,
            string newEmailAddress,
            ContactResponse contactResponse,
            EpaOrganisation epaOrganisation)
        {
            contactResponse.Email = emailAddress;
            var contactsApiClient = new Mock<IContactsApiClient>();
            var organisationApiClient = new Mock<IOrganisationsApiClient>();
            var sessionService = new Mock<ISessionService>();
            var webConfiguration = new Mock<IWebConfiguration>();
            var handler =
                new AssessorServiceAccountPostAuthenticationClaimsHandler(
                    Mock.Of<ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler>>(), contactsApiClient.Object,
                    organisationApiClient.Object, sessionService.Object, webConfiguration.Object);

            var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, newEmailAddress, "");
            contactsApiClient.Setup(x => x.GetContactByEmail(newEmailAddress)).ReturnsAsync(contactResponse);
            organisationApiClient.Setup(x => x.GetEpaOrganisationById(contactResponse.OrganisationId.ToString()))
                .ReturnsAsync(epaOrganisation);
            webConfiguration.Setup(x => x.UseGovSignIn).Returns(true);

            var actual = await handler.GetClaims(tokenValidatedContext);

            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(newEmailAddress);
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId);
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());

            contactsApiClient.Verify(x =>x.UpdateEmail(It.IsAny<UpdateEmailRequest>()), Times.Once);
        }


        private TokenValidatedContext ArrangeTokenValidatedContext(string nameIdentifier, string emailAddress, string sub)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, emailAddress)
            };
            
            if (!string.IsNullOrEmpty(nameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            }
            
            if (!string.IsNullOrEmpty(sub))
            {
                claims.Add(new Claim("sub", sub));
            }
            
            var identity = new ClaimsIdentity(claims);
        
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
            return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","", typeof(TestAuthHandler)),
                new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
            {
                Principal = claimsPrincipal
            };
        }
        
        private class TestAuthHandler : IAuthenticationHandler
        {
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }

            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}