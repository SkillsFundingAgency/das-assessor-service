using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register
{
    [TestFixture]
    public class EpaOrganisationValidatorTests
    {
        private Mock<IRegisterValidationRepository> _registerRepository;
        private EpaOrganisationValidator _validator;
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizer;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IRegisterQueryRepository> _registerQueryRepository;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterValidationRepository>();
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _validator = new EpaOrganisationValidator(_registerRepository.Object, _registerQueryRepository.Object,
                _cleanserService.Object, _localizer.Object);

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
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.AnotherOrganisationUsingTheUkprn,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.UkprnIsInvalid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.UkprnIsInvalid, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.StandardNotFound])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.StandardNotFound, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent])
                .Returns(new LocalizedString(
                    EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.EmailIsMissing])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.ContactIdDoesntExist])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.ContactIdDoesntExist, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.DisplayNameIsMissing])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.DisplayNameIsMissing,
                    "DisplayNameIsMissing"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.DisplayNameTooShort])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.DisplayNameTooShort,
                    "DisplayNameTooShort"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.EmailIsMissing])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.EmailIsMissing, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.EmailAlreadyPresentInAnotherOrganisation])
                .Returns(new LocalizedString(
                    EpaOrganisationValidatorMessageName.EmailAlreadyPresentInAnotherOrganisation, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.ContactIdIsRequired])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.ContactIdIsRequired, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.NoDeliveryAreasPresent])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.NoDeliveryAreasPresent, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.DeliveryAreaNotValid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.DeliveryAreaNotValid, "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationCompanyNumberNotValid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationCompanyNumberNotValid,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationCharityNumberNotValid])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationCharityNumberNotValid,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationCompanyNumberAlreadyUsed])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationCompanyNumberAlreadyUsed,
                    "fail"));
            _localizer.Setup(l => l[EpaOrganisationValidatorMessageName.OrganisationCharityNumberAlreadyUsed])
                .Returns(new LocalizedString(EpaOrganisationValidatorMessageName.OrganisationCharityNumberAlreadyUsed,
                    "fail"));
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
        
        [TestCase("name", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void CheckEmailIsMissingReturnsExpectedMessage(string name, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckIfEmailIsMissing(name).Length == 0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase("", true)]
        [TestCase("01234567", true)]
        [TestCase("RC123456", true)]
        [TestCase("1234567", false)]
        [TestCase("ABC12345", false)]
        public void CheckCompanyNumberIsValid(string companyNumber, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckCompanyNumberIsValid(companyNumber).Length == 0;
            Assert.AreEqual(isAcceptable, noMessageReturned);
        }

        [TestCase("ORG-1", "12345678")]
        public void CheckCompanyNumberIsRejectedIfValidButAlreadyInUse(string organisationId, string companyNumber)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCompanyNumber(organisationId, companyNumber))
                .Returns(Task.FromResult<bool>(true));

            string validationErrorMessage = _validator.CheckIfOrganisationCompanyNumberExists(organisationId, companyNumber);

            Assert.IsNotEmpty(validationErrorMessage);
        }

        [TestCase("ORG-1", "12345678")]
        public void CheckCompanyNumberIsAcceptedIfValidAndNotAlreadyInUse(string organisationId, string companyNumber)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCompanyNumber(organisationId, companyNumber))
                .Returns(Task.FromResult<bool>(false));

            string validationErrorMessage = _validator.CheckIfOrganisationCompanyNumberExists(organisationId, companyNumber);

            Assert.IsEmpty(validationErrorMessage);
        }

        [TestCase("ORG-1", "12345678")]
        public void CheckCharityNumberIsRejectedIfValidButAlreadyInUse(string organisationId, string charityNumber)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCharityNumber(organisationId, charityNumber))
                .Returns(Task.FromResult<bool>(true));

            string validationErrorMessage = _validator.CheckIfOrganisationCharityNumberExists(organisationId, charityNumber);

            Assert.IsNotEmpty(validationErrorMessage);
        }

        [TestCase("ORG-1", "12345678")]
        public void CheckCharityNumberIsAcceptedIfValidAndNotAlreadyInUse(string organisationId, string charityNumber)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCharityNumber(organisationId, charityNumber))
                .Returns(Task.FromResult<bool>(false));

            string validationErrorMessage = _validator.CheckIfOrganisationCharityNumberExists(organisationId, charityNumber);

            Assert.IsEmpty(validationErrorMessage);
        }

        [TestCase("", true)]
        [TestCase("01234567", true)]
        [TestCase("1234567", true)]
        [TestCase("A123456", false)]
        [TestCase("ABC12345", false)]
        public void CheckCharityNumberIsValid(string charityNumber, bool isAcceptable)
        {
            var noMessageReturned = _validator.CheckCharityNumberIsValid(charityNumber).Length == 0;
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
        
        [TestCase("name", true, "")]
        [TestCase("    ", false, "DisplayNameIsMissing; ")]
        [TestCase("", false, "DisplayNameIsMissing; ")]
        [TestCase(null, false,"DisplayNameIsMissing; ")]
        [TestCase("a", false,  "DisplayNameTooShort; ")]
        [TestCase("  a  ", false,  "DisplayNameTooShort; ")]
        public void CheckDisplayNameReturnsExpectedMessage(string name, bool isAcceptable, string messageExpected)
        {
            var messageReturned = _validator.CheckDisplayName(name);
            Assert.AreEqual(messageExpected, messageReturned);
            
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationIdAlreadyUsedReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationAlreadyUsingName(It.IsAny<string>(),string.Empty))
                .Returns(Task.FromResult(alreadyPresent));
            var noMessageReturned = _validator.CheckOrganisationNameNotUsed("id here").Length > 0;
            Assert.AreEqual(noMessageReturned, alreadyPresent);
        }
        
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfOrganisationIdAlreadyUsedByOtherOrganisationsReturnExpectedMessage(bool alreadyPresent, bool messageShown)
        {
            _registerRepository.Setup(r => r.EpaOrganisationAlreadyUsingName(It.IsAny<string>(),It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyPresent));
            var noMessageReturned = _validator.CheckOrganisationNameNotUsedForOtherOrganisations("id here","other org Id").Length > 0;
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
        
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfContactExistsReturnExpectedMessage(bool alreadyPresent, bool messageShown)
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


        [TestCase("test@test.com",true)]
        [TestCase("test@test.co.uk", true)]
        [TestCase("test.tester@test.com", true)]
        [TestCase("test.tester@digitaleducation.gov.uk", true)]
        [TestCase("test.terser@test.co.uk", true)]
        [TestCase("testtest",false)]
        [TestCase("testtest@com", false)]
        [TestCase("testtest@test..com", false)]
        [TestCase("testtest@", false)]
        [TestCase("@testtest", false)]
        [TestCase("n/a", false)]
        [TestCase("test test", false)]
        [TestCase("testtest", false)]
        [TestCase("",false)]
        [TestCase("firstname-lastname@domain-one.co.in", true)]
        [TestCase("firstname-lastname@domain-one.com", true)]
        [TestCase("firstname-lastname@domain-one.nz", true)]
        public void CheckIfEmailIsAcceptableFormat(string email, bool isValidExpected)
        {
            var isValidReturned =
                _validator.CheckIfEmailIsPresentAndInSuitableFormat(email).Length == 0;
            Assert.AreEqual(isValidExpected, isValidReturned);
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


        [TestCase("")]
        [TestCase(null)]
        public void CheckIfOrganisationNotFoundReturnsAnErrorMessageWhenEmpty(string organisationId)
        {
            var isMessageReturned =
                _validator.CheckIfOrganisationNotFound(null).Length > 0;
            Assert.AreEqual(isMessageReturned, true);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
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
        
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfEmailAlreadyExistsAgainstOtherOrganisationReturnsAnErrorMessage(bool exists, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.EmailAlreadyPresentInAnotherOrganisation(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(exists));
            var isMessageReturned =
                _validator.CheckIfEmailAlreadyPresentInAnotherOrganisation("email", "orgId").Length > 0;
            Assert.AreEqual(noMessageReturned, exists);
            _registerRepository.Verify(r => r.EmailAlreadyPresentInAnotherOrganisation(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        
        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckIfEmailAlreadyExistsAgainstOtherOrganisationNotAssociatedWithContactIdReturnsAnErrorMessage(bool exists, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.EmailAlreadyPresentInAnOrganisationNotAssociatedWithContact(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(exists));
            var isMessageReturned =
                _validator.CheckIfEmailAlreadyPresentInOrganisationNotAssociatedWithContact("email", Guid.NewGuid().ToString()).Length > 0;
            Assert.AreEqual(noMessageReturned, exists);
            _registerRepository.Verify(r => r.EmailAlreadyPresentInAnOrganisationNotAssociatedWithContact(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }
        

        [TestCase("", false, false)]
        [TestCase(null,false,false)]
        [TestCase("02741b43-acfd-45c6-a9d6-780ebe5df65c", true, true)]
        public void CheckIfContactIdExistsReturnsAnErrorMessage(string contactId, bool repositoryCheckResult, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.ContactExists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(repositoryCheckResult));
            var isMessageReturned =
                _validator.CheckContactIdExists(contactId).Length > 0;
            Assert.AreEqual(noMessageReturned, !isMessageReturned);
            if (repositoryCheckResult == false)
                _registerRepository.Verify(r => r.ContactExists(It.IsAny<Guid>()), Times.Never);
            else
                _registerRepository.Verify(r => r.ContactExists(It.IsAny<Guid>()), Times.Once);
        }
        
        
        [TestCase("", "",false, false)]
        [TestCase("wrong contact id", "valid org id",false, false)]
        [TestCase(null,"",false,false)]
        [TestCase("3151f01c-ba75-4123-965e-ff1e5f128514", "valid org Id", true, true)]
        public void CheckIfOrganisationStandardHasValidContactIdReturnsAnErrorMessage(string contactId, string organisationId, bool repositoryCheckResult, bool noMessageReturned)
        {
            _registerRepository.Setup(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(repositoryCheckResult));
            var isMessageReturned =
                _validator.CheckIfContactIdIsValid(contactId, organisationId).Length > 0;
            Assert.AreEqual(noMessageReturned, !isMessageReturned);
            if (repositoryCheckResult == false)
                _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            else
                _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CheckOrganisationRequestValidationWhenThereAreNoIssues()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test", Ukprn = null, OrganisationTypeId = 9
            };
            
            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(true));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);
            
            Assert.AreEqual(0, result.Errors.Count);
        }
        
        
        
        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsInvalidOrganisationTypeId()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test", Ukprn = null, OrganisationTypeId = 9
            };
            
            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);
            
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("OrganisationTypeId", result.Errors[0].Field);
        }
        
        
        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsInvalidOrganisationTypeIdAndNoName()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "", Ukprn = null, OrganisationTypeId = 9
            };
            
            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);
            
            Assert.AreEqual(2, result.Errors.Count(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()));
            Assert.AreEqual(1,result.Errors.Count(x => x.Field == "OrganisationTypeId"));
            Assert.AreEqual(1,result.Errors.Count(x => x.Field == "Name"));
        }
        
        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsInvalidOrganisationTypeIdAndPresentUkprn()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test", Ukprn = 12345678, OrganisationTypeId = 9
            };
            
            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(true));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);
            
            Assert.AreEqual(1, result.Errors.Count(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()));
            Assert.AreEqual(1, result.Errors.Count(x => x.StatusCode == ValidationStatusCode.AlreadyExists.ToString()));
            Assert.AreEqual(1,result.Errors.Count(x => x.Field == "OrganisationTypeId"));
            Assert.AreEqual(1,result.Errors.Count(x => x.Field == "Ukprn"));
        }
    }
}