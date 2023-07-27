using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    public class RegisterQueryAparSummaryUpdateTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected AparSummaryUpdateHandler AparSummaryUpdateHandler;
        protected Mock<ILogger<AparSummaryUpdateHandler>> Logger;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            Logger = new Mock<ILogger<AparSummaryUpdateHandler>>();

            RegisterQueryRepository.Setup(x => x.AparSummaryUpdate()).ReturnsAsync(1);

            AparSummaryUpdateHandler = new AparSummaryUpdateHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            AparSummaryUpdateHandler.Handle().Wait();
            RegisterQueryRepository.Verify(x => x.AparSummaryUpdate());
        }
    }
}
