using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query
{
    [TestFixture]
    public class When_GetPrivilegesForContact_called
    {
        [Test]
        public async Task Then_repository_is_called_with_correct_Id()
        {
            var repository = new Mock<IContactQueryRepository>();
            var controller = new ContactQueryController(repository.Object, null, null, null, null, null);
            var userId = Guid.NewGuid();
            await controller.GetPrivilegesForContact(userId);
            
            repository.Verify(repo => repo.GetPrivilegesFor(userId));
        }
    }
}