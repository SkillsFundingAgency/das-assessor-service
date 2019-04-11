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

            if ("RoEPAO".Equals(command.OrganisationReferenceType, StringComparison.InvariantCultureIgnoreCase))
            {
                await UpdateFinancialDetails(command);
                _logger.LogInformation("Source reference type is EPAO. No need to inject organisation details into register");
                response.ApplySourceIsEpao = true;
                return response;
            }
            else if (command.IsEpaoApproved is true)
            {
                await UpdateFinancialDetails(command);
                _logger.LogInformation("Source is RoEPAO approved. No need to inject organisation details into register");
                response.IsEpaoApproved = true;
                return response;
            }

            var warningMessages = new List<string>();
            var organisationName = DecideOrganisationName(command.UseTradingName, command.TradingName, command.OrganisationName);
            var ukprnAsLong = GetUkprnFromRequestDetails(command.OrganisationUkprn, command.CompanyUkprn);
            var organisationTypeId = await GetOrganisationTypeIdFromDescriptor(command.OrganisationType);

            // organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationName(organisationName, warningMessages);
            RaiseWarningIfOrganisationNameTooShort(organisationName, warningMessages);
            await RaiseWarningIfOrganisationNameAlreadyUsed(organisationName, warningMessages);
            RaiseWarningOrganisationTypeNotIdentified(organisationTypeId, warningMessages);
            RaiseWarningIfUkprnIsInvalid(ukprnAsLong, warningMessages);
            await RaiseWarningIfUkprnIsAlreadyUsed(ukprnAsLong, warningMessages);
            RaiseWarningIfCompanyNumberIsInvalid(command.CompanyNumber, warningMessages);
            await RaiseWarningIfCompanyNumberAlreadyUsed(command.CompanyNumber, warningMessages);
            RaiseWarningIfCharityNumberIsInvalid(command.CharityNumber, warningMessages);
            await RaiseWarningIfCharityNumberAlreadyUsed(command.CharityNumber, warningMessages);

            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
            {
                _logger.LogError("A valid organisation Id could not be generated");
                throw new Exception("A valid organisation Id could not be generated");
            }
            var organisation = MapCommandToOrganisation(command, newOrganisationId, organisationName, ukprnAsLong, organisationTypeId);

            // Contact checks ////////////////////////////////   
            RaiseWarningIfEmailIsMissingInvalidOrAlreadyUsed(command.ContactEmail, warningMessages);
            RaiseWarningIfContactNameIsMissingOrTooShort(command.ContactName, warningMessages);

            if (warningMessages.Count == 0)
            {
                var newUsername = _organisationIdGenerator.GetNextContactUsername();
                if (newUsername == string.Empty)
                {
                    _logger.LogError("A valid contact user name could not be generated");
                    throw new Exception("A valid contact user name could not be generated");
                }

                _logger.LogInformation("Injecting new organisation into register");
                newOrganisationId = await _registerRepository.CreateEpaOrganisation(organisation);

                var contact = MapCommandToContact(command, newOrganisationId, newUsername);
                var assessorContact = await _registerQueryRepository.GetContactByContactId(contact.Id);

                if (assessorContact != null)
                {
                    //Update existing contact entry
                    var newOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(newOrganisationId);
                    await _registerRepository.AssociateOrganisationWithContact(assessorContact.Id, newOrganisation, "Live", "MakePrimaryContact");
                }
                else
                {
                    //Create a new contact entry
                    _logger.LogInformation("Injecting new contact into register");
                    await _registerRepository.CreateEpaOrganisationContact(contact);
                }
              
                response.EpaOrganisationId = newOrganisationId;
            }
            else
            {
                _logger.LogWarning("Source has invalid data. Cannot inject organisation details into register at this time");
            }

            response.WarningMessages = warningMessages;          

            return response;
        }


        public async Task<CreateOrganisationStandardFromApplyResponse> InjectApplyOrganisationStandardDetailsIntoRegister(CreateOrganisationStandardCommand command)
        {
            var response = new CreateOrganisationStandardFromApplyResponse { WarningMessages = new List<string>() };

            var warningMessages = new List<string>();

            // organisation checks ////////////////////////////////
            RaiseWarningIfNoOrganisationId(command.OrganisationId, warningMessages);
            RaiseWarningIfOrganisationIdIsInvalid(command.OrganisationId, warningMessages);

            // Standard checks ///////////////////////////////////
            RaiseWarningIfStandardInvalidOrAlreadyUsed(command.StandardCode, command.OrganisationId, warningMessages);

            if (warningMessages.Count == 0)
            {
                var standard = MapCommandToOrganisationStandard(command);
                var deliveryAreas = await MapCommandToDeliveryAreas(command);

                _logger.LogInformation("Injecting new standard into register");
                response.EpaoStandardId = await _registerRepository.CreateEpaOrganisationStandard(standard, deliveryAreas);
            }
            else
            {
                _logger.LogWarning("Source has invalid data. Cannot inject standard details into register at this time");
            }

            response.WarningMessages = warningMessages;

            return response;
        }

        private async Task UpdateFinancialDetails(CreateOrganisationContactCommand command)
        {
            var epaOrgs = await _registerQueryRepository.GetAssessmentOrganisationsByNameOrCharityNumberOrCompanyNumber(command.OrganisationName);
            var result = epaOrgs.FirstOrDefault();

            if (result != null)
            {
                _logger.LogInformation($"Updating FHADetails for {result.Id}");
                var org = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(result.Id);

                if (org?.OrganisationData != null)
                {
                    if (org.OrganisationData.FHADetails is null)
                    {
                        org.OrganisationData.FHADetails = new FHADetails();
                    }

                    org.OrganisationData.FHADetails.FinancialDueDate = command.FinancialDueDate;
                    org.OrganisationData.FHADetails.FinancialExempt = command.IsFinancialExempt;
                    await _registerRepository.UpdateEpaOrganisation(org);
                }
            }
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

        private void RaiseWarningIfUkprnIsInvalid(long? ukprnAsLong, ICollection<string> warningMessages)
        {
            if (ukprnAsLong.HasValue && !_validationService.UkprnIsValid(ukprnAsLong.Value.ToString()))
            warningMessages.Add(OrganisationAndContactMessages.UkprnIsInvalidFormat);
        }

        private async Task RaiseWarningIfUkprnIsAlreadyUsed(long? ukprnAsLong, ICollection<string> warningMessages)
        {
            if (ukprnAsLong.HasValue && await _assessorValidationService.IsOrganisationUkprnTaken(ukprnAsLong.Value))
            warningMessages.Add(OrganisationAndContactMessages.UkprnAlreadyUsed);
        }

        private void RaiseWarningIfCompanyNumberIsInvalid(string companyNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(companyNumber) && !_validationService.CompanyNumberIsValid(companyNumber))
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberNotValid);
        }

        private async Task RaiseWarningIfCompanyNumberAlreadyUsed(string companyNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(companyNumber) && await _assessorValidationService.IsCompanyNumberTaken(companyNumber))
            warningMessages.Add(OrganisationAndContactMessages.CompanyNumberAlreadyUsed);
        }

        private void RaiseWarningIfCharityNumberIsInvalid(string charityNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(charityNumber) && !_validationService.CharityNumberIsValid(charityNumber))
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberNotValid);
        }

        private async Task RaiseWarningIfCharityNumberAlreadyUsed(string charityNumber, ICollection<string> warningMessages)
        {
            if (!string.IsNullOrEmpty(charityNumber) && await _assessorValidationService.IsCharityNumberTaken(charityNumber))
            warningMessages.Add(OrganisationAndContactMessages.CharityNumberAlreadyUsed);
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

        private void RaiseWarningIfNoOrganisationId(string organisationId, List<string> warningMessages)
        {
            if (!_validationService.IsNotEmpty(organisationId))
                warningMessages.Add(OrganisationAndContactMessages.NoOrganisationId);
        }

        private void RaiseWarningIfOrganisationIdIsInvalid(string organisationId, List<string> warningMessages)
        {
            if (!_validationService.OrganisationIdIsValid(organisationId))
                warningMessages.Add(OrganisationAndContactMessages.OrganisationIdNotValid);
        }

        private void RaiseWarningIfStandardInvalidOrAlreadyUsed(int standardCode, string organisationId, List<string> warningMessagesStandard)
        {
            if (standardCode < 1)
            {
                warningMessagesStandard.Add(OrganisationAndContactMessages.StandardInvalid);
            }
            else if (_assessorValidationService.IsOrganisationStandardTaken(organisationId, standardCode).Result)
            {
                warningMessagesStandard.Add(OrganisationAndContactMessages.StandardAlreadyUsed);
            }
        }

        private EpaOrganisation MapCommandToOrganisation(CreateOrganisationContactCommand command, string newOrganisationId, string organisationName, long? ukprnAsLong, int? organisationTypeId)
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
            var companyNumber = _cleanser.CleanseStringForSpecialCharacters(command.CompanyNumber);
            var charityNumber = _cleanser.CleanseStringForSpecialCharacters(command.CharityNumber);

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
                    CharityNumber = charityNumber,
                    FHADetails = new FHADetails
                    {
                        FinancialDueDate = command.FinancialDueDate,
                        FinancialExempt = command.IsFinancialExempt
                    }
                }
            };

            return organisation;
        }

        private EpaContact MapCommandToContact(CreateOrganisationContactCommand command, string organisationId, string newUsername)
        {
            var id = command.CreatedBy;
            var contactName = _cleanser.CleanseStringForSpecialCharacters(command.ContactName);
            var contactEmail = _cleanser.CleanseStringForSpecialCharacters(command.ContactEmail);
            var contactPhoneNumber = _cleanser.CleanseStringForSpecialCharacters(command.ContactPhoneNumber); 

            return new EpaContact
            {
                DisplayName = contactName,
                Email = contactEmail,
                EndPointAssessorOrganisationId = organisationId,
                Id = string.IsNullOrEmpty(id)?Guid.NewGuid():Guid.Parse(id),
                PhoneNumber = contactPhoneNumber,
                Username = newUsername
            };
        }

        private EpaOrganisationStandard MapCommandToOrganisationStandard(CreateOrganisationStandardCommand command)
        {
            Guid? contactId = null;
            if (Guid.TryParse(command.CreatedBy, out var contactIdGuid))
            {
                contactId = contactIdGuid;
            }

            var standard = new EpaOrganisationStandard
            {
                OrganisationId = command.OrganisationId,
                StandardCode = command.StandardCode,
                EffectiveFrom = command.EffectiveFrom,
                ContactId = contactId,
                OrganisationStandardData = new OrganisationStandardData
                {
                    DeliveryAreasComments = string.Empty
                }
            };

            return standard;
        }

        private async Task<List<int>> MapCommandToDeliveryAreas(CreateOrganisationStandardCommand command)
        {
            if (command.DeliveryAreas != null)
            {
                var areas = await _registerQueryRepository.GetDeliveryAreas();
                return areas.Where(a => command.DeliveryAreas.Contains(a.Area)).Select(a => a.Id).ToList();
            }

            return new List<int>();
        }
    }
}
