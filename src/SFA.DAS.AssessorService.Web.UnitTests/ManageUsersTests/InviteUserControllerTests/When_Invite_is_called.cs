using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.InviteUserControllerTests
{
    [TestFixture]
    public class When_Invite_is_called
    {
        [Test]
        public async Task Then_ViewResult_is_returned()
        {
            var controller = new InviteUserController();
            var result = await controller.Invite();
            result.Should().BeOfType<ViewResult>();
        }
    }
}