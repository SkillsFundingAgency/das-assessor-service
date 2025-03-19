using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;

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
        private Mock<IStandardService> _standardService;

        [SetUp]
        public void Setup()
        {
            _registerRepository = new Mock<IRegisterValidationRepository>();
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();
            
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _cleanserService
                .Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _standardService = new Mock<IStandardService>();
            
            _localizer = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            var messageNameType = typeof(EpaOrganisationValidatorMessageName);
            foreach (var field in messageNameType.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                var value = field.GetValue(null) as string;
                _localizer
                    .Setup(l => l[value])
                    .Returns(new LocalizedString(value, "fail"));
            }

            _validator = new EpaOrganisationValidator(_registerRepository.Object, _registerQueryRepository.Object,
                _cleanserService.Object, _localizer.Object, _standardService.Object);
        }

        [TestCase("EPA000", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("ThirteenChars", false)]
        [TestCase("Twelve_chars", true)]
        public void CheckOrganisationIdIsPresentAndValidReturnsExpectedMessage(string organisationId, bool validExpected)
        {
            var valid = _validator.CheckOrganisationIdIsPresentAndValid(organisationId).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("name", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void CheckOrganisationNameIsPresentAndValidReturnsExpectedMessage(string name, bool validExpected)
        {
            var valid = _validator.CheckOrganisationNameIsPresentAndValid(name).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("", true)]
        [TestCase("01234567", true)]
        [TestCase("RC123456", true)]
        [TestCase("rc123456", true)]
        [TestCase("1234567", false)]
        [TestCase("ABC12345", false)]
        [TestCase("1000$!&*^%", false)]
        [TestCase("!£$%^&*()", false)]
        public void CheckCompanyNumberIsMissingOrValid(string companyNumber, bool validExpected)
        {
            var valid = _validator.CheckCompanyNumberIsMissingOrValid(companyNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("ORG-1", "12345678", false)]
        public void CheckCompanyNumberIsRejectedIfValidButAlreadyInUse(string organisationId, string companyNumber, bool validExpected)
        {
            _registerRepository
                .Setup(x => x.EpaOrganisationExistsWithCompanyNumber(organisationId, companyNumber))
                .Returns(Task.FromResult<bool>(true));

            var valid = _validator.CheckIfOrganisationCompanyNumberExists(organisationId, companyNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("ORG-1", "12345678", true, false)]
        [TestCase("ORG-1", "12345678", false, true)]
        public void CheckCompanyNumberIsAcceptedIfValidAndNotAlreadyInUse(string organisationId, string companyNumber, bool exists, bool validExpected)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCompanyNumber(organisationId, companyNumber))
                .Returns(Task.FromResult<bool>(exists));

            var valid = _validator.CheckIfOrganisationCompanyNumberExists(organisationId, companyNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("ORG-1", "12345678", false)]
        public void CheckCharityNumberIsRejectedIfValidButAlreadyInUse(string organisationId, string charityNumber, bool validExpected)
        {
            _registerRepository.Setup(x => x.EpaOrganisationExistsWithCharityNumber(organisationId, charityNumber))
                .Returns(Task.FromResult<bool>(true));

            var valid = _validator.CheckIfOrganisationCharityNumberExists(organisationId, charityNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("ORG-1", "12345678", false, true)]
        [TestCase("ORG-1", "12345678", true, false)]
        public void CheckCharityNumberIsAcceptedIfValidAndNotAlreadyInUse(string organisationId, string charityNumber, bool exists, bool validExpected)
        {
            _registerRepository
                .Setup(x => x.EpaOrganisationExistsWithCharityNumber(organisationId, charityNumber))
                .Returns(Task.FromResult<bool>(exists));

            var valid = _validator.CheckIfOrganisationCharityNumberExists(organisationId, charityNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase("", true)]
        [TestCase("01234567", true)]
        [TestCase("1234567", true)]
        [TestCase("A123456", true)]
        [TestCase("ABC12345", true)]
        [TestCase("12345679-1", true)]
        [TestCase("1000$!&*^%", false)]
        [TestCase("!£$%^&*()", false)]
        [TestCase("010101888-1££££''''", false)]
        public void CheckCharityNumberIsValid(string charityNumber, bool validExpected)
        {
            var valid = _validator.CheckCharityNumberIsValid(charityNumber).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase(null, true)]
        [TestCase(10000000, true)]
        [TestCase(99999999, true)]
        [TestCase(9999999, false)]
        [TestCase(100000000, false)]
        public void CheckUkprnIsValid(long? ukprn, bool validExpected)
        {
            var valid = _validator.CheckUkprnIsValid(ukprn).Length == 0;
            valid.Should().Be(validExpected);
        }
        
        [TestCase("name", true)]
        [TestCase("    ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("a", false)]
        [TestCase("  a  ", false)]
        public void CheckDisplayNameIsPresentAndValidReturnsExpectedMessage(string name, bool validExpected)
        {
            var valid = _validator.CheckDisplayNameIsPresentAndValid(name).Length == 0;
            valid.Should().Be(validExpected);

        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckOrganisationNameNotExistsReturnExpectedMessage(bool alreadyUsing, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationNameExists(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyUsing));
            
            var valid = _validator.CheckOrganisationNameNotExists("id here").Length == 0;
            valid.Should().Be(validExpected);
        }
        
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckOrganisationNameNotExistsForOtherOrganisationsReturnExpectedMessage(bool alreadyUsing, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationNameExists(It.IsAny<string>(),It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyUsing));

            var valid = _validator.CheckOrganisationNameNotExistsExcludingOrganisation("id here","other org Id").Length == 0;
            valid.Should().Be(validExpected);
        }

        [Test]
        public void CheckIfOrganisationIdAlreadyUsedIgnoresNullOrganisationId()
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            var valid = _validator.CheckIfOrganisationAlreadyExists(null).Length == 0;
            valid.Should().Be(true);

            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckIfUkprnAlreadyUsedReturnExpectedMessage(bool alreadyUsed, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(alreadyUsed));

            var valid = _validator.CheckIfOrganisationUkprnExists(1234).Length == 0;
            valid.Should().Be(validExpected);
        }
        
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckIfUkprnExistsReturnExpectedMessage(bool contactExists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(contactExists));
            
            var valid = _validator.CheckIfOrganisationUkprnExists(1234).Length == 0;
            valid.Should().Be(validExpected);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void CheckIfOrganisationTypeExistsReturnsExpectedMessage(bool organisationTypeExists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(organisationTypeExists));

            var valid = _validator.CheckOrganisationTypeIsNullOrExists(1234).Length == 0;
            valid.Should().Be(validExpected);
        }

        [Test]
        public void CheckIfUkprnAlreadyUsedIgnoresNullUkprn()
        {
            var valid = _validator.CheckIfOrganisationUkprnExists(null).Length == 0;
            
            valid.Should().BeTrue();
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()), Times.Never);
        }


        [Test]
        public void CheckIfOrganisationUkprnExistsForOtherOrganisationsWhenUkprnIsNull()
        {
            var valid = _validator.CheckIfOrganisationUkprnExistsForOtherOrganisations(null, "123445").Length == 0;
            
            valid.Should().BeTrue();
            _registerRepository.Verify(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }


        [TestCase("test@test.com", true)]
        [TestCase("test@test.co.uk", true)]
        [TestCase("test.tester@test.com", true)]
        [TestCase("test.tester@digitaleducation.gov.uk", true)]
        [TestCase("test.terser@test.co.uk", true)]
        [TestCase("testtest", false)]
        [TestCase("testtest@com", false)]
        [TestCase("testtest@test..com", false)]
        [TestCase("testtest@", false)]
        [TestCase("@testtest", false)]
        [TestCase("n/a", false)]
        [TestCase("test test", false)]
        [TestCase("testtest", false)]
        [TestCase("", false)]
        [TestCase("firstname-lastname@domain-one.co.in", true)]
        [TestCase("firstname-lastname@domain-one.com", true)]
        [TestCase("firstname-lastname@domain-one.nz", true)]
        public void CheckIfEmailIsAcceptableFormat(string email, bool validExpected)
        {
            var valid =  _validator.CheckEmailPresentAndValid(email).Length == 0;

            valid.Should().Be(validExpected);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void CheckIfOrganisationUkprnExistsForOtherOrganisationsWhenUkprnIsAlreadyUsed(bool alreadyUsed, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()))
                .Returns(Task.FromResult(alreadyUsed));

            var valid =
                _validator.CheckIfOrganisationUkprnExistsForOtherOrganisations(123456, "123445").Length == 0;
            
            valid.Should().Be(validExpected);
            _registerRepository.Verify(r => r.EpaOrganisationAlreadyUsingUkprn(It.IsAny<long>(), It.IsAny<string>()), Times.Once);
        }

        [TestCase(null, false, false, false)]
        [TestCase("valid-organsation-id", true, true, true)]
        [TestCase("valid-organsation-id", false, false, true)]
        public void CheckOrganisationExistsReturnsAnErrorMessage(string organisationId, bool exists, bool validExpected, bool repositoryShouldBeCalled)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()))
                .Returns(Task.FromResult(exists));

            var valid = _validator.CheckOrganisationExists(organisationId).Length == 0;
            
            valid.Should().Be(validExpected);
            if(repositoryShouldBeCalled)
                _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Once);
            else
                _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
        }


        [Test]
        public void CheckOrganisationExistsReturnsAnErrorMessageWhenNull()
        {
            var valid = _validator.CheckOrganisationExists(null).Length == 0;
            
            valid.Should().BeFalse();
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithOrganisationId(It.IsAny<string>()), Times.Never);
        }


        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckIfOrganisationStandardAlreadyExistsReturnsAnErrorMessage(bool exists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationStandardExists(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(exists));

            var valid = _validator.CheckIfOrganisationStandardAlreadyExists("orgId", 5).Length == 0;
            
            valid.Should().Be(validExpected);
            _registerRepository.Verify(r => r.EpaOrganisationStandardExists(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }
        
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckEmailNotExistsExcludeContactReturnsAnErrorMessage(bool emailExists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EmailExistsExcludeContact(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(emailExists));

            var valid =
                _validator.CheckEmailNotExistsExcludingContact("email", Guid.NewGuid().ToString()).Length == 0;
            
            valid.Should().Be(validExpected);
            _registerRepository.Verify(r => r.EmailExistsExcludeContact(It.IsAny<string>(), It.IsAny<Guid>()), Times.Once);
        }
        

        [TestCase("", false, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase("02741b43-acfd-45c6-a9d6-780ebe5df65c", false, false, true)]
        [TestCase("02741b43-acfd-45c6-a9d6-780ebe5df65c", true, true, true)]
        public void CheckIfContactIdExistsReturnsAnErrorMessage(string contactId, bool contactExists, bool validExpected, bool repositoryShouldBeCalled)
        {
            _registerRepository
                .Setup(r => r.ContactExists(It.IsAny<Guid>()))
                .Returns(Task.FromResult(contactExists));
            
            var valid = _validator.CheckContactIdExists(contactId).Length == 0;
            
            valid.Should().Be(validExpected);

            if (repositoryShouldBeCalled)
                _registerRepository.Verify(r => r.ContactExists(It.IsAny<Guid>()), Times.Once);
            else
                _registerRepository.Verify(r => r.ContactExists(It.IsAny<Guid>()), Times.Never);
        }
        
        
        [TestCase("", "", false, false, false)]
        [TestCase("invalid-contact-id", "valid-contact-id", false, false, false)]
        [TestCase(null, "valid-contact-id", false, false, false)]
        [TestCase("3151f01c-ba75-4123-965e-ff1e5f128514", "valid-contact-id", true, true, true)]
        public void CheckIfOrganisationStandardHasValidContactIdReturnsAnErrorMessage(string contactId, string organisationId, bool contactExistsForOrganisation, bool validExpected, bool repositoryShouldBeCalled)
        {
            _registerRepository
                .Setup(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.FromResult(contactExistsForOrganisation));

            var valid = _validator.CheckIfContactIdIsValid(contactId, organisationId).Length ==0;
            
            valid.Should().Be(validExpected);
            if (repositoryShouldBeCalled)
                _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
            else
                _registerRepository.Verify(r => r.ContactIdIsValidForOrganisationId(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void CheckOrganisationRequestValidationWhenThereAreNoIssues()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test", Ukprn = null, OrganisationTypeId = 9
            };
            
            _registerRepository
                .Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);
            
            result.Errors.Count.Should().Be(0);
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
            
            result.Errors.Count.Should().Be(1);
            result.Errors[0].Field.Should().Be("OrganisationTypeId");
        }

        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsNoOrganisationTypeId()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test",
                Ukprn = null,
                OrganisationTypeId = null
            };

            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].Field.Should().Be("OrganisationTypeId");
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
            
            result.Errors.Count(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()).Should().Be(2);
            result.Errors.Count(x => x.Field == "OrganisationTypeId").Should().Be(1);
            result.Errors.Count(x => x.Field == "Name").Should().Be(1);
        }

        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsNoOrganisationTypeIdAndNoName()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "",
                Ukprn = null,
                OrganisationTypeId = null
            };

            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);

            result.Errors.Count(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()).Should().Be(2);
            result.Errors.Count(x => x.Field == "OrganisationTypeId").Should().Be(1);
            result.Errors.Count(x => x.Field == "Name").Should().Be(1);
        }

        [Test]
        public void CheckOrganisationRequestValidationWhenThereIsInvalidOrganisationTypeIdAndPresentUkprn()
        {
            var request = new CreateEpaOrganisationRequest
            {
                Name = "test",
                Ukprn = 12345678,
                OrganisationTypeId = 9
            };

            _registerRepository.Setup(r => r.OrganisationTypeExists(It.IsAny<int>()))
                .Returns(Task.FromResult(false));
            _registerRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(true));
            var result = _validator.ValidatorCreateEpaOrganisationRequest(request);

            result.Errors.Count(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()).Should().Be(1);
            result.Errors.Count(x => x.StatusCode == ValidationStatusCode.AlreadyExists.ToString()).Should().Be(1);
            result.Errors.Count(x => x.Field == "OrganisationTypeId").Should().Be(1);
            result.Errors.Count(x => x.Field == "Ukprn").Should().Be(1);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CheckRecognitionNumberInUseReturnsAnErrorMessage(bool exists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.EpaOrganisationExistsWithRecognitionNumber(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(exists));
            
            var valid = _validator.CheckRecognitionNumberInUse("RN", "orgName").Length == 0;

            valid.Should().Be(validExpected);
            _registerRepository.Verify(r => r.EpaOrganisationExistsWithRecognitionNumber(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CheckRecognitionNumberExistsReturnsAnErrorMessage(bool exists, bool validExpected)
        {
            _registerRepository
                .Setup(r => r.CheckRecognitionNumberExists(It.IsAny<string>()))
                .Returns(Task.FromResult(exists));

            var valid = _validator.CheckRecognitionNumberExists("RN").Length == 0;
            
            valid.Should().Be(validExpected);
            _registerRepository.Verify(r => r.CheckRecognitionNumberExists(It.IsAny<string>()), Times.Once);
        }
    }
}