using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.UserDetailsControllerTests
{
    [TestFixture]
    public class UserDetailsControllerTestsBase
    {
        protected UserDetailsController Controller;
        protected Guid UserId;
        protected Mock<IContactsApiClient> ContactsApiClient;
        protected Guid CallingUserId;
        protected Guid DifferentOrganisationId;
        protected Guid PrivilegeId2;
        protected Guid PrivilegeId1;
        protected Guid PrivilegeId3;
        protected IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.CreateMap<ContactResponse, UserViewModel>();
            });
            _mapper = config.CreateMapper();
           
            UserId = Guid.NewGuid();
            CallingUserId = Guid.NewGuid();
            var requestedContactOrganisationId = Guid.NewGuid();
            DifferentOrganisationId = Guid.NewGuid();
            
            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(apiClient => apiClient.GetById(UserId)).ReturnsAsync(new ContactResponse
            {
                Id = UserId,
                Title = "AA",
                GivenNames = "BBBB",
                FamilyName = "CCCC",
                Email = "DDDD",
                PhoneNumber = "EEEE",
                Status = ContactStatus.Active,
                OrganisationId = requestedContactOrganisationId
            });
            
            ContactsApiClient.Setup(apiClient => apiClient.GetById(CallingUserId)).ReturnsAsync(new ContactResponse
            {
                Id = CallingUserId,
                OrganisationId = requestedContactOrganisationId
            });


            PrivilegeId1 = Guid.NewGuid();
            PrivilegeId2 = Guid.NewGuid();
            PrivilegeId3 = Guid.NewGuid();
            
            ContactsApiClient.Setup(apiClient => apiClient.GetContactPrivileges(UserId)).ReturnsAsync(new List<ContactsPrivilege>()
            {
                new ContactsPrivilege{PrivilegeId = PrivilegeId1},
                new ContactsPrivilege{PrivilegeId = PrivilegeId2}
            });

            ContactsApiClient.Setup(apiClient => apiClient.GetPrivileges()).ReturnsAsync(new List<Privilege>
            {
                new Privilege{Id = PrivilegeId1},
                new Privilege{Id = PrivilegeId2},
                new Privilege{Id = PrivilegeId3}
            });
            
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            
            var context = new DefaultHttpContext();
            var claimsPrincipal = new ClaimsPrincipal();

            var claimsIdentity = new ClaimsIdentity(new List<Claim>{new Claim("UserId", CallingUserId.ToString())});
            claimsPrincipal.AddIdentity(claimsIdentity);
            context.User = claimsPrincipal;

            httpContextAccessor.Setup(a => a.HttpContext).Returns(context);
            
            Controller = new UserDetailsController(ContactsApiClient.Object, httpContextAccessor.Object, _mapper, new Mock<IOrganisationsApiClient>().Object);
        }
        
        [TearDown]
        public void TearDown()
        {
        }
    }
}