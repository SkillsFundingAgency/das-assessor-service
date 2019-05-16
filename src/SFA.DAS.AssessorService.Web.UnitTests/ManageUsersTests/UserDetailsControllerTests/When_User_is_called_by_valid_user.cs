using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers.ViewModels;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.UserDetailsControllerTests
{
    [TestFixture]
    public class When_User_is_called_by_valid_user : UserDetailsControllerTestsBase
    {
        private IActionResult _result;

        [SetUp]
        public async Task Arrange()
        {
            _result = await Controller.User(UserId);
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
                Id = UserId,
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