using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Login;
using SFA.DAS.AssessorService.Application.Interfaces;
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
            Mediator = new Mock<IMediator>();
            Handler = new LoginHandler(new Mock<ILogger<LoginHandler>>().Object, config,
                OrgQueryRepository.Object, ContactQueryRepository.Object,
                Mediator.Object);
        }
    }
}