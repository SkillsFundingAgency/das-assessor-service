﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Login;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{

    public class LoginHandlerTestsBase
    {
        protected LoginHandler Handler;
        protected Mock<IOrganisationQueryRepository> OrgQueryRepository;
        protected Mock<IContactQueryRepository> ContactQueryRepository;
        protected Mock<IContactRepository> ContactRepository;
        protected Mock<IRegisterRepository> RegisterRepository;

        [SetUp]
        public void Arrange()
        {
            OrgQueryRepository = new Mock<IOrganisationQueryRepository>();

            ContactQueryRepository = new Mock<IContactQueryRepository>();
            ContactQueryRepository.Setup(r => r.GetContactByGovIdentifier(It.IsNotIn(string.Empty)))
                .Returns(Task.FromResult(new Contact() {Id = It.IsAny<Guid>(), Status = ContactStatus.Live,
                    OrganisationId = It.IsAny<Guid>(), Username = "Test", Email = "test@email.com" }));
            ContactQueryRepository.Setup(r => r.GetContactByGovIdentifier(string.Empty))
                .Returns(Task.FromResult(new Contact() { Id = Guid.Empty, Status = ContactStatus.InvitePending,
                    OrganisationId = It.IsAny<Guid>(), Username = "Test", Email = "test@email.com" }));

            OrgQueryRepository.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync(new Organisation
            {
                EndPointAssessorName = "SomeName",
                EndPointAssessorUkprn = 12345,
                Id = It.IsAny<Guid>(),
                Status = OrganisationStatus.New
            });

            ContactRepository = new Mock<IContactRepository>();
            ContactRepository.Setup(x => x.UpdateUserName(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(default(object)));

            RegisterRepository = new Mock<IRegisterRepository>();
            RegisterRepository.Setup(m => m.UpdateEpaOrganisationPrimaryContact(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.FromResult(default(object)));

            Handler = new LoginHandler(new Mock<ILogger<LoginHandler>>().Object, 
                OrgQueryRepository.Object, ContactQueryRepository.Object, ContactRepository.Object, RegisterRepository.Object);
        }
    }
}