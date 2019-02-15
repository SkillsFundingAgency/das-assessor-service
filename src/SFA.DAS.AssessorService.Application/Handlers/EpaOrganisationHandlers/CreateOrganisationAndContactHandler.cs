using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Interfaces.Validation;


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
            var noOrganisationNameMessage = "Organisation name missing - no organisation or contact added";
            var organisationNameTooShortMessage = "organisation name too short - no organisation or contact added";
            var organisationNameAlreadyUsedMessage = "organisation name already used - no organisation or contact added";
            var organisationTypeNotIdentifiedMessage = "organisation type not identified so not added to organisation in register";
            var ukprnNotValidMessage = "Ukprn is an invalid format so not added to this organisation in register";
            var ukprnAlreadyUsedMessage =
                "The ukprn is already used by another organisation so not added to this organisation in register";
            var companyNumberNotValidMessage = "Company number is an invalid format so not added to this organisation in register";
            var companyNumberAlreadyUsedMessage =
                "The company number is already used by another organisation so not added to this organisation in register";
            var charityNumberNotValidMessage = "Charity number is an invalid format so not added to this organisation in register";
            var charityNumberAlreadyUsedMessage =
                "The charity number is already used by another organisation so not added to this organisation in register";

            var emailIsMissingMessage =
                "The email is missing so the contact will not be added to the register";
            var emailIsInvalidMessage =
                "The email is invalid so the contact will not be added to the register";
            var emailAlreadyUsedMessage =
                "The email is already used by another organisation so the contact will not be added to the register";
            var contactNameIsMissingMessage =
                "The contact name is missing so the contact will not be added to the register";
            var contactNameIsInvalidMessage =
                "The contact name is invalid so the contact will not be added to the register";

            var contactAdded = false;
            var organisationAdded = false;
            var warningMessages = new List<string>();

            var organisationName = DecideOrganisationName(request.UseTradingName,request.TradingName, request.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(request.OrganisationUkprn, request.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(request.OrganisationType);
            var companyNumber = request.CompanyNumber;
            var charityNumber = request.CharityNumber;

            // organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationName(organisationName, warningMessages, noOrganisationNameMessage);

            if (!_validationService.IsMinimumLengthOrMore(organisationName, 2))
                warningMessages.Add(organisationNameTooShortMessage);

            if (await _assessorValidationService.IsOrganisationNameTaken(organisationName))
                warningMessages.Add(organisationNameAlreadyUsedMessage);

            // If details make adding organisation impossible, then eject here
            if (warningMessages.Count>0)
                return new CreateOrganisationContactResponse(null, false, false, warningMessages);

            if (organisationTypeId == null)
                warningMessages.Add(organisationTypeNotIdentifiedMessage);

            if (!_validationService.UkprnIsValid(ukprnAsLong?.ToString()))
            {
                ukprnAsLong = null;
                warningMessages.Add(ukprnNotValidMessage);
            }

            if (ukprnAsLong.HasValue && await _assessorValidationService.IsOrganisationUkprnTaken(ukprnAsLong.Value))
            {
                ukprnAsLong = null;
                warningMessages.Add(ukprnAlreadyUsedMessage);
            }

            if (!_validationService.CompanyNumberIsValid(companyNumber))
            {
                companyNumber = null;
                warningMessages.Add(companyNumberNotValidMessage);
            }

            if (await _assessorValidationService.IsCompanyNumberTaken(companyNumber))
            {
                companyNumber = null;
                warningMessages.Add(companyNumberAlreadyUsedMessage);
            }

            if (!_validationService.CharityNumberIsValid(charityNumber))
            {
                charityNumber = null;
                warningMessages.Add(charityNumberNotValidMessage);
            }

            if (await _assessorValidationService.IsCharityNumberTaken(charityNumber))
            {
                charityNumber = null;
                warningMessages.Add(charityNumberAlreadyUsedMessage);
            }

            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
                throw new Exception("A valid organisation Id could not be generated");

            var organisation = MapRequestToOrganisation(request, newOrganisationId, organisationName, companyNumber, charityNumber,
                ukprnAsLong, organisationTypeId);
            var organisationSaved = await _registerRepository.CreateEpaOrganisation(organisation);

            organisationAdded = true;


            // Contact ////////////////////////////////
            var warningMessagesContact = new List<string>();

            if (!_validationService.IsNotEmpty(request.ContactEmail))
                warningMessagesContact.Add(emailIsMissingMessage);
            
            if(!_validationService.CheckEmailIsValid(request.ContactEmail))
                warningMessagesContact.Add(emailIsInvalidMessage);

            if(await _assessorValidationService.IsEmailTaken(request.ContactEmail))
                warningMessagesContact.Add(emailAlreadyUsedMessage);

            if (!_validationService.IsNotEmpty(request.ContactName))
                warningMessagesContact.Add(contactNameIsMissingMessage);

            if (!_validationService.IsMinimumLengthOrMore(request.ContactName,2))
                warningMessagesContact.Add(contactNameIsMissingMessage);



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
           
            return new CreateOrganisationContactResponse(newOrganisationId, organisationAdded, contactAdded, warningMessages);
        }

        private void RaiseWarningIfNoOrganisationName(string organisationName, List<string> warningMessages,
            string noOrganisationNameMessage)
        {
            if (!_validationService.IsNotEmpty(organisationName))
                warningMessages.Add(noOrganisationNameMessage);
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
