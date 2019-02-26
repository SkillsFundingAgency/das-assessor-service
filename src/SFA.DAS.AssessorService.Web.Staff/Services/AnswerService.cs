using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IApplyApiClient _applyApiClient;
        private readonly IValidationService _validationService;
        private readonly IAssessorValidationService _assessorValidationService;

        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;

        private readonly ILogger<AnswerService> _logger;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public AnswerService(IApplyApiClient applyApiClient, IRegisterQueryRepository registerQueryRepository, IValidationService validationService, IAssessorValidationService assessorValidationService, IEpaOrganisationIdGenerator organisationIdGenerator, ILogger<AnswerService> logger, ISpecialCharacterCleanserService cleanser, IRegisterRepository registerRepository)
        {
            _applyApiClient = applyApiClient;
            _registerQueryRepository = registerQueryRepository;
            _validationService = validationService;
            _assessorValidationService = assessorValidationService;
            _organisationIdGenerator = organisationIdGenerator;
            _logger = logger;
            _cleanser = cleanser;
            _registerRepository = registerRepository;
        }

        public async Task<CreateOrganisationContactCommand> GatherAnswersForOrganisationAndContactForApplication(Guid applicationId)
        {
            var tradingName = await GetAnswer(applicationId, "trading-name");
            var useTradingNameString = await GetAnswer(applicationId, "use-trading-name");
            var contactName = await GetAnswer(applicationId, "contact-name");
            var contactAddress1 = await GetAnswer(applicationId, "contact-address") ?? await GetAnswer(applicationId, "contact-address1");
            var contactAddress2 = await GetAnswer(applicationId, "contact-address2");
            var contactAddress3 = await GetAnswer(applicationId, "contact-address3");
            var contactAddress4 = await GetAnswer(applicationId, "contact-address4");
            var contactPostcode = await GetAnswer(applicationId, "contact-postcode");
            var contactEmail = await GetAnswer(applicationId, "contact-email");
            var contactPhoneNumber = await GetAnswer(applicationId, "contact-phone-number");
            var companyUkprn = await GetAnswer(applicationId, "company-ukprn");
            var companyNumber = await GetAnswer(applicationId, "company-number");
            var charityNumber = await GetAnswer(applicationId, "charity-number");
            var standardWebsite = await GetAnswer(applicationId, "standard-website");
            var organisation = await _applyApiClient.GetOrganisationForApplication(applicationId);
            var organisationName = organisation.Name;
            var organisationType = organisation.OrganisationType;
            var organisationUkprn = organisation.OrganisationUkprn;


            var useTradingName = useTradingNameString != null && (useTradingNameString.ToLower() == "yes" || useTradingNameString.ToLower() == "true" || useTradingNameString.ToLower() == "1");

            var command = new CreateOrganisationContactCommand
            (organisationName,
                organisationType,
                organisationUkprn?.ToString(),
                tradingName,
                useTradingName,
                contactName,
                contactAddress1,
                contactAddress2,
                contactAddress3,
                contactAddress4,
                contactPostcode,
                contactEmail,
                contactPhoneNumber,
                companyUkprn,
                companyNumber,
                charityNumber,
                standardWebsite);


            return command;
        }
        public async Task<string> GetAnswer(Guid applicationId, string questionTag)
        {
           var response= await _applyApiClient.GetAnswer(applicationId, questionTag);

            return response.Answer;
        }

        public async Task<List<string>> InjectApplyOrganisationAndContactDetailsIntoRegister(
            CreateOrganisationContactCommand command)
        {
            var warningMessages = new List<string>();
            var organisationName = DecideOrganisationName(command.UseTradingName, command.TradingName, command.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(command.OrganisationUkprn, command.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(command.OrganisationType);
            var companyNumber = command.CompanyNumber;
            var charityNumber = command.CharityNumber;

            // organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationName(organisationName, warningMessages);
            RaiseWarningIfOrganisationNameTooShort(organisationName, warningMessages);
            await RaiseWarningIfOrganisationNameAlreadyUsed(organisationName, warningMessages);
            RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
            ukprnAsLong = RaiseWarningAndResetIfUkprnIsInvalid(ukprnAsLong, warningMessages);
            ukprnAsLong = await RaiseWarningAndResetIfUkprnIsAlreadyUsed(warningMessages, ukprnAsLong);
            companyNumber = RaiseWarningAndResetIfCompanyNumberIsInvalid(companyNumber, warningMessages);
            companyNumber = await RaiseWarningAndResetIfCompanyNumberAlreadyUsed(companyNumber, warningMessages);
            charityNumber = RaiseWarningAndResetIfCharityNumberIsInvalid(charityNumber, warningMessages);
            charityNumber = await RaiseWarningAndResetIfCharityNumberAlreadyUsed(charityNumber, warningMessages);

            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
            {
                _logger.LogWarning("A valid organisation Id could not be generated");
                throw new Exception("A valid organisation Id could not be generated");
            }
            var organisation = MapCommandToOrganisation(command, newOrganisationId, organisationName, companyNumber, charityNumber,
                ukprnAsLong, organisationTypeId);

            // Contact checks ////////////////////////////////   
            RaiseWarningIfEmailIsMissingInvalidOrAlreadyUsed(command.ContactEmail, warningMessages);
            RaiseWarningIfContactNameIsMissingOrTooShort(command.ContactName, warningMessages);

            if (warningMessages.Count == 0)
            {
                var newUsername = _organisationIdGenerator.GetNextContactUsername();
                if (newUsername == string.Empty)
                {
                    _logger.LogWarning("A valid contact user name could not be generated");
                    throw new Exception("A valid contact user name could not be generated");
                }
                newOrganisationId = await _registerRepository.CreateEpaOrganisation(organisation);
                var contact = MapCommandToContact(command.ContactName, command.ContactEmail, newOrganisationId, command.ContactPhoneNumber, newUsername);
                await _registerRepository.CreateEpaOrganisationContact(contact);
            }

            return warningMessages;
        }

        private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        {
            return useTradingName && !string.IsNullOrEmpty(tradingName)
                ? tradingName
                : organisationName;
        }

        private static long? GetUkprnFromRequestDetails(string organisationUkprn, string companyUkprn)
        {
            long? ukprnAsLong = null;
            var ukprn = !string.IsNullOrEmpty(organisationUkprn) ? organisationUkprn : companyUkprn;

            if (long.TryParse(ukprn, out long _))
            {
                ukprnAsLong = long.Parse(ukprn);
            }
            return ukprnAsLong;
        }

        private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        {
            var organisationTypes = await _registerQueryRepository.GetOrganisationTypes();
            return organisationTypes.FirstOrDefault(x => string.Equals(x.Type.Replace(" ", ""),
                organisationType.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private void RaiseWarningIfNoOrganisationName(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationName);
        }

        private void RaiseWarningIfOrganisationNameTooShort(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationNameTooShort);
        }

        private async Task RaiseWarningIfOrganisationNameAlreadyUsed(string organisationName, ICollection<string> warningMessages)
        {
            if (await _assessorValidationService.IsOrganisationNameTaken(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationNameAlreadyUsed);
        }

        private static void RaiseWarningOrganisationTypeNotIdentified(int? organisationTypeId, ICollection<string> warningMessages)
        {
            if (organisationTypeId == null)
                warningMessages.Add(OrganisationAndContactMessages.OrganisationTypeNotIdentified);
        }

        private long? RaiseWarningAndResetIfUkprnIsInvalid(long? ukprnAsLong, ICollection<string> warningMessages)
        {
            if (_validationService.UkprnIsValid(ukprnAsLong?.ToString())) return ukprnAsLong;
            warningMessages.Add(OrganisationAndContactMessages.UkprnIsInvalidFormat);
            return null;
        }

        private async Task<long?> RaiseWarningAndResetIfUkprnIsAlreadyUsed(ICollection<string> warningMessages, long? ukprnAsLong)
        {
            if (!ukprnAsLong.HasValue || !await _assessorValidationService.IsOrganisationUkprnTaken(ukprnAsLong.Value)) return ukprnAsLong;
            warningMessages.Add(OrganisationAndContactMessages.UkprnAlreadyUsed);
            return null;
        }

        private string RaiseWarningAndResetIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        {
            if (_validationService.CompanyNumberIsValid(companyNumber)) return companyNumber;
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberNotValid);
            return null;
        }

        private async Task<string> RaiseWarningAndResetIfCompanyNumberAlreadyUsed(string companyNumber, ICollection<string> warningMessages)
        {
            if (!await _assessorValidationService.IsCompanyNumberTaken(companyNumber)) return companyNumber;
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberAlreadyUsed);
            return null;
        }

        private string RaiseWarningAndResetIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        {
            if (_validationService.CharityNumberIsValid(charityNumber)) return charityNumber;
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberNotValid);
            return null;
        }
        private async Task<string> RaiseWarningAndResetIfCharityNumberAlreadyUsed(string charityNumber, ICollection<string> warningMessages)
        {
            if (!await _assessorValidationService.IsCharityNumberTaken(charityNumber)) return charityNumber;
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberAlreadyUsed);
            return null;
        }

        private void RaiseWarningIfEmailIsMissingInvalidOrAlreadyUsed(string email, ICollection<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsMissing);

            if (!_validationService.CheckEmailIsValid(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsInvalid);

            if (_assessorValidationService.IsEmailTaken(email).Result)
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailAlreadyUsed);
        }

        private void RaiseWarningIfContactNameIsMissingOrTooShort(string contactName, List<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(contactName))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsMissing);

            if (!_validationService.IsMinimumLengthOrMore(contactName, 2))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsTooShort);
        }

        private EpaOrganisation MapCommandToOrganisation(CreateOrganisationContactCommand command, string newOrganisationId, string organisationName, string companyNumber, string charityNumber, long? ukprnAsLong, int? organisationTypeId)
        {
            organisationName = _cleanser.CleanseStringForSpecialCharacters(organisationName);
            var legalName = _cleanser.CleanseStringForSpecialCharacters(command.OrganisationName);
            var tradingName = _cleanser.CleanseStringForSpecialCharacters(command.TradingName);
            var website = _cleanser.CleanseStringForSpecialCharacters(command.StandardWebsite);
            var address1 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress1);
            var address2 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress2);
            var address3 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress3);
            var address4 = _cleanser.CleanseStringForSpecialCharacters(command.ContactAddress4);
            var postcode = _cleanser.CleanseStringForSpecialCharacters(command.ContactPostcode);
            companyNumber = _cleanser.CleanseStringForSpecialCharacters(companyNumber);
            charityNumber = _cleanser.CleanseStringForSpecialCharacters(charityNumber);

            if (!string.IsNullOrWhiteSpace(companyNumber))
            {
                companyNumber = companyNumber.ToUpper();
            }

            var organisation = new EpaOrganisation
            {
                Name = organisationName,
                OrganisationId = newOrganisationId,
                OrganisationTypeId = organisationTypeId,
                Ukprn = ukprnAsLong,
                Id = Guid.NewGuid(),
                OrganisationData = new OrganisationData
                {
                    Address1 = address1,
                    Address2 = address2,
                    Address3 = address3,
                    Address4 = address4,
                    LegalName = legalName,
                    TradingName = tradingName,
                    Postcode = postcode,
                    WebsiteLink = website,
                    CompanyNumber = companyNumber,
                    CharityNumber = charityNumber
                }
            };

            return organisation;
        }

        private EpaContact MapCommandToContact(string contactName, string contactEmail, string organisationId, string contactPhoneNumber, string username)
        {
            contactName = _cleanser.CleanseStringForSpecialCharacters(contactName);
            contactEmail = _cleanser.CleanseStringForSpecialCharacters(contactEmail);
            contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(contactPhoneNumber);

            return new EpaContact
            {
                DisplayName = contactName,
                Email = contactEmail,
                EndPointAssessorOrganisationId = organisationId,
                Id = Guid.NewGuid(),
                PhoneNumber = contactPhoneNumber,
                Username = username
            };
        }
    }

  
}
