using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register
{
    [TestFixture]
    public class EpaOrganisationValidatorTests
    {
        private Mock<IRegisterQueryRepository> _registerRepository;
        private EpaOrganisationValidator _validator;
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizer;
        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterQueryRepository>();
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _validator = new EpaOrganisationValidator(_registerRepository.Object, _localizer.Object);

            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationTypeIsInvalid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationTypeIsInvalid, "fail"));          
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationIdAlreadyUsed])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationIdAlreadyUsed, "fail"));          
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.NoOrganisationId])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.NoOrganisationId, "fail"));           
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationNotFound])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationNotFound, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationIdTooLong])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationIdTooLong, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationNameEmpty])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationNameEmpty, "fail"));         
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.UkprnAlreadyUsed])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.UkprnAlreadyUsed, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.AnotherOrganisationUsingTheUkprn])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.AnotherOrganisationUsingTheUkprn, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.UkprnIsInvalid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.UkprnIsInvalid, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.StandardNotFound])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.StandardNotFound, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist, "fail")); 
        }

        [TestCase("EPA000", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("ThirteenChars", false)]
        [TestCase("Twelve_chars", true)]
        public void CheckOrganisationIdIsPresentAndValidReturnsExpectedMessage(string organisationId, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckOrganisationIdIsPresentAndValid(organisationId).Length == 0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase("name", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void CheckOrganisationNameReturnsExpectedMessage(string name, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckOrganisationName(name).Length == 0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase(null, true)]
        [TestCase(10000000, true)]
        [TestCase(99999999, true)]
        [TestCase(9999999, false)]
        [TestCase(100000000, false)]
        public void CheckUkprnIsValid(long? ukprn, bool isValid)
        {
            var noMessageReturned = _validator.CheckUkprnIsValid(ukprn).Length == 0;
            Assert.AreEqual(isValid, noMessageReturned);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationIdAlreadyUsedReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyPresent));
            var noMessageReturned = _validator.CheckIfOrganisationAlreadyExists("id here").Length > 0;
            Assert.AreEqual(noMessageReturned, alreadyPresent);
        }

        [Test]
        public void CheckIfOrganisationIdAlreadyUsedIgnoresNullOrganisationId()
        {
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            var noMessageReturned = _validator.CheckIfOrganisationAlreadyExists(null).Length > 0;
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


        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationStandardAlreadyExistsReturnsAnErrorMessage(bool exists, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.EpaOrganisationStandardExists(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(exists));
            var isMessageReturned =
                _validator.CheckIfOrganisationStandardAlreadyExists("orgId", 5).Length > 0;
            Assert.AreEqual(noMessageReturned, exists);
            _registerRepository.Verify(r => r.EpaOrganisationStandardExists(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }


        [TestCase("", "",false, true)]
        [TestCase(null,"",false,true)]
        [TestCase("valid contact id", "valid org Id", true, true)]
        public void CheckIfOrganisationStandardHasValidContactIdReturnsAnErrorMessage(string contactId, string organisationId, bool repositoryCheckResult, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.ContactIdIsValidForOrganisationId(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(repositoryCheckResult));
            var isMessageReturned =
                _validator.CheckIfContactIdIsEmptyOrValid(contactId, organisationId).Length > 0;
            Assert.AreEqual(noMessageReturned, !isMessageReturned);
            if (repositoryCheckResult == false)
                _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            else
            _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}