using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Login;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{

    public class LoginHandlerTestsBase
    {
        protected LoginHandler Handler;
        protected Mock<IOrganisationQueryRepository> OrgQueryRepository;
        protected Mock<IContactQueryRepository> ContactQueryRepository;
        protected Mock<IMediator> Mediator;

        [SetUp]
        public void Arrange()
        {
            var config = new WebConfiguration() {Authentication = new AuthSettings() {Role = "EPA"}};


            OrgQueryRepository = new Mock<IOrganisationQueryRepository>();

            ContactQueryRepository = new Mock<IContactQueryRepository>();
            var roles = new List<ContactRole>
            {
                new ContactRole
                {
                    RoleName = "SuperUser"
                }
            };
            ContactQueryRepository.Setup(r => r.GetBySignInId(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Contact() {Id = It.IsAny<Guid>(), OrganisationId = It.IsAny<Guid>()}));

            ContactQueryRepository.Setup(r => r.GetRolesFor(It.IsAny<Guid>())).ReturnsAsync(roles);
            OrgQueryRepository.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(new Organisation
            {
                EndPointAssessorName = "SomeName",
                EndPointAssessorUkprn = 12345,
                Id = It.IsAny<Guid>(),
                Status = OrganisationStatus.New
            });
            Mediator = new Mock<IMediator>();
            Handler = new LoginHandler(new Mock<ILogger<LoginHandler>>().Object, config,
                OrgQueryRepository.Object, ContactQueryRepository.Object,
                Mediator.Object);
        }
    }
}