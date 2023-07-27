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
    public class RegisterQueryAparSummaryUpdateTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetAparSummaryUpdateRequestHandler AparSummaryUpdateHandler;
        protected Mock<ILogger<GetAparSummaryUpdateRequestHandler>> Logger;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();

            Logger = new Mock<ILogger<GetAparSummaryUpdateRequestHandler>>();

            RegisterQueryRepository.Setup(x => x.AparSummaryUpdate()).ReturnsAsync(1);

            AparSummaryUpdateHandler = new GetAparSummaryUpdateRequestHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            AparSummaryUpdateHandler.Handle(new GetAparSummaryUpdateRequest(), CancellationToken.None).Wait();
            RegisterQueryRepository.Verify(x => x.AparSummaryUpdate());
        }
    }
}
