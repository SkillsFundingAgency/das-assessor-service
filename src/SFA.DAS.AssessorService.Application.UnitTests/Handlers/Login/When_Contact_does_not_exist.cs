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
    public class When_Contact_does_not_exist : LoginHandlerTestsBase
    {
        [Test]
        public void Then_a_new_contact_is_created()
        {
            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation
            {
                Status = OrganisationStatus.New,
                EndPointAssessorOrganisationId = "EPA001",
                EndPointAssessorUkprn = 12345,
            });
            ContactQueryRepository.Setup(r => r.GetContact("username")).ReturnsAsync(default(Contact));
            Mediator.Setup(m => m.Send(It.IsAny<CreateContactRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(
                new Contact()
                {
                    Username = "username",
                    DisplayName = "Display Name",
                    Email = "email@domain.com", EndPointAssessorOrganisationId = "EPA001"
                });

            Handler.Handle(
                new LoginRequest()
                {
                    Roles = new List<string>() {"ABC", "DEF", "EPA"},
                    UkPrn = 12345,
                    Username = "username",
                    DisplayName = "Display Name",
                    Email = "email@domain.com"
                }, new CancellationToken()).Wait();

            Mediator.Verify(m =>
                m.Send(
                    It.Is<CreateContactRequest>(r =>
                        r.Username == "username" && r.DisplayName == "Display Name" && r.Email == "email@domain.com" &&
                        r.EndPointAssessorOrganisationId == "EPA001"), It.IsAny<CancellationToken>()));
        }
    }
}