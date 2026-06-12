using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Handlers.Apply;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.GetApplicationHandlerTests
{
    [TestFixture]
    public class GetApplicationsHandlerTestsBase : MapperBase
    {
        protected Mock<IApplyRepository> ApplyRepository;
        protected GetApplicationsHandler Handler;

        [SetUp]
        public void Setup()
        {
            ApplyRepository = new Mock<IApplyRepository>();
            Handler = new GetApplicationsHandler(ApplyRepository.Object, Mapper);
        }
    }
}