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
            var noMessageReturned = _validator.CheckIfOrganisationIdExists("id here").Length>0;
            Assert.AreEqual(noMessageReturned, alreadyPresent);
        }

        [Test]
        public void CheckIfOrganisationIdAlreadyUsedIgnoresNullOrganisationId()
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            var noMessageReturned = _validator.CheckIfOrganisationIdExists(null).Length > 0;
            Assert.AreEqual(noMessageReturned, false);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfUkprnAlreadyUsedReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(alreadyPresent));
            var messageReturned = _validator.CheckIfOrganisationUkprnExists(1234).Length > 0;
            Assert.AreEqual(messageReturned, alreadyPresent);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void CheckIfObjectTypeInvalidReturnsExpectedMessage(bool exists, bool messageShown)
        {
            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(exists));
            var noMessageReturned = _validator.CheckOrganisationTypeIsNullOrExists(1234).Length == 0;
            Assert.AreEqual(noMessageReturned, exists);
        }

        [Test]
        public void CheckIfUkprnAlreadyUsedIgnoresNullUkprn()
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(false));
            var messageReturned = _validator.CheckIfOrganisationUkprnExists(null).Length > 0;
            Assert.AreEqual(messageReturned, false);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()), Times.Never);
        }


        [Test]
        public void CheckIfOrganisationUkprnExistsForOtherOrganisationsWhenUkprnIsNull()
        {
            var isMessageReturned =
                _validator.CheckIfOrganisationUkprnExistsForOtherOrganisations(null, "123445").Length > 0;
            Assert.IsFalse(isMessageReturned);
            _registerRepository.Verify(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CheckIfOrganisationUkprnExistsForOtherOrganisationsWhenUkprnIsNotUsedElsewhere()
        {
            _registerRepository.Setup(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            var isMessageReturned =
                _validator.CheckIfOrganisationUkprnExistsForOtherOrganisations(123456, "123445").Length > 0;
            Assert.IsFalse(isMessageReturned);
            _registerRepository.Verify(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CheckIfOrganisationUkprnExistsForOtherOrganisationsWhenUkprnIsUsedElsewhere()
        {
            _registerRepository.Setup(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));
            var isMessageReturned =
                _validator.CheckIfOrganisationUkprnExistsForOtherOrganisations(123456, "123445").Length > 0;
            Assert.IsTrue(isMessageReturned);
            _registerRepository.Verify(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationNotFoundReturnsAnErrorMessage(bool exists, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(exists));
            var isMessageReturned =
                _validator.CheckIfOrganisationNotFound("123445").Length > 0;
            Assert.AreEqual(noMessageReturned, exists);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Once);
        }

    }
}
