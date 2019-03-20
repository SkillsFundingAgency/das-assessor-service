using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
    [TestFixture]
    public class When_Contact_exists_and_username_is_now_known : LoginHandlerTestsBase
    {
        [Test]
        public void Then_contact_username_is_Updated()
        {
            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation
            {
                Status = OrganisationStatus.New,
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


            Handler.Handle(
                new LoginRequest()
                {
                    Roles = new List<string>() { "ABC", "DEF", "EPA" },
                    UkPrn = 12345,
                    Username = "username", // New Username provided by login
                    DisplayName = "Display Name",
                    Email = "email@domain.com"
                }, new CancellationToken()).Wait();

            Mediator.Verify(m =>
                m.Send(
                    It.Is<UpdateContactRequest>(r =>
                        r.UserName == "username" && r.DisplayName == "Display Name" && r.Email == "email@domain.com"),
                    It.IsAny<CancellationToken>()));
        }
    }
}