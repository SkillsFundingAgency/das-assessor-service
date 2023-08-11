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
        protected Mock<IRegisterQueryRepository> _registerQueryRepository;
        protected UpdateAparSummaryHandler _updateAparSummaryHandler;
        protected Mock<ILogger<UpdateAparSummaryHandler>> _logger;

        [SetUp]
        public void Setup()
        {
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();

            _logger = new Mock<ILogger<UpdateAparSummaryHandler>>();

            _registerQueryRepository.Setup(x => x.UpdateAparSummary()).ReturnsAsync(1);

            _updateAparSummaryHandler = new UpdateAparSummaryHandler(_registerQueryRepository.Object, _logger.Object);
        }

        [Test]
        public void AparSummaryUpdateRepoIsCalledWhenHandlerIsInvoked()
        {
            _updateAparSummaryHandler.Handle(new UpdateAparSummaryRequest(), CancellationToken.None).Wait();
            _registerQueryRepository.Verify(x => x.UpdateAparSummary());
        }
    }
}
