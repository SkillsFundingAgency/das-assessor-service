using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.ManagerUsersControllerTests
{
    [TestFixture]
    public class When_User_is_called
    {
        private UserDetailsController _controller;
        private Guid _userId;
        private IActionResult _result;
        private Mock<IContactsApiClient> _contactsApiClient;

        [SetUp]
        public async Task SetUp()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ContactResponse, UserViewModel>();
            });
            
            _userId = Guid.NewGuid();
            
            _contactsApiClient = new Mock<IContactsApiClient>();
            _contactsApiClient.Setup(apiClient => apiClient.GetById(_userId.ToString())).ReturnsAsync(new ContactResponse
            {
                Id = _userId,
                Title = "AA",
                GivenNames = "BBBB",
                FamilyName = "CCCC",
                Email = "DDDD",
                PhoneNumber = "EEEE",
                Status = ContactStatus.Active                
            });

            _contactsApiClient.Setup(apiClient => apiClient.GetContactPrivileges(_userId)).ReturnsAsync(new List<ContactsPrivilege>()
            {
                new ContactsPrivilege(),
                new ContactsPrivilege()
            });
            
            _controller = new UserDetailsController(_contactsApiClient.Object);
            _result = await _controller.User(_userId);
        }

        [TearDown]
        public async Task TearDown()
        {
            Mapper.Reset();
        }
        
        [Test]
        public async Task Then_a_ViewResult_is_returned()
        {
            _result.Should().BeOfType<ViewResult>();
        }

        [Test]
        public async Task Then_ViewResult_should_contain_UserViewModel()
        {
            _result.As<ViewResult>().Model.Should().BeOfType<UserViewModel>();
        }

        [Test]
        public async Task Then_UserViewModel_contains_correct_user_details()
        {
            var expectedViewModel = new UserViewModel
            {
                Id = _userId,
                Title = "AA",
                GivenNames = "BBBB",
                FamilyName = "CCCC",
                Email = "DDDD",
                PhoneNumber = "EEEE", 
                Status = ContactStatus.Active,
                ActionRequired = "No action required",
                AssignedPrivileges = new List<ContactsPrivilege>{new ContactsPrivilege(), new ContactsPrivilege()}
            };
            _result.As<ViewResult>().Model.As<UserViewModel>().Should().BeEquivalentTo(expectedViewModel);
        }

        [Test]
        public async Task Then_UserViewModel_contains_correct_privileges()
        {
            _result.As<ViewResult>().Model.As<UserViewModel>().AssignedPrivileges.Count.Should().Be(2);
        }
    }
}