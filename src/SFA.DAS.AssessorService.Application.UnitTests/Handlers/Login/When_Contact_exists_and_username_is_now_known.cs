using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
    [TestFixture]
    public class When_Contact_exists_and_username_is_now_known : LoginHandlerTestsBase
    {
        [Test]
        public void Then_contact_username_is_Updated()
        {
            var guid = Guid.NewGuid();

            ContactQueryRepository.Setup(x => x.GetBySignInId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new Contact
                {
                    Id = guid,
                    Username = "Unknown-100",
                    Email = "email@domain.com"
                }));

            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation
            {
                Status = OrganisationStatus.Live,
                EndPointAssessorOrganisationId = "EPA001",
                EndPointAssessorUkprn = 12345,
            });
            ContactQueryRepository.Setup(r => r.GetContact("username")).ReturnsAsync(default(Contact));
            ContactQueryRepository.Setup(r => r.GetContactFromEmailAddress("email@domain.com")).ReturnsAsync(new Contact
            {
                DisplayName = "Display Name",
                Email = "email@domain.com",
                Username = "unknown"
            });

            ContactRepository.Setup(x => x.UpdateUserName(guid, "email@domain.com")).Returns(Task.FromResult(default(object)));

            // Username wiil be ignored and replaced with email address
            Handler.Handle(
                new LoginRequest()
                {
                    Roles = new List<string>() { "ABC", "DEF", "EPA" },
                    UkPrn = 12345,
                    Username = "username",
                    DisplayName = "Display Name",
                    Email = "email@domain.com"
                }, new CancellationToken()).Wait();

            ContactRepository.Verify(m => m.UpdateUserName(guid, "email@domain.com"));
        }
    }
}