using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    [TestFixture]
    public class EpaOrganisationIdGeneratorTests
    {
        private Mock<IRegisterQueryRepository> _registerQueryRepository;
        private EpaOrganisationIdGenerator _generator;


        [SetUp]
        public void Setup()
        {
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();
             _generator = new EpaOrganisationIdGenerator(_registerQueryRepository.Object);   
        }

        [TestCase("EPA0001", "EPA0002")]
        [TestCase("EPA1", "EPA0002")]
        [TestCase("EPA11111", "EPA11112")]
        [TestCase("EPA11A11", "")]

        public void GetNewOrganisationIdFromCurrentMaximumOrganisationId(string currentId, string newId)
        {
            _registerQueryRepository.Setup(r => r.EpaOrganisationIdCurrentMaximum()).Returns(Task.FromResult(currentId));
           var returnedId = _generator.GetNextOrganisationId();
            Assert.AreEqual(returnedId, newId);
        }
    }
}
