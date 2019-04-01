using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Commands;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;

namespace SFA.DAS.AssessorService.Web.Staff.Services
{
    public class AnswerInjectionService : IAnswerInjectionService
    {
        private readonly IValidationService _validationService;
        private readonly IAssessorValidationService _assessorValidationService;

        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;

        private readonly ILogger<AnswerService> _logger;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public AnswerInjectionService(IValidationService validationService,
            IAssessorValidationService assessorValidationService, IRegisterQueryRepository registerQueryRepository,
            IRegisterRepository registerRepository, IEpaOrganisationIdGenerator organisationIdGenerator,
            ISpecialCharacterCleanserService cleanser, ILogger<AnswerService> logger)
        {
            _validationService = validationService;
            _assessorValidationService = assessorValidationService;
            _registerQueryRepository = registerQueryRepository;
            _registerRepository = registerRepository;
            _organisationIdGenerator = organisationIdGenerator;
            _cleanser = cleanser;
            _logger = logger;
        }


        public async Task<CreateOrganisationAndContactFromApplyResponse> InjectApplyOrganisationAndContactDetailsIntoRegister(CreateOrganisationContactCommand command)
        {
            var response = new CreateOrganisationAndContactFromApplyResponse { IsEpaoApproved = false, ApplySourceIsEpao = false, WarningMessages = new List<string>() };
            if (command.OrganisationReferenceType != null &&
                command.OrganisationReferenceType.ToLower().Contains("epao"))
            {
                response.ApplySourceIsEpao = true;
                return response;
            }

            if (command.IsEpaoApproved.Value)
            {
                response.IsEpaoApproved = true;
                return response;
            }

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

            //if (warningMessages.Count == 0) //Taken out because of Deen
           // {
                var newUsername = _organisationIdGenerator.GetNextContactUsername();
                if (newUsername == string.Empty)
                {
                    _logger.LogWarning("A valid contact user name could not be generated");
                    throw new Exception("A valid contact user name could not be generated");
                }
                newOrganisationId = await _registerRepository.CreateEpaOrganisation(organisation);
                var contact = MapCommandToContact(command.CreatedBy,command.ContactName, command.ContactEmail, newOrganisationId, command.ContactPhoneNumber, newUsername);
                var assessorContact = await _registerQueryRepository.GetContactByContactId(contact.Id);
                if (assessorContact != null)
                {
                    //Update existing contact entry
                    var newOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(newOrganisationId);
                    await _registerRepository.AssociateOrganisationWithContact(assessorContact.Id, newOrganisation,
                        "Live", "MakePrimaryContact");
                }
                else
                {
                    //Create a new contact entry
                    await _registerRepository.CreateEpaOrganisationContact(contact);
                }
              
                response.OrganisationId = newOrganisationId;
         //   }

            response.WarningMessages = warningMessages;
            

            return response;
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

        private EpaContact MapCommandToContact(string id, string contactName, string contactEmail, string organisationId, string contactPhoneNumber, string username)
        {
            contactName = _cleanser.CleanseStringForSpecialCharacters(contactName);
            contactEmail = _cleanser.CleanseStringForSpecialCharacters(contactEmail);
            contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(contactPhoneNumber);

            return new EpaContact
            {
                DisplayName = contactName,
                Email = contactEmail,
                EndPointAssessorOrganisationId = organisationId,
                Id = string.IsNullOrEmpty(id)?Guid.NewGuid():Guid.Parse(id),
                PhoneNumber = contactPhoneNumber,
                Username = username
            };
        }
    }
}
