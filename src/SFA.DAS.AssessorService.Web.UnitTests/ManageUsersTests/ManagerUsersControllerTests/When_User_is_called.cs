using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.ManagerUsersControllerTests
{
    [TestFixture]
    public class When_User_is_called
    {
        [Test]
        public async Task Then_a_ViewResult_is_returned()
        {
            var controller = new UserDetailsController();

            var result = await controller.User(Guid.NewGuid());

            result.Should().BeOfType<ViewResult>();
        }
    }
}