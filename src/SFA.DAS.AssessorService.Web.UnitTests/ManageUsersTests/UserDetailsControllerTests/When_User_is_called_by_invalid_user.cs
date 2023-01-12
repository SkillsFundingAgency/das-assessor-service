using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.UnitTests.ManageUsersTests.UserDetailsControllerTests
{
    [TestFixture]
    public class When_User_is_called_by_invalid_user : UserDetailsControllerTestsBase
    {
        [Test]
        public async Task Then_UnauthorizedResult_is_returned()
        {
            ContactsApiClient.Setup(apiClient => apiClient.GetById(CallingUserId)).ReturnsAsync(new ContactResponse
            {
                Id = CallingUserId,
                OrganisationId = DifferentOrganisationId
            });

            var result = await Controller.Details(UserId);

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}