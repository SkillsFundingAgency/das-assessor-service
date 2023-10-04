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
        protected Mock<IRegisterQueryRepository> _registerQueryRepository;
        protected AparSummaryUpdateHandler _aparSummaryUpdateHandler;
        protected Mock<ILogger<AparSummaryUpdateHandler>> _logger;

        [SetUp]
        public void Setup()
        {
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();

            _logger = new Mock<ILogger<AparSummaryUpdateHandler>>();

            _registerQueryRepository.Setup(x => x.AparSummaryUpdate()).ReturnsAsync(1);

            _aparSummaryUpdateHandler = new AparSummaryUpdateHandler(_registerQueryRepository.Object, _logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            _aparSummaryUpdateHandler.Handle(new AparSummaryUpdateRequest(), CancellationToken.None).Wait();
            _registerQueryRepository.Verify(x => x.AparSummaryUpdate());
        }
    }
}
