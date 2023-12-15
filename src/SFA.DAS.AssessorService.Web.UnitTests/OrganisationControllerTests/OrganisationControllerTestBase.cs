using System;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

using SFA.DAS.AssessorService.Web.Controllers;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Azure;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

using OrganisationData = SFA.DAS.AssessorService.Api.Types.Models.AO.OrganisationData;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.OrganisationControllerTests
{
    
    public class OrganisationControllerTestBase
    {
        protected OrganisationController sut;
        protected IActionResult _actionResult;

        protected Mock<IHttpContextAccessor> HttpContextAssessor;
        protected Mock<ITokenService> TokenService;
        protected Mock<ISessionService> SessionService;
        protected Mock<IOrganisationsApiClient> OrganisationApiClient;
        protected Mock<IContactsApiClient> ContactsApiClient;
        protected Mock<IEmailApiClient> EmailApiClient;
        protected Mock<IValidationApiClient> ValidateApiClient;
        protected Mock<IWebConfiguration> WebConfiguration;
        protected Mock<IAzureApiClient> ExternalApiClient;
        protected Mock<ILogger<OrganisationController>> Logger;

        protected string EpaoId = "EPA0001";
        protected Guid OrganisationOneId = Guid.NewGuid();
        protected Guid OrganisationTwoId = Guid.NewGuid();

        protected Guid UserId = Guid.NewGuid();
        protected Guid ManageUsersPrivilegeId = Guid.NewGuid();
        protected Guid ChangeOrganisationPrivilegeId = Guid.NewGuid();

        protected EpaOrganisation EpaOrganisation;
        protected ContactsPrivilege ChangeOrganisationDetailsContactsPrivilege;
        protected ContactsPrivilege ManageUsersContactsPrivilege;

        protected const string ValidPrimaryContact = "valid@valid.com";
        protected const string ValidPrimaryContactDifferentOrganisation = "othervalid@othervalid.com";

        protected const string ValidAddress1 = "Address";
        protected const string ValidAddress2 = "Address";
        protected const string ValidAddress3 = "Address";
        protected const string ValidAddress4 = "Address";
        protected const string ValidPostcode = "PP1 7PP";
        protected const string ValidPhoneNumber = "012345679";
        protected const string ValidEmailAddress = "validcontact@validcompany.com";
        protected const string ValidWebsiteLink = "www.validcompany.com";

        public OrganisationControllerTestBase()
        {
            ChangeOrganisationDetailsContactsPrivilege = new ContactsPrivilege
            {
                Privilege = new Privilege
                {
                    Id = ChangeOrganisationPrivilegeId,
                    Key = Privileges.ChangeOrganisationDetails
                },
                PrivilegeId = ChangeOrganisationPrivilegeId
            };

            ManageUsersContactsPrivilege = new ContactsPrivilege
            {
                Privilege = new Privilege
                {
                    Id = ManageUsersPrivilegeId,
                    Key = Privileges.ManageUsers
                },
                PrivilegeId = ManageUsersPrivilegeId
            };
        }

        public void Arrange(bool addEpaoClaim, bool addUkprnClaim, List<ContactsPrivilege> contactsPrivileges = null)
        {
            HttpContextAssessor = new Mock<IHttpContextAccessor>();

            Logger = new Mock<ILogger<OrganisationController>>();
            SessionService = new Mock<ISessionService>();
            TokenService = new Mock<ITokenService>();
            TokenService.Setup(s => s.GetTokenAsync()).Returns(Task.FromResult("jwt"));

            OrganisationApiClient = new Mock<IOrganisationsApiClient>();
            OrganisationApiClient.Setup(c => c.Get("12345")).ReturnsAsync(new OrganisationResponse() { });

            EpaOrganisation = new EpaOrganisation
            {
                Id = OrganisationOneId,
                OrganisationId = EpaoId,
                OrganisationData = new OrganisationData
                {
                    Address1 = ValidAddress1,
                    Address2 = ValidAddress2,
                    Address3 = ValidAddress3,
                    Address4 = ValidAddress4,
                    Postcode = ValidPostcode,
                    PhoneNumber = ValidPhoneNumber,
                    Email = ValidEmailAddress,
                    WebsiteLink = ValidWebsiteLink
                },
                PrimaryContact = ValidPrimaryContact
            };

            OrganisationApiClient.Setup(c => c.GetEpaOrganisation(EpaoId)).ReturnsAsync(EpaOrganisation);

            WebConfiguration = new Mock<IWebConfiguration>();
            WebConfiguration.Setup(c => c.AzureApiAuthentication).Returns(new AzureApiAuthentication
            {
                ProductId = "1234567"
            });

            ExternalApiClient = new Mock<IAzureApiClient>();
            ExternalApiClient.Setup(c => c.GetUserDetailsByUkprn(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(
                new List<AzureUser>
                {
                    new AzureUser
                    {
                        Subscriptions = new List<AzureSubscription>
                        {
                            new AzureSubscription()
                        }
                    }
                });

            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(c => c.GetPrivileges()).ReturnsAsync(
                new List<Privilege>()
                {
                    new Privilege
                    {
                        Id = ChangeOrganisationPrivilegeId,
                        Key = Privileges.ChangeOrganisationDetails
                    },
                    new Privilege
                    {
                        Id = ManageUsersPrivilegeId,
                        Key = Privileges.ManageUsers
                    }

                });

            ContactsApiClient.Setup(c => c.GetContactPrivileges(UserId)).ReturnsAsync(
                contactsPrivileges);

            EmailApiClient = new Mock<IEmailApiClient>();
            ValidateApiClient = new Mock<IValidationApiClient>();

            var claims = new List<Claim>();
            if(addEpaoClaim)
            {
                claims.Add(new Claim("http://schemas.portal.com/epaoid", EpaoId.ToString()));
            }

            if (addUkprnClaim)
            {
                claims.Add(new Claim("http://schemas.portal.com/ukprn", "12345"));
            }

            claims.Add(new Claim("UserId", UserId.ToString()));

            HttpContextAssessor
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims.ToArray()))
                });

            sut = new OrganisationController(HttpContextAssessor.Object, OrganisationApiClient.Object, ExternalApiClient.Object,
                ContactsApiClient.Object, EmailApiClient.Object, ValidateApiClient.Object,
                Logger.Object, WebConfiguration.Object);
        }
    }
}
