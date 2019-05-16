using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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

        [SetUp]
        public async Task SetUp()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ContactResponse, UserViewModel>();
            });
            
            UserId = Guid.NewGuid();
            CallingUserId = Guid.NewGuid();
            var requestedContactOrganisationId = Guid.NewGuid();
            DifferentOrganisationId = Guid.NewGuid();
            
            ContactsApiClient = new Mock<IContactsApiClient>();
            ContactsApiClient.Setup(apiClient => apiClient.GetById(UserId.ToString())).ReturnsAsync(new ContactResponse
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
            
            ContactsApiClient.Setup(apiClient => apiClient.GetById(CallingUserId.ToString())).ReturnsAsync(new ContactResponse
            {
                Id = CallingUserId,
                OrganisationId = requestedContactOrganisationId
            });
            
            ContactsApiClient.Setup(apiClient => apiClient.GetContactPrivileges(UserId)).ReturnsAsync(new List<ContactsPrivilege>()
            {
                new ContactsPrivilege(),
                new ContactsPrivilege()
            });

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            
            var context = new DefaultHttpContext();
            var claimsPrincipal = new ClaimsPrincipal();

            var claimsIdentity = new ClaimsIdentity(new List<Claim>{new Claim("UserId", CallingUserId.ToString())});
            claimsPrincipal.AddIdentity(claimsIdentity);
            context.User = claimsPrincipal;

            httpContextAccessor.Setup(a => a.HttpContext).Returns(context);
            
            Controller = new UserDetailsController(ContactsApiClient.Object, httpContextAccessor.Object);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            Mapper.Reset();
        }
    }
}