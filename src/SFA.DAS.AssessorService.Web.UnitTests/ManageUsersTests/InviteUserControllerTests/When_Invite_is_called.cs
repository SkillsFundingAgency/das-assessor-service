using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Controllers.ManageUsers;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.InviteUserControllerTests
{
    [TestFixture]
    public class When_Invite_is_called
    {
        [Test]
        public async Task Then_ViewResult_is_returned()
        {
            var contactsApiClient = new Mock<IContactsApiClient>();
            contactsApiClient.Setup(api => api.GetPrivileges()).ReturnsAsync(new List<Privilege>());
            var controller = new InviteUserController(contactsApiClient.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IOrganisationsApiClient>().Object);
            var result = await controller.Invite();
            result.Should().BeOfType<ViewResult>();
        }
    }
}