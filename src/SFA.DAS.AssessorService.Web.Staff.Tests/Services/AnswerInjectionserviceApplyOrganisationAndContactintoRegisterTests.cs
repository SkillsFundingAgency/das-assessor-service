using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FluentAssertions.Primitives;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Services;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Web.Staff.Resources;

namespace SFA.DAS.AssessorService.Web.Staff.Tests.Services
{
    [TestFixture]
    public class AnswerInjectionserviceApplyOrganisationAndContactintoRegisterTests
    {
        private AnswerInjectionService _answerInjectionService;
        private Mock<IRegisterQueryRepository> _mockRegisterQueryRepository;
        private IValidationService _validationService;
        private Mock<IAssessorValidationService> _mockAssessorValidationService;
        private Mock<IEpaOrganisationIdGenerator> _mockEpaOrganisationIdGenerator;
        private Mock<ILogger<AnswerService>> _mockLogger;
        private Mock<ISpecialCharacterCleanserService> _mockSpecialCharacterCleanserService;
        private Mock<IRegisterRepository> _mockRegisterRepository;

        private Guid _applicationId;
        private string _organisationId;

        [SetUp]
        public void Setup()
        {
            _applicationId = Guid.NewGuid();
            _mockRegisterQueryRepository = new Mock<IRegisterQueryRepository>();
           // _mockValidationService = new Mock<IValidationService>();
           _validationService = new ValidationService();
            _mockAssessorValidationService = new Mock<IAssessorValidationService>();
            _mockEpaOrganisationIdGenerator = new Mock<IEpaOrganisationIdGenerator>();
            _mockLogger = new Mock<ILogger<AnswerService>>();
            _mockSpecialCharacterCleanserService = new Mock<ISpecialCharacterCleanserService>();
            _mockRegisterRepository = new Mock<IRegisterRepository>();
            _answerInjectionService = new AnswerInjectionService(
                _validationService,
                _mockAssessorValidationService.Object,
                _mockRegisterQueryRepository.Object,
                _mockRegisterRepository.Object,
                _mockEpaOrganisationIdGenerator.Object,
                _mockSpecialCharacterCleanserService.Object,
                _mockLogger.Object
            );


            var organisationType1 = new OrganisationType { Id = 1, Type = "Type 1" };
            var organisationType2 = new OrganisationType { Id = 2, Type = "Training Provider" };

            var expectedOrganisationTypes = new List<OrganisationType>
            {
                organisationType1,
                organisationType2
            };

            _organisationId = "EPA9999";
            _mockAssessorValidationService.Setup(s => s.IsOrganisationNameTaken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockAssessorValidationService.Setup(s => s.IsOrganisationUkprnTaken(It.IsAny<long>())).Returns(Task.FromResult(false));
            _mockAssessorValidationService.Setup(s => s.IsCompanyNumberTaken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockAssessorValidationService.Setup(s => s.IsCharityNumberTaken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockAssessorValidationService.Setup(s => s.IsEmailTaken(It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockRegisterQueryRepository.Setup(r => r.GetOrganisationTypes()).Returns(Task.FromResult(expectedOrganisationTypes.AsEnumerable()));
            _mockEpaOrganisationIdGenerator.Setup(g => g.GetNextOrganisationId()).Returns(_organisationId);
            _mockEpaOrganisationIdGenerator.Setup(g => g.GetNextContactUsername()).Returns("unknown-9999");
            _mockSpecialCharacterCleanserService.Setup(c => c.CleanseStringForSpecialCharacters(It.IsAny<string>()))
                .Returns((string s) => s);

            _mockRegisterRepository.Setup(r => r.CreateEpaOrganisation(It.IsAny<EpaOrganisation>())).Returns(Task.FromResult(_organisationId));
            _mockRegisterRepository.Setup(r => r.CreateEpaOrganisationContact(It.IsAny<EpaContact>())).Returns(Task.FromResult("unknown-9999"));  

        }

        [Test, TestCaseSource(nameof(InjectionTestCases))]
        public void WhenInjectingOrganisationAndContactForAnApplication(InjectionTestCase testCase)
        {
            var actualResponse = _answerInjectionService.InjectApplyOrganisationAndContactDetailsIntoRegister(testCase.Command).Result;
            Assert.AreEqual(JsonConvert.SerializeObject(testCase.ExpectedResponse), JsonConvert.SerializeObject(actualResponse));
        }



        protected static IEnumerable<InjectionTestCase> InjectionTestCases
        {
            get
            {
                yield return new InjectionTestCase("RoEPAO", true, false, null, null, null, null, null, null, null, null, null, null);
                yield return new InjectionTestCase("RoATP", false, true, null, null, null, null, null, null, null, null, null, null);
                yield return new InjectionTestCase("RoATP", false, false, "org name", null, "TrainingProvider", "12345678", "12345678", "1234", "EPA9999", "joe@cool.com", "Joe Cool", null);
                yield return new InjectionTestCase("RoATP", false, false, null, "trading name 1", "TrainingProvider", "12345678", "12345678", "1234", "EPA9999", "joe@cool.com", "Joe Cool", null);
                //yield return new InjectionTestCase("RoATP", false, false, "a", null, "TrainingProvider", "12345678", "12345678", "1234", "EPA9999", "joe@cool.com", "Joe Cool", "OrganisationNameTooShort");
            }
        }

        public class InjectionTestCase
        {
            public CreateOrganisationContactCommand Command { get; set; }
            public CreateOrganisationAndContactFromApplyResponse ExpectedResponse { get; set; }

            public InjectionTestCase(string organisationReferenceType, bool isEpaoSource, bool isEpaoApproved, string organisationName, string tradingName, string organisationType, string ukprn, string companyNumber, string charityNumber, string organisationId, string email, string contactName, string warningMessage1)
            {
                var warningMessages = new List<string>();
                if (!string.IsNullOrEmpty(warningMessage1))
                {
                    warningMessages.Add(warningMessage1);
                    //    if (warningMessage1 == "OrganisationNameTooShort")
                    //        warningMessages.Add(OrganisationAndContactMessages.OrganisationNameTooShort);
                }
                var response = new CreateOrganisationAndContactFromApplyResponse { IsEpaoApproved = isEpaoApproved, ApplySourceIsEpao = isEpaoSource, WarningMessages = warningMessages, OrganisationId = organisationId };
                //Command = new CreateOrganisationContactCommand();
                //{OrganisationReferenceType = organisationReferenceType};
                Command = new CreateOrganisationContactCommand
                    {
                        OrganisationReferenceType = organisationReferenceType,
                        IsEpaoApproved = isEpaoApproved,
                        OrganisationName = organisationName,
                        TradingName = tradingName,
                        UseTradingName = true,
                        OrganisationUkprn = ukprn,
                        CompanyUkprn = "87654321",
                        CompanyNumber = companyNumber,
                        CharityNumber = charityNumber,
                        OrganisationType = organisationType,
                        ContactEmail = email,
                        ContactName = contactName,
                        ContactPhoneNumber = "11111111"
                };
                ExpectedResponse = response;
            }
        }

        //[TestCase("tag-one", "answer1")]
        //[TestCase("tag-two", null)]
        //[TestCase(null, null)]
        //public void WhenGettingAnswersForQuestionTag(string questionTag, string answer)
        //{
        //    _mockApplyApiClient.Setup(x => x.GetAnswer(_applicationId, questionTag))
        //        .Returns(Task.FromResult(new GetAnswersResponse { Answer = answer }));

        //    var expectedResult = _answerService.GetAnswer(_applicationId, questionTag).Result;

        //    Assert.AreEqual(expectedResult, answer);
        //}



        //    public async Task<CreateOrganisationAndContactFromApplyResponse> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command)
        //    {
        //        var response = new CreateOrganisationAndContactFromApplyResponse { IsEpaoApproved = false, ApplySourceIsEpao = false, WarningMessages = new List<string>() };


        //        var warningMessages = new List<string>();


        //        var organisationName = DecideOrganisationName(command.UseTradingName, command.TradingName, command.OrganisationName);
        //        var ukprnAsLong = GetUkprnFromRequestDetails(command.OrganisationUkprn, command.CompanyUkprn);
        //        var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(command.OrganisationType);
        //        var companyNumber = command.CompanyNumber;
        //        var charityNumber = command.CharityNumber;

        //        // organisation checks ////////////////////////////////
        //        RaiseWarningIfNoOrganisationName(organisationName, warningMessages);
        //        RaiseWarningIfOrganisationNameTooShort(organisationName, warningMessages);
        //        await RaiseWarningIfOrganisationNameAlreadyUsed(organisationName, warningMessages);
        //        RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
        //        ukprnAsLong = RaiseWarningAndResetIfUkprnIsInvalid(ukprnAsLong, warningMessages);
        //        ukprnAsLong = await RaiseWarningAndResetIfUkprnIsAlreadyUsed(warningMessages, ukprnAsLong);
        //        companyNumber = RaiseWarningAndResetIfCompanyNumberIsInvalid(companyNumber, warningMessages);
        //        companyNumber = await RaiseWarningAndResetIfCompanyNumberAlreadyUsed(companyNumber, warningMessages);
        //        charityNumber = RaiseWarningAndResetIfCharityNumberIsInvalid(charityNumber, warningMessages);
        //        charityNumber = await RaiseWarningAndResetIfCharityNumberAlreadyUsed(charityNumber, warningMessages);

        //        var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
        //        if (newOrganisationId == string.Empty)
        //        {
        //            _logger.LogWarning("A valid organisation Id could not be generated");
        //            throw new Exception("A valid organisation Id could not be generated");
        //        }
        //        var organisation = MapCommandToOrganisation(command, newOrganisationId, organisationName, companyNumber, charityNumber,
        //            ukprnAsLong, organisationTypeId);

        //        // Contact checks ////////////////////////////////   
        //        RaiseWarningIfEmailIsMissingInvalidOrAlreadyUsed(command.ContactEmail, warningMessages);
        //        RaiseWarningIfContactNameIsMissingOrTooShort(command.ContactName, warningMessages);

        //        if (warningMessages.Count == 0)
        //        {
        //            var newUsername = _organisationIdGenerator.GetNextContactUsername();
        //            if (newUsername == string.Empty)
        //            {
        //                _logger.LogWarning("A valid contact user name could not be generated");
        //                throw new Exception("A valid contact user name could not be generated");
        //            }
        //            newOrganisationId = await _registerRepository.CreateEpaOrganisation(organisation);
        //            var contact = MapCommandToContact(command.ContactName, command.ContactEmail, newOrganisationId, command.ContactPhoneNumber, newUsername);
        //            await _registerRepository.CreateEpaOrganisationContact(contact);
        //            response.OrganisationId = newOrganisationId;
        //        }

        //        response.WarningMessages = warningMessages;


        //        return response;
        //    }

        //    private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        //    {
        //        return useTradingName && !string.IsNullOrEmpty(tradingName)
        //            ? tradingName
        //            : organisationName;
        //    }

        //    private static long? GetUkprnFromRequestDetails(string organisationUkprn, string companyUkprn)
        //    {
        //        long? ukprnAsLong = null;
        //        var ukprn = !string.IsNullOrEmpty(organisationUkprn) ? organisationUkprn : companyUkprn;

        //        if (long.TryParse(ukprn, out long _))
        //        {
        //            ukprnAsLong = long.Parse(ukprn);
        //        }
        //        return ukprnAsLong;
        //    }

        //    private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        //    {
        //        var organisationTypes = await _registerQueryRepository.GetOrganisationTypes();
        //        return organisationTypes.FirstOrDefault(x => string.Equals(x.Type.Replace(" ", ""),
        //            organisationType.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        //    }

        //    private void RaiseWarningIfNoOrganisationName(string organisationName, ICollection<string> warningMessages)
        //    {
        //        if (!_validationService.IsNotEmpty(organisationName))
        //            warningMessages.Add(OrganisationAndContactMessages.NoOrganisationName);
        //    }

        //    private void RaiseWarningIfOrganisationNameTooShort(string organisationName, ICollection<string> warningMessages)
        //    {
        //        if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
        //            warningMessages.Add(OrganisationAndContactMessages.OrganisationNameTooShort);
        //    }

        //    private async Task RaiseWarningIfOrganisationNameAlreadyUsed(string organisationName, ICollection<string> warningMessages)
        //    {
        //        if (await _assessorValidationService.IsOrganisationNameTaken(organisationName))
        //            warningMessages.Add(OrganisationAndContactMessages.OrganisationNameAlreadyUsed);
        //    }

        //    private static void RaiseWarningOrganisationTypeNotIdentified(int? organisationTypeId, ICollection<string> warningMessages)
        //    {
        //        if (organisationTypeId == null)
        //            warningMessages.Add(OrganisationAndContactMessages.OrganisationTypeNotIdentified);
        //    }

        //    private long? RaiseWarningAndResetIfUkprnIsInvalid(long? ukprnAsLong, ICollection<string> warningMessages)
        //    {
        //        if (_validationService.UkprnIsValid(ukprnAsLong?.ToString())) return ukprnAsLong;
        //        warningMessages.Add(OrganisationAndContactMessages.UkprnIsInvalidFormat);
        //        return null;
        //    }

        //    private async Task<long?> RaiseWarningAndResetIfUkprnIsAlreadyUsed(ICollection<string> warningMessages, long? ukprnAsLong)
        //    {
        //        if (!ukprnAsLong.HasValue || !await _assessorValidationService.IsOrganisationUkprnTaken(ukprnAsLong.Value)) return ukprnAsLong;
        //        warningMessages.Add(OrganisationAndContactMessages.UkprnAlreadyUsed);
        //        return null;
        //    }

        //    private string RaiseWarningAndResetIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        //    {
        //        if (_validationService.CompanyNumberIsValid(companyNumber)) return companyNumber;
        //        warningMessages.Add(OrganisationAndContactMessages.CompanyNumberNotValid);
        //        return null;
        //    }

        //    private async Task<string> RaiseWarningAndResetIfCompanyNumberAlreadyUsed(string companyNumber, ICollection<string> warningMessages)
        //    {
        //        if (!await _assessorValidationService.IsCompanyNumberTaken(companyNumber)) return companyNumber;
        //        warningMessages.Add(OrganisationAndContactMessages.CompanyNumberAlreadyUsed);
        //        return null;
        //    }

        //    private string RaiseWarningAndResetIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        //    {
        //        if (_validationService.CharityNumberIsValid(charityNumber)) return charityNumber;
        //        warningMessages.Add(OrganisationAndContactMessages.CharityNumberNotValid);
        //        return null;
        //    }
        //    private async Task<string> RaiseWarningAndResetIfCharityNumberAlreadyUsed(string charityNumber, ICollection<string> warningMessages)
        //    {
        //        if (!await _assessorValidationService.IsCharityNumberTaken(charityNumber)) return charityNumber;
        //        warningMessages.Add(OrganisationAndContactMessages.CharityNumberAlreadyUsed);
        //        return null;
        //    }

        //    private void RaiseWarningIfEmailIsMissingInvalidOrAlreadyUsed(string email, ICollection<string> warningMessagesContact)
        //    {
        //        if (!_validationService.IsNotEmpty(email))
        //            warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsMissing);

        //        if (!_validationService.CheckEmailIsValid(email))
        //            warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsInvalid);

        //        if (_assessorValidationService.IsEmailTaken(email).Result)
        //            warningMessagesContact.Add(OrganisationAndContactMessages.EmailAlreadyUsed);
        //    }

        //    private void RaiseWarningIfContactNameIsMissingOrTooShort(string contactName, List<string> warningMessagesContact)
        //    {
        //        if (!_validationService.IsNotEmpty(contactName))
        //            warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsMissing);

        //        if (!_validationService.IsMinimumLengthOrMore(contactName, 2))
        //            warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsTooShort);
        //    }

        //    private EpaOrganisation MapCommandToOrganisation(CreateOrganisationContactCommand command, string newOrganisationId, string organisationName, string companyNumber, string charityNumber, long? ukprnAsLong, int? organisationTypeId)
        //    {
        //        organisationName = _cleanser.CleanseStringForSpecialCharacters(organisationName);
        //        var legalName = _cleanser.CleanseStringForSpecialCharacters(command.OrganisationName);
        //        var tradingName = _cleanser.CleanseStringForSpecialCharacters(command.TradingName);
        //        var website = _cleanser.CleanseStringForSpecialCharacters(command.StandardWebsite);
        //        var address1 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress1);
        //        var address2 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress2);
        //        var address3 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress3);
        //        var address4 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress4);
        //        var postcode = _cleanser.CleanseStringForSpecialCharacters(command.ContactPostcode);
        //        companyNumber = _cleanser.CleanseStringForSpecialCharacters(companyNumber);
        //        charityNumber = _cleanser.CleanseStringForSpecialCharacters(charityNumber);

        //        if (!string.IsNullOrWhiteSpace(companyNumber))
        //        {
        //            companyNumber = companyNumber.ToUpper();
        //        }

        //        var organisation = new EpaOrganisation
        //        {
        //            Name = organisationName,
        //            OrganisationId = newOrganisationId,
        //            OrganisationTypeId = organisationTypeId,
        //            Ukprn = ukprnAsLong,
        //            Id = Guid.NewGuid(),
        //            OrganisationData = new OrganisationData
        //            {
        //                Address1 = address1,
        //                Address2 = address2,
        //                Address3 = address3,
        //                Address4 = address4,
        //                LegalName = legalName,
        //                TradingName = tradingName,
        //                Postcode = postcode,
        //                WebsiteLink = website,
        //                CompanyNumber = companyNumber,
        //                CharityNumber = charityNumber
        //            }
        //        };

        //        return organisation;
        //    }

        //    private EpaContact MapCommandToContact(string contactName, string contactEmail, string organisationId, string contactPhoneNumber, string username)
        //    {
        //        contactName = _cleanser.CleanseStringForSpecialCharacters(contactName);
        //        contactEmail = _cleanser.CleanseStringForSpecialCharacters(contactEmail);
        //        contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(contactPhoneNumber);

        //        return new EpaContact
        //        {
        //            DisplayName = contactName,
        //            Email = contactEmail,
        //            EndPointAssessorOrganisationId = organisationId,
        //            Id = Guid.NewGuid(),
        //            PhoneNumber = contactPhoneNumber,
        //            Username = username
        //        };
        //    }
        //}

    }

    
}


