using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;
using SFA.DAS.AssessorService.Application.Resources;


namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class CreateOrganisationAndContactHandler : IRequestHandler<CreateOrganisationContactRequest, CreateOrganisationContactResponse>
    {
        private readonly IValidationService _validationService;
        private readonly IAssessorValidationService _assessorValidationService;

        private readonly IRegisterRepository _registerRepository;     
        private readonly IRegisterQueryRepository _registerQueryRepository;

        private readonly ILogger<CreateOrganisationAndContactHandler> _logger;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public CreateOrganisationAndContactHandler(IValidationService validationService, IAssessorValidationService assessorValidationService, IRegisterRepository registerRepository, IRegisterQueryRepository registerQueryRepository, ILogger<CreateOrganisationAndContactHandler> logger, IEpaOrganisationIdGenerator organisationIdGenerator, ISpecialCharacterCleanserService cleanser)
        {
            _validationService = validationService;
            _assessorValidationService = assessorValidationService;
            _registerRepository = registerRepository;
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
            _organisationIdGenerator = organisationIdGenerator;
            _cleanser = cleanser;

        }


        public async Task<CreateOrganisationContactResponse> Handle(CreateOrganisationContactRequest request, CancellationToken cancellationToken)
        {
            var contactAdded = false;
            var warningMessages = new List<string>();

            var organisationName = DecideOrganisationName(request.UseTradingName,request.TradingName, request.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(request.OrganisationUkprn, request.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(request.OrganisationType);
            var companyNumber = request.CompanyNumber;
            var charityNumber = request.CharityNumber;

            // organisation checks ////////////////////////////////
            RaiseBreakingWarningIfNoOrganisationName(organisationName, warningMessages);
            RaiseBreakingWarningIfOrganisationNameTooShort(organisationName, warningMessages);
            await RaiseBreakingWarningIfOrganisationNameAlreadyUsed(organisationName, warningMessages);

            if (warningMessages.Count>0)
                return new CreateOrganisationContactResponse(null, false, false, warningMessages);

            RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
            ukprnAsLong = RaiseWarningAndResetIfUkprnIsInvalid(ukprnAsLong, warningMessages);
            ukprnAsLong = await RaiseWarningAndResetIfUkprnIsAlreadyUsed(warningMessages, ukprnAsLong);
            companyNumber = RaiseWarningAndResetIfCompanyNumberIsInvalid(companyNumber, warningMessages);
            companyNumber = await RaiseWarningAndResetIfCompanyNumberAlreadyUsed(companyNumber, warningMessages);
            charityNumber = RaiseWarningAndResetIfCharityNumberIsInvalid(charityNumber, warningMessages);
            charityNumber = await RaiseWarningAndResetIfCharityNumberAlreadyUsed(charityNumber, warningMessages);

            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
                throw new Exception("A valid organisation Id could not be generated");

            var organisation = MapRequestToOrganisation(request, newOrganisationId, organisationName, companyNumber, charityNumber,
                ukprnAsLong, organisationTypeId);
            var organisationSaved = await _registerRepository.CreateEpaOrganisation(organisation);

            // Contact ////////////////////////////////
            var warningMessagesContact = new List<string>();

            RaiseBreakingWarningIfEmailIsMissingInvalidOrAlreadyUsed(request.ContactEmail, warningMessagesContact);
            RaiseWarningIfContactNameIsMissingOrTooShort(request.ContactName, warningMessagesContact);

            if (warningMessagesContact.Count > 0)
                warningMessages.AddRange(warningMessagesContact);
            else
            {
                var newUsername = _organisationIdGenerator.GetNextContactUsername();
                if (newUsername == string.Empty)
                    throw new Exception("A valid contact user name could not be generated");

                var contact = MapRequestToContact(request.ContactName,request.ContactEmail,organisationSaved,request.ContactPhoneNumber, newUsername);
                await _registerRepository.CreateEpaOrganisationContact(contact);
                contactAdded = true;
            }
           
            return new CreateOrganisationContactResponse(newOrganisationId, true, contactAdded, warningMessages);
        }

        private void RaiseWarningIfContactNameIsMissingOrTooShort(string contactName, List<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(contactName))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsMissing);

            if (!_validationService.IsMinimumLengthOrMore(contactName, 2))
                warningMessagesContact.Add(OrganisationAndContactMessages.ContactNameIsTooShort);
        }

        private void RaiseBreakingWarningIfEmailIsMissingInvalidOrAlreadyUsed(string email, ICollection<string> warningMessagesContact)
        {
            if (!_validationService.IsNotEmpty(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsMissing);

            if (!_validationService.CheckEmailIsValid(email))
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailIsInvalid);

            if (_assessorValidationService.IsEmailTaken(email).Result)
                warningMessagesContact.Add(OrganisationAndContactMessages.EmailAlreadyUsed);
        }

        private async Task<string> RaiseWarningAndResetIfCharityNumberAlreadyUsed(string charityNumber, ICollection<string> warningMessages)
        {
            if (!await _assessorValidationService.IsCharityNumberTaken(charityNumber)) return charityNumber;
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberAlreadyUsed);
            return null;
        }

        private string RaiseWarningAndResetIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        {
            if (_validationService.CharityNumberIsValid(charityNumber)) return charityNumber;
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberNotValid);
            return null;
        }

        private async Task<string> RaiseWarningAndResetIfCompanyNumberAlreadyUsed(string companyNumber, ICollection<string> warningMessages)
        {
            if (!await _assessorValidationService.IsCompanyNumberTaken(companyNumber)) return companyNumber;
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberAlreadyUsed);
            return null;
        }

        private string RaiseWarningAndResetIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        {
            if (_validationService.CompanyNumberIsValid(companyNumber)) return companyNumber;
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberNotValid);
            return null;
        }

        private async Task<long?> RaiseWarningAndResetIfUkprnIsAlreadyUsed(ICollection<string> warningMessages, long? ukprnAsLong)
        {
            if (!ukprnAsLong.HasValue || !await _assessorValidationService.IsOrganisationUkprnTaken(ukprnAsLong.Value)) return ukprnAsLong;
            warningMessages.Add(OrganisationAndContactMessages.UkprnAlreadyUsed);
            return null;
        }

        private long? RaiseWarningAndResetIfUkprnIsInvalid(long? ukprnAsLong, ICollection<string> warningMessages)
        {
            if (_validationService.UkprnIsValid(ukprnAsLong?.ToString())) return ukprnAsLong;
            warningMessages.Add(OrganisationAndContactMessages.UkprnIsInvalidFormat);
            return null;
        }

        private static void RaiseWarningOrganisationTypeNotIdentified(int? organisationTypeId, ICollection<string> warningMessages)
        {
            if (organisationTypeId == null)
                warningMessages.Add(OrganisationAndContactMessages.OrganisationTypeNotIdentified);
        }

        private async Task RaiseBreakingWarningIfOrganisationNameAlreadyUsed(string organisationName, ICollection<string> warningMessages)
        {
              if (await _assessorValidationService.IsOrganisationNameTaken(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationNameAlreadyUsed);
        }

        private void RaiseBreakingWarningIfOrganisationNameTooShort(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationNameTooShort);
        }

        private void RaiseBreakingWarningIfNoOrganisationName(string organisationName, ICollection<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationName))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationName);
        }


        private EpaContact MapRequestToContact(string contactName, string contactEmail, string organisationId, string contactPhoneNumber, string username)
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

        private EpaOrganisation MapRequestToOrganisation(CreateOrganisationContactRequest request, string newOrganisationId, string organisationName, string companyNumber, string charityNumber, long? ukprnAsLong, int? organisationTypeId)
        {
            organisationName = _cleanser.CleanseStringForSpecialCharacters(organisationName);
            var legalName = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationName);
            var tradingName = _cleanser.CleanseStringForSpecialCharacters(request.TradingName);
            var website = _cleanser.CleanseStringForSpecialCharacters(request.StandardWebsite);
            var address1 = _cleanser.CleanseStringForSpecialCharacters(request.ContactAddress1);
            var address2 = _cleanser.CleanseStringForSpecialCharacters(request.ContactAddress2);
            var address3 = _cleanser.CleanseStringForSpecialCharacters(request.ContactAddress3);
            var address4 = _cleanser.CleanseStringForSpecialCharacters(request.ContactAddress4);
            var postcode = _cleanser.CleanseStringForSpecialCharacters(request.ContactPostcode);
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

        private async Task<int?> GetOrganisationTypeIdFromDescriptor(string organisationType)
        {
            var organisationTypes = await _registerQueryRepository.GetOrganisationTypes();
           return organisationTypes.FirstOrDefault(x => string.Equals(x.Type.Replace(" ", ""),
                organisationType.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private static long? GetUkprnFromRequestDetails(string organisationUkprn, string companyUkprn )
        {

            long? ukprnAsLong = null;
            var ukprn = !string.IsNullOrEmpty(organisationUkprn) ? organisationUkprn : companyUkprn;

            if (long.TryParse(ukprn, out long _))
            {
                ukprnAsLong = long.Parse(ukprn);
            }
            return ukprnAsLong;
        }

        private static string DecideOrganisationName(bool useTradingName, string tradingName, string organisationName)
        {
            return useTradingName && !string.IsNullOrEmpty(tradingName)
                ? tradingName
                : organisationName;
        }  
    }
}
