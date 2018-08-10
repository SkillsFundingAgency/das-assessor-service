using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register
{
    [TestFixture]
    public class EpaOrganisationValidatorTests
    {
        private Mock<IRegisterRepository> _registerRepository;
        private EpaOrganisationValidator _validator;
        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterRepository>();
            _validator = new EpaOrganisationValidator(_registerRepository.Object);
        }

        [TestCase("EPA000",true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("ThirteenChars", false)]
        [TestCase("Twelve_chars", true)]
        public void CheckOrganisationIdReturnsExpectedMessage(string organisationId, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckOrganisationId(organisationId).Length==0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase("name", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void CheckOrganisationNameReturnsExpectedMessage(string name, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckOrganisationName(name).Length==0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationIdAlreadyUsedReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyPresent));
            var noMessageReturned = _validator.CheckIfOrganisationIdExists("id here").Result.Length>0;
            Assert.AreEqual(noMessageReturned, alreadyPresent);
        }

        [Test]
        public void CheckIfOrganisationIdAlreadyUsedIgnoresNullOrganisationId()
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            var noMessageReturned = _validator.CheckIfOrganisationIdExists(null).Result.Length > 0;
            Assert.AreEqual(noMessageReturned, false);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfUkprnAlreadyUsedReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(alreadyPresent));
            var noMessageReturned = _validator.CheckIfOrganisationUkprnExists(1234).Result.Length > 0;
            Assert.AreEqual(noMessageReturned, alreadyPresent);
        }

        [Test]
        public void CheckIfUkprnAlreadyUsedIgnoresNullUkprn()
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(false));
            var noMessageReturned = _validator.CheckIfOrganisationUkprnExists(null).Result.Length > 0;
            Assert.AreEqual(noMessageReturned, false);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()), Times.Never);
        }
    }
}
