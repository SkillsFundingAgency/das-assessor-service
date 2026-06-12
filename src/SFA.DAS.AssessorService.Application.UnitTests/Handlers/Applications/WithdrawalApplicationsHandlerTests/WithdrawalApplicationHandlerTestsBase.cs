using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Apply.Review;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.WithdrawalApplicationsHandlerTests
{
    [TestFixture]
    public class WithdrawalApplicationHandlerTestsBase : MapperBase
    {
        protected Mock<IApplyRepository> ApplyRepository;
        protected Mock<ILogger<WithdrawalApplicationsHandler>> Logger;
        protected WithdrawalApplicationsHandler Handler;

        [SetUp]
        public void Setup()
        {
            ApplyRepository = new Mock<IApplyRepository>();
                    
            Logger = new Mock<ILogger<WithdrawalApplicationsHandler>>();
            
            Handler = new WithdrawalApplicationsHandler(ApplyRepository.Object, Logger.Object, Mapper);
        }
    }
}
