using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Common.Exceptions;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            contactsApiClient.Verify(
                x => x.UpdateFromGovLogin(
                    It.Is<UpdateContactGovLoginRequest>(c => string.IsNullOrEmpty(c.GovIdentifier))), Times.Once);
        }
        
        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_Gov_User_And_Sign_In_Id_Updated(
            string nameIdentifier,
            string emailAddress,
            ContactResponse contactResponse,
            ContactResponse contactUpdateResponse,
            EpaOrganisation epaOrganisation)
        {
            contactResponse.SignInId = null;
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
            contactsApiClient.Setup(x => x.UpdateFromGovLogin(It.Is<UpdateContactGovLoginRequest>(c =>
                c.GovIdentifier.Equals(nameIdentifier) 
                && c.ContactId.Equals(contactResponse.Id)
                && !c.SignInId.Equals(Guid.Empty)))).ReturnsAsync(contactUpdateResponse);
        
            var actual = await handler.GetClaims(tokenValidatedContext);
        
            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(contactResponse.Email);
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactUpdateResponse.SignInId.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());
        }

        [Test, MoqAutoData]
        public async Task Then_The_Claims_Are_Populated_For_Gov_User_When_GovSignIn_Email_Changes
        (
            string nameIdentifier,
            string emailAddress,
            string newEmailAddress,
            ContactResponse contactResponse,
            EpaOrganisation epaOrganisation,
            [Frozen] Mock<IContactsApiClient> contactsApiClient,
            [Frozen] Mock<IOrganisationsApiClient> organisationApiClient,
            [Frozen] Mock<ISessionService> sessionService,
            [Frozen] Mock<IWebConfiguration> webConfiguration,
            AssessorServiceAccountPostAuthenticationClaimsHandler handler)
        {
            contactResponse.Email = emailAddress;
            var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, newEmailAddress, "");
            contactsApiClient.Setup(x => x.GetContactBySignInId(null)).ThrowsAsync(new EntityNotFoundException());
            contactsApiClient.Setup(x => x.GetContactByEmail(newEmailAddress)).ThrowsAsync(new EntityNotFoundException());
            contactsApiClient.Setup(x => x.GetContactByGovIdentifier(nameIdentifier)).ReturnsAsync(contactResponse);
            organisationApiClient.Setup(x => x.GetEpaOrganisationById(contactResponse.OrganisationId.ToString()))
                .ReturnsAsync(epaOrganisation);

            var actual = await handler.GetClaims(tokenValidatedContext);

            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(newEmailAddress);
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactResponse.SignInId.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId);
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());

            contactsApiClient.Verify(x =>x.UpdateEmail(It.Is<UpdateEmailRequest>(c=>
                c.NewEmail.Equals(newEmailAddress)
                && c.GovUkIdentifier.Equals(contactResponse.GovUkIdentifier)
                )), Times.Once);
        }
        [Test, MoqAutoData]
        public async Task Then_The_User_Status_Is_Updated_If_Pending
        (
            string nameIdentifier,
            string emailAddress,
            ContactResponse contactResponse,
            ContactResponse contactUpdateResponse,
            EpaOrganisation epaOrganisation)
        {
            contactResponse.Status = "Pending";
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
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactResponse.SignInId.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());
            contactsApiClient.Verify(x => x.Callback(It.Is<SignInCallback>(c =>
                c.GovIdentifier.Equals(nameIdentifier) && c.Sub.Equals(contactResponse.SignInId.ToString()) &&
                c.SourceId.Equals(contactResponse.Id.ToString()))), Times.Once);
        }
        
        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_Sign_In_Id_Not_Updated(
            string nameIdentifier,
            string emailAddress,
            ContactResponse contactResponse,
            ContactResponse contactUpdateResponse,
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
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactResponse.SignInId.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());
            contactsApiClient.Verify(x => x.UpdateFromGovLogin(It.Is<UpdateContactGovLoginRequest>(c=>
                c.GovIdentifier.Equals(nameIdentifier)
                && c.ContactId.Equals(contactResponse.Id)
                && c.SignInId.Equals(contactResponse.SignInId))), Times.Once);
        }
        
        
        
        [Test, AutoData]
        public async Task Then_The_Claims_Are_Populated_For_New_User_And_Sign_In_Id_Not_Updated_If_Gov_User_With_Sign_In_Id(
            string nameIdentifier,
            string emailAddress,
            Guid signInId,
            ContactResponse contactResponse,
            ContactResponse contactUpdateResponse,
            EpaOrganisation epaOrganisation)
        {
            contactResponse.SignInId = null;
            var contactsApiClient = new Mock<IContactsApiClient>();
            var organisationApiClient = new Mock<IOrganisationsApiClient>();
            var sessionService = new Mock<ISessionService>();
            var webConfiguration = new Mock<IWebConfiguration>();
            var handler =
                new AssessorServiceAccountPostAuthenticationClaimsHandler(
                    Mock.Of<ILogger<AssessorServiceAccountPostAuthenticationClaimsHandler>>(), contactsApiClient.Object,
                    organisationApiClient.Object, sessionService.Object, webConfiguration.Object);
            
            var tokenValidatedContext = ArrangeTokenValidatedContext(nameIdentifier, emailAddress, signInId.ToString());
            contactsApiClient.Setup(x => x.GetContactBySignInId(signInId.ToString())).ThrowsAsync(new EntityNotFoundException());
            contactsApiClient.Setup(x => x.GetContactByEmail(emailAddress)).ReturnsAsync(contactResponse);
            organisationApiClient.Setup(x => x.GetEpaOrganisationById(contactResponse.OrganisationId.ToString()))
                .ReturnsAsync(epaOrganisation);
            contactsApiClient.Setup(x => x.UpdateFromGovLogin(It.Is<UpdateContactGovLoginRequest>(c=>
                c.GovIdentifier.Equals(nameIdentifier)
                && c.ContactId.Equals(contactResponse.Id)
                && c.SignInId.Equals(signInId)))).ReturnsAsync(contactUpdateResponse);
            
        
            var actual = await handler.GetClaims(tokenValidatedContext);
        
            actual.First(c => c.Type.Equals("UserId")).Value.Should().Be(contactResponse.Id.ToString());
            actual.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")).Value.Should().Be(contactResponse.Username);
            actual.First(c => c.Type.Equals("display_name")).Value.Should().Be(contactResponse.DisplayName);
            actual.First(c => c.Type.Equals("email")).Value.Should().Be(contactResponse.Email);
            actual.First(c => c.Type.Equals("sub")).Value.Should().Be(contactUpdateResponse.SignInId.ToString());
            actual.First(c => c.Type.Equals("http://schemas.portal.com/epaoid")).Value.Should().Be(epaOrganisation.OrganisationId); 
            actual.First(c => c.Type.Equals("http://schemas.portal.com/ukprn")).Value.Should().Be(epaOrganisation.Ukprn.ToString());
            
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