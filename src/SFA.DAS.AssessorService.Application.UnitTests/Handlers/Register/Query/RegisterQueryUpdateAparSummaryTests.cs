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
    public class RegisterQueryUpdateAparSummaryTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected UpdateAparSummaryHandler AparSummaryUpdateHandler;
        protected Mock<ILogger<UpdateAparSummaryHandler>> Logger;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            Logger = new Mock<ILogger<UpdateAparSummaryHandler>>();

            RegisterQueryRepository.Setup(x => x.UpdateAparSummary()).ReturnsAsync(1);

            AparSummaryUpdateHandler = new UpdateAparSummaryHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            AparSummaryUpdateHandler.Handle(new UpdateAparSummaryRequest(), CancellationToken.None).Wait();
            RegisterQueryRepository.Verify(x => x.UpdateAparSummary());
        }
    }
}
