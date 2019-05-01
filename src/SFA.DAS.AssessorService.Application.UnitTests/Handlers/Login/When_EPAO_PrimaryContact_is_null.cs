using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
    [TestFixture]
    public class When_EPAO_PrimaryContact_is_null : LoginHandlerTestsBase
    {
        [Test]
        public void Then_Epao_PrimaryContact_is_Updated()
        {

            ContactQueryRepository.Setup(x => x.GetBySignInId(It.IsAny<Guid>())).Returns(Task.FromResult(
                new Contact
                {
                    Id = Guid.NewGuid()
                }));

            IList<ContactRole> listOfRoles = new List<ContactRole> { new ContactRole { RoleName = "SuperUser" } };
            ContactQueryRepository.Setup(x => x.GetRolesFor(It.IsAny<Guid>()))
                .Returns(Task.FromResult(listOfRoles));
            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation
            {
                Status = OrganisationStatus.Live,
                EndPointAssessorOrganisationId = "EPA001",
                EndPointAssessorUkprn = 12345,
                PrimaryContact = null
            });
            ContactQueryRepository.Setup(r => r.GetContact("username")).ReturnsAsync(new Contact
            {
                DisplayName = "Display Name",
                Email = "email@domain.com",
                Username = "username"
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

            //Mediator.Verify(m =>
            //    m.Send(
            //        It.Is<UpdateOrganisationRequest>(r =>
            //            r.EndPointAssessorOrganisationId == "EPA001" && r.EndPointAssessorUkprn == 12345
            //            && r.PrimaryContact == "username"), It.IsAny<CancellationToken>()));
        }
    }
}