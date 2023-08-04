using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    public class RegisterQuesryAparSummaryLastUpdatedTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetAparSummaryLastUpdatedHandler AparSummaryLastUpdatedHandler;
        protected Mock<ILogger<GetAparSummaryLastUpdatedHandler>> Logger;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            Logger = new Mock<ILogger<GetAparSummaryLastUpdatedHandler>>();

            RegisterQueryRepository.Setup(x => x.GetAparSummaryLastUpdated()).ReturnsAsync(DateTime.UtcNow);

            AparSummaryLastUpdatedHandler = new GetAparSummaryLastUpdatedHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            AparSummaryLastUpdatedHandler.Handle(new GetAparSummaryLastUpdatedRequest(), CancellationToken.None).Wait();
            RegisterQueryRepository.Verify(x => x.GetAparSummaryLastUpdated());
        }
    }
}
