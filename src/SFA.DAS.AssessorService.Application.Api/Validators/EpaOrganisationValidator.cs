using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterValidationRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly IStringLocalizer<EpaOrganisationValidator> _localizer;
        private readonly ISpecialCharacterCleanserService _cleanserService;
        private readonly IStandardService _standardService;

        private const string CompaniesHouseNumberRegex = "[A-Za-z0-9]{2}[0-9]{6}";
        private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";


        public EpaOrganisationValidator(IRegisterValidationRepository registerRepository, IRegisterQueryRepository registerQueryRepository,
                                        ISpecialCharacterCleanserService cleanserService, IStringLocalizer<EpaOrganisationValidator> localizer, IStandardService standardService)
        {
            _registerRepository = registerRepository;
            _registerQueryRepository = registerQueryRepository;
            _cleanserService = cleanserService;
            _localizer = localizer;
            _standardService = standardService;
        }
        
        public string CheckOrganisationIdIsPresentAndValid(string organisationId)
        {
            if (string.IsNullOrEmpty(organisationId) || organisationId.Trim().Length==0)
            {
               return FormatErrorMessage(EpaOrganisationValidatorMessageName.NoOrganisationId);
            }

            return organisationId.Trim().Length > 12 ? 
                FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationIdTooLong) 
                : string.Empty;
        }

        public string CheckOrganisationNameIsPresentAndValid(string name)
        {
            var organisationName = _cleanserService.CleanseStringForSpecialCharacters(name);
            if (string.IsNullOrEmpty(organisationName) || organisationName.Trim().Length==0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationNameEmpty);
            
            return organisationName.Trim().Length < 2 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationNameTooShort) 
                : string.Empty;
        }

        public string CheckIfOrganisationAlreadyExists(string organisationId)
        {
            if (organisationId == null || !_registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result) 
                return string.Empty;
            
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationIdAlreadyUsed);
        }

        public string CheckIfOrganisationUkprnExists(long? ukprn)
        {
            if (ukprn == null || !_registerRepository.EpaOrganisationExistsWithUkprn(ukprn.Value).Result) 
                return string.Empty;
            
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnAlreadyUsed);
        }

        public string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId)
        {
            if (organisationTypeId == null || _registerRepository.OrganisationTypeExists(organisationTypeId.Value).Result) 
                return string.Empty;
                
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationTypeIsInvalid);
        }

        public string CheckOrganisationTypeExists(int? organisationTypeId)
        {
            if (organisationTypeId != null && _registerRepository.OrganisationTypeExists(organisationTypeId.Value).Result) 
                return string.Empty;
            
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationTypeIsRequired);
        }


        public string CheckUkprnIsValid(long? ukprn)
        {
            if (ukprn == null) 
                return string.Empty;
            
            var isValid = ukprn >= 10000000 && ukprn <= 99999999;
            return isValid ? string.Empty : FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnIsInvalid);
        }

        public string CheckCompanyNumberIsMissingOrValid(string companyNumber)
        {

            if (String.IsNullOrWhiteSpace(companyNumber))
            {
                return string.Empty;
            }

            if ((companyNumber.Length != 8) ||
             !Regex.IsMatch(companyNumber, CompaniesHouseNumberRegex))
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCompanyNumberNotValid);
            }

            return string.Empty;
        }

        public string CheckCharityNumberIsValid(string charityNumber)
        {
            if (String.IsNullOrWhiteSpace(charityNumber))
            {
                return string.Empty;
            }

            if (Regex.IsMatch(charityNumber, CharityNumberInvalidCharactersRegex))
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCharityNumberNotValid);
            }

            return string.Empty;
        }

        public string CheckOrganisationNameNotExists(string name)
        {
            return _registerRepository.EpaOrganisationNameExists(name).Result 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent) : 
                string.Empty;
        }

        public string CheckOrganisationNameNotExistsExcludingOrganisation(string name, string excludingOrganisationId)
        {
            return _registerRepository.EpaOrganisationNameExists(name, excludingOrganisationId).Result 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent) : 
                string.Empty;
        }

        public string CheckIfDeliveryAreasAreValid(List<int> deliveryAreas)
        {
            if (deliveryAreas == null || deliveryAreas.Count == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.NoDeliveryAreasPresent);

            var validDeliveryAreas = _registerQueryRepository.GetDeliveryAreas().Result;

            foreach (var deliveryArea in deliveryAreas)
            {
                if (!validDeliveryAreas.Any(x => x.Id == deliveryArea))
                    return FormatErrorMessage(EpaOrganisationValidatorMessageName.DeliveryAreaNotValid);               
            }

            return string.Empty;
        }

        public string CheckSearchStringForStandardsIsValid(string searchstring)
        {
            if (searchstring == null) searchstring = string.Empty;       
            var isAnInt = int.TryParse(searchstring, out _);
            if (!isAnInt && searchstring.Length < 2)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.SearchStandardsTooShort);

            return string.Empty;
        }

        public string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore)
        {
        if (ukprn == null || !_registerRepository.EpaOrganisationAlreadyUsingUkprn(ukprn.Value, organisationIdToIgnore).Result) return string.Empty;
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnAlreadyUsed);
        }

        public string CheckOrganisationExists(string organisationId)
        {
            return organisationId != null && _registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result 
                ? string.Empty :
                FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationNotFound);
        }

        public async Task<Standard> GetStandard(string standardId)
        {
            return await _standardService.GetStandardVersionById(standardId);
        }

        public string CheckIfOrganisationStandardAlreadyExists(string organisationId, int standardCode)
        {
            return _registerRepository.EpaOrganisationStandardExists(organisationId, standardCode).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists)
                : string.Empty;
        }

        public string CheckIfOrganisationStandardVersionAlreadyExists(string organisationId, int standardCode, List<string> standardVersions)
        {
            return _registerRepository.EpaOrganisationStandardVersionExists(organisationId, standardCode, standardVersions).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionAlreadyExists)
                : string.Empty;
        }

        public string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode)
        {
            return _registerRepository.EpaOrganisationStandardExists(organisationId, standardCode).Result
                ? string.Empty
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist);
        }

        public string CheckIfContactIdIsValid(string contactId, string organisationId)
        {
            if (!Guid.TryParse(contactId, out Guid validContactId))
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdIsRequired);

            return _registerRepository.ContactIdIsValidForOrganisationId(validContactId, organisationId).Result
                ? string.Empty 
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId);
        }

        public string CheckDisplayNameIsPresentAndValid(string displayName)
        {
            var newName = _cleanserService.CleanseStringForSpecialCharacters(displayName);
            if (string.IsNullOrEmpty(newName) || newName.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameIsMissing);

            return newName.Trim().Length < 2
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameTooShort)
                : string.Empty;
        }

        public string CheckFirstNameIsPresentAndMinimumLength(string name)
        {
            var newName = _cleanserService.CleanseStringForSpecialCharacters(name);
            if (string.IsNullOrEmpty(newName) || newName.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.FirstNameIsMissing);

            return newName.Trim().Length < 2
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.FirstNameTooShort)
                : string.Empty;
        }

        public string CheckLastNameIsPresentAndMininumLength(string name)
        {
            var newName = _cleanserService.CleanseStringForSpecialCharacters(name);
            if (string.IsNullOrEmpty(newName) || newName.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.LastNameIsMissing);

            return newName.Trim().Length < 2
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.LastNameTooShort)
                : string.Empty;
        }

        public string CheckContactDetailsNotExists(string firstName, string lastName, string email, string phone, string contactId)
        {
            Guid? validContactId = Guid.TryParse(contactId, out Guid parsedContactId) ? parsedContactId : null;

            return _registerRepository.ContactDetailsAlreadyExist(firstName, lastName, email, phone, validContactId).Result
                    ? FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactDetailsAreDuplicates)
                    : string.Empty;
        }

        public string CheckContactIdExists(string contactId)
        {
            if (!Guid.TryParse(contactId, out Guid validContactId))
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdDoesntExist);
            
            return _registerRepository.ContactExists(validContactId).Result
                ? string.Empty
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdDoesntExist);
        }

        public string CheckEmailNotExists(string email)
        {
            return _registerRepository.EmailExists(email).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailAlreadyExists)
                : string.Empty;
        }

        public string CheckEmailNotExistsExcludingContact(string email, string excludingContactId)
        {
            if (!Guid.TryParse(excludingContactId, out Guid validExcludingContactId))
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdDoesntExist);

            return _registerRepository
                .EmailExistsExcludeContact(email, validExcludingContactId).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailAlreadyExists)
                : string.Empty;
        }

        public string CheckEmailPresentAndValid(string email)
        {
            var validationResults = new EmailValidator().Validate(new EmailChecker {EmailToCheck = email});
            return validationResults.IsValid ? string.Empty : FormatErrorMessage(validationResults.Errors.First().ErrorMessage);
        }

        public string CheckOrganisationStandardFromDateIsWithinStandardDateRanges(DateTime? effectiveFrom, DateTime? standardEffectiveFrom,
            DateTime? standardEffectiveTo, DateTime? lastDateForNewStarts)
        {
            if (effectiveFrom == null || standardEffectiveFrom == null)
                return string.Empty;

            if (effectiveFrom < standardEffectiveFrom)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromBeforeStandardEffectiveFrom);

            return string.Empty;
        }


        public string CheckOrganisationStandardToDateIsWithinStandardDateRanges(DateTime? effectiveTo, DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo)
        {
            if (effectiveTo == null)
                return string.Empty;

            if (effectiveTo < standardEffectiveFrom)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveToBeforeStandardEffectiveFrom);

            return string.Empty;
        }


        public string CheckEffectiveFromIsOnOrBeforeEffectiveTo(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            if (!effectiveFrom.HasValue || !effectiveTo.HasValue || effectiveFrom.Value <= effectiveTo.Value) return string.Empty;

            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardEffectiveFromAfterEffectiveTo);

        }

        public string CheckOrganisationStandardEffectiveFromIsEntered(DateTime? effectiveFrom)
        {
            return effectiveFrom != null 
                ? string.Empty 
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardCannotBeAddedBecauseEffectiveFromNotSet);
        }

        public string CheckAddressDetailsForOrganisation(string address1, string address2, string address3, string address4)
        {
            var address1Cleansed = _cleanserService.CleanseStringForSpecialCharacters(address1)?.Trim();
            var address2Cleansed = _cleanserService.CleanseStringForSpecialCharacters(address2)?.Trim();
            var address3Cleansed = _cleanserService.CleanseStringForSpecialCharacters(address3)?.Trim();
            var address4Cleansed = _cleanserService.CleanseStringForSpecialCharacters(address4)?.Trim();
            if ((address1Cleansed != null && address1Cleansed.Length > 1) ||
                (address2Cleansed != null && address2Cleansed.Length > 1) ||
                (address3Cleansed != null && address3Cleansed.Length > 1) ||
                (address4Cleansed != null && address4Cleansed.Length > 1))
                    return string.Empty;

            return FormatErrorMessage(EpaOrganisationValidatorMessageName.AddressIsNotEntered);
        }

        public string CheckOrganisationStandardVersionFromDateIsWithinStandardDateRanges(DateTime? effectiveFrom, DateTime? standardEffectiveFrom,
            DateTime? standardEffectiveTo, DateTime? lastDateForNewStarts)
        {
            if (effectiveFrom == null || standardEffectiveFrom == null)
                return string.Empty;

            if (effectiveFrom < standardEffectiveFrom)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveFromBeforeStandardEffectiveFrom);


            if (standardEffectiveTo.HasValue && effectiveFrom > standardEffectiveTo)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveFromAfterStandardEffectiveTo);


            if (lastDateForNewStarts.HasValue && effectiveFrom > lastDateForNewStarts)
                return
                    FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveFromAfterStandardLastDayForNewStarts);

            return string.Empty;
        }


        public string CheckOrganisationStandardVersionToDateIsWithinStandardDateRanges(DateTime? effectiveTo, DateTime? standardEffectiveFrom, DateTime? standardEffectiveTo)
        {
            if (effectiveTo == null)
                return string.Empty;

            if (effectiveTo < standardEffectiveFrom)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveToBeforeStandardEffectiveFrom);

            if (standardEffectiveTo.HasValue && effectiveTo > standardEffectiveTo)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveToAfterStandardEffectiveTo);

            return string.Empty;
        }


        public string CheckVersionEffectiveFromIsOnOrBeforeEffectiveTo(DateTime? effectiveFrom, DateTime? effectiveTo)
        {
            if (!effectiveFrom.HasValue || !effectiveTo.HasValue || effectiveFrom.Value <= effectiveTo.Value) return string.Empty;

            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveFromAfterEffectiveTo);

        }

        public string CheckVersionEffectiveToIsOnOrAfterEffectiveFrom(DateTime? effectiveTo, DateTime? effectiveFrom)
        {
            if (!effectiveFrom.HasValue || !effectiveTo.HasValue || effectiveTo.Value >= effectiveFrom.Value) return string.Empty;

            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardVersionEffectiveToBeforeEffectiveFrom);

        }
        public string CheckPostcodeIsPresentForOrganisation(string postcode)
        {
            var postcodeCleansed = _cleanserService.CleanseStringForSpecialCharacters(postcode);
            if (string.IsNullOrEmpty(postcodeCleansed) || postcodeCleansed.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.PostcodeIsNotEntered);

            return string.Empty;
        }

        public string CheckUkprnForOrganisation(long? ukprn)
        {
            if (ukprn == null)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnIsNotPresent);

            return string.Empty;
        }

        public string CheckContactCountForOrganisation(int? numberOfContacts)
        {
            if (numberOfContacts==null || numberOfContacts==0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactsAreNotPresent);

            return string.Empty;
        }

        public string CheckIfOrganisationCompanyNumberExists(string organisationIdToExclude, string companyNumber)
        {
            Task<bool> companyAlreadyRegistered = _registerRepository.EpaOrganisationExistsWithCompanyNumber(organisationIdToExclude, companyNumber);
            if (companyAlreadyRegistered.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCompanyNumberAlreadyUsed);
            }
            return string.Empty;
        }

        public string CheckIfOrganisationCompanyNumberExists(string companyNumber)
        {
            Task<bool> companyAlreadyRegistered = _registerRepository.EpaOrganisationExistsWithCompanyNumber(companyNumber);
            if (companyAlreadyRegistered.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCompanyNumberAlreadyUsed);
            }
            return string.Empty;
        }

        public string CheckIfOrganisationCharityNumberExists(string organisationIdToExclude, string charityNumber)
        {
            Task<bool> charityAlreadyRegistered = _registerRepository.EpaOrganisationExistsWithCharityNumber(organisationIdToExclude, charityNumber);
            if (charityAlreadyRegistered.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCharityNumberAlreadyUsed);
            }
            return string.Empty;
        }

        public string CheckIfOrganisationCharityNumberExists(string charityNumber)
        {
            Task<bool> charityAlreadyRegistered = _registerRepository.EpaOrganisationExistsWithCharityNumber(charityNumber);
            if (charityAlreadyRegistered.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationCharityNumberAlreadyUsed);
            }
            return string.Empty;
        }

        public string CheckOrganisationStandardId(int organisationStandardId)
        {
            if (organisationStandardId == 0)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardIdIsRequired);
            }
            return string.Empty;
        }

        public string CheckApplicationdIdIsPresent(Guid applicationId)
        {
            if (applicationId == Guid.Empty)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ApplicationIdIsRequired);
            }
            return string.Empty;
        }

        public string CheckRecognitionNumberExists(string recognitionNumber)
        {
            if (string.IsNullOrEmpty(recognitionNumber)) { return string.Empty; }
            Task<bool> recognitionNumberExists = _registerRepository.CheckRecognitionNumberExists(recognitionNumber.ToLower());
            if (!recognitionNumberExists.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.RecognitionNumberNotFound);
            }
            return string.Empty;
        }

        public string CheckRecognitionNumberInUse(string recognitionNumber, string organisationId = "")
        {
            if (string.IsNullOrEmpty(recognitionNumber)) { return string.Empty; }
            Task<bool> recognitionNumberAlreadyRegistered = _registerRepository.EpaOrganisationExistsWithRecognitionNumber(recognitionNumber.ToLower(), organisationId);
            if (recognitionNumberAlreadyRegistered.Result)
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.RecognitionNumberAlreadyInUse);
            }
            return string.Empty;
        }

        public ValidationResponse ValidatorCreateEpaOrganisationRequest(CreateEpaOrganisationRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameIsPresentAndValid(request.Name), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("OrganisationTypeId", CheckOrganisationTypeExists(request.OrganisationTypeId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckUkprnIsValid(request.Ukprn), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameNotExists(request.Name), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckIfOrganisationUkprnExists(request.Ukprn), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("CompanyNumber", CheckCompanyNumberIsMissingOrValid(request.CompanyNumber), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("CompanyNumber", CheckIfOrganisationCompanyNumberExists(request.CompanyNumber), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("CharityNumber", CheckCharityNumberIsValid(request.CharityNumber), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("CharityNumber", CheckIfOrganisationCharityNumberExists(request.CharityNumber), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("RecognitionNumber", CheckRecognitionNumberExists(request.RecognitionNumber), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("RecognitionNumber", CheckRecognitionNumberInUse(request.RecognitionNumber, request.EndPointAssessmentOrgId), validationResult, ValidationStatusCode.AlreadyExists);

            return validationResult;
        }

        public ValidationResponse ValidatorCreateEpaOrganisationContactRequest(
            CreateEpaOrganisationContactRequest request)
        {
            var validationResult = new ValidationResponse();
            RunValidationCheckAndAppendAnyError("Email", 
                CheckEmailPresentAndValid(request.Email), 
                validationResult, ValidationStatusCode.BadRequest);
            
            RunValidationCheckAndAppendAnyError("Email", 
                CheckEmailNotExists(request.Email), 
                validationResult, ValidationStatusCode.AlreadyExists);

            if (validationResult.IsValid)
            {
                RunValidationCheckAndAppendAnyError("ContactDetails",
                    CheckContactDetailsNotExists(request.FirstName, request.LastName, request.Email, request.PhoneNumber, null), 
                    validationResult, ValidationStatusCode.AlreadyExists);
            }

            RunValidationCheckAndAppendAnyError("EndPointAssessorOrganisationId",
                CheckOrganisationExists(request.EndPointAssessorOrganisationId), 
                validationResult, ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError("FirstName", 
                CheckFirstNameIsPresentAndMinimumLength(request.FirstName), 
                validationResult, ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError("LastName", 
                CheckLastNameIsPresentAndMininumLength(request.LastName), 
                validationResult, ValidationStatusCode.BadRequest);

            return validationResult;
        }

        public ValidationResponse ValidatorUpdateEpaOrganisationContactRequest(UpdateEpaOrganisationContactRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("Email", CheckEmailPresentAndValid(request.Email), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Email", CheckEmailNotExistsExcludingContact(request.Email, request.ContactId), validationResult, ValidationStatusCode.AlreadyExists);

            if (validationResult.IsValid)
                RunValidationCheckAndAppendAnyError("ContactDetails", CheckContactDetailsNotExists(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.ContactId), validationResult, ValidationStatusCode.AlreadyExists);

            RunValidationCheckAndAppendAnyError("ContactId", CheckContactIdExists(request.ContactId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("FirstName", CheckFirstNameIsPresentAndMinimumLength(request.FirstName), validationResult,
                ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("LastName", CheckLastNameIsPresentAndMininumLength(request.LastName), validationResult,
                ValidationStatusCode.BadRequest);
            return validationResult;
        }

        public ValidationResponse ValidatorCreateEpaOrganisationStandardRequest(
            CreateEpaOrganisationStandardRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("OrganisationId", CheckOrganisationExists(request.OrganisationId), validationResult, ValidationStatusCode.NotFound);
            if (!validationResult.IsValid) return validationResult;

            //  SV-658 / SV-659 Now that we have versions, this will check will include versions too.
            RunValidationCheckAndAppendAnyError("OrganisationId", CheckIfOrganisationStandardVersionAlreadyExists(request.OrganisationId, request.StandardCode, request.StandardVersions), validationResult, ValidationStatusCode.AlreadyExists);
            if (!validationResult.IsValid) return validationResult;

            var standard = GetStandard(request.StandardCode.ToString()).Result;
            if (standard is null)
            {
                var standardErrorMessage = FormatErrorMessage(EpaOrganisationValidatorMessageName.StandardNotFound);
                RunValidationCheckAndAppendAnyError("StandardCode", standardErrorMessage, validationResult, ValidationStatusCode.NotFound);
                return validationResult;
            }

            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckOrganisationStandardFromDateIsWithinStandardDateRanges(request.EffectiveFrom, standard.EffectiveFrom, standard.EffectiveTo, standard.LastDateStarts), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckEffectiveFromIsOnOrBeforeEffectiveTo(request.EffectiveFrom, request.EffectiveTo), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckOrganisationStandardEffectiveFromIsEntered(request.EffectiveFrom), validationResult, ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError("OrganisationId", CheckOrganisationIdIsPresentAndValid(request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("ContactId", CheckIfContactIdIsValid(request.ContactId, request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            
            RunValidationCheckAndAppendAnyError("EffectiveTo", CheckOrganisationStandardToDateIsWithinStandardDateRanges(request.EffectiveTo, standard.EffectiveFrom, standard.EffectiveTo), validationResult, ValidationStatusCode.BadRequest);
   
            return validationResult;
        }

        public ValidationResponse ValidatorUpdateEpaOrganisationStandardRequest(UpdateEpaOrganisationStandardRequest request)
        {
            var validationResult = new ValidationResponse();
            var standard = GetStandard(request.StandardCode.ToString()).Result;
            if (standard is null)
            {
                var standardErrorMessage = FormatErrorMessage(EpaOrganisationValidatorMessageName.StandardNotFound);
                RunValidationCheckAndAppendAnyError("StandardCode", standardErrorMessage, validationResult, ValidationStatusCode.NotFound);
                return validationResult;
            }

            RunValidationCheckAndAppendAnyError("OrganisationStandardId", CheckOrganisationStandardId(request.OrganisationStandardId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("OrganisationId", CheckIfOrganisationStandardDoesNotExist(request.OrganisationId, request.StandardCode), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("ContactId", CheckIfContactIdIsValid(request.ContactId,request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("DeliveryAreas", CheckIfDeliveryAreasAreValid(request.DeliveryAreas), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckOrganisationStandardFromDateIsWithinStandardDateRanges(request.EffectiveFrom, standard.EffectiveFrom, standard.EffectiveTo, standard.LastDateStarts), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckEffectiveFromIsOnOrBeforeEffectiveTo(request.EffectiveFrom, request.EffectiveTo), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveTo", CheckOrganisationStandardToDateIsWithinStandardDateRanges(request.EffectiveTo, standard.EffectiveFrom, standard.EffectiveTo), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckOrganisationStandardEffectiveFromIsEntered(request.EffectiveFrom), validationResult, ValidationStatusCode.BadRequest);   

            return validationResult;
        }

        public async Task<ValidationResponse> ValidatorUpdateOrganisationStandardVersionRequest(UpdateOrganisationStandardVersionRequest request)
        {
            var validationResult = new ValidationResponse();

            var organisationStandard = await _registerQueryRepository.GetOrganisationStandardFromOrganisationStandardId(request.OrganisationStandardId);
            
            if (organisationStandard is null)
            {
                var standardErrorMessage = FormatErrorMessage(EpaOrganisationValidatorMessageName.StandardNotFound);
                RunValidationCheckAndAppendAnyError("StandardCode", standardErrorMessage, validationResult, ValidationStatusCode.NotFound);
                return validationResult;
            }

            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckOrganisationStandardFromDateIsWithinStandardDateRanges(request.EffectiveFrom, organisationStandard.StandardEffectiveFrom, organisationStandard.StandardEffectiveTo, organisationStandard.StandardLastDateForNewStarts), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveTo", CheckOrganisationStandardVersionToDateIsWithinStandardDateRanges(request.EffectiveTo, organisationStandard.StandardEffectiveFrom, organisationStandard.StandardEffectiveTo), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveFrom", CheckVersionEffectiveFromIsOnOrBeforeEffectiveTo(request.EffectiveFrom, request.EffectiveTo), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("EffectiveTo", CheckVersionEffectiveToIsOnOrAfterEffectiveFrom(request.EffectiveTo, request.EffectiveFrom), validationResult, ValidationStatusCode.BadRequest);
            return validationResult;
        }

        public ValidationResponse ValidatorWithdrawOrganisationRequest(WithdrawOrganisationRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("EndPointAssessorOrganisationId",
               CheckOrganisationExists(request.EndPointAssessorOrganisationId), validationResult,
               ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError("ApplicationId",
              CheckApplicationdIdIsPresent(request.ApplicationId), validationResult,
              ValidationStatusCode.BadRequest);

            return validationResult;
        }

        public ValidationResponse ValidatorWithdrawStandardRequest(WithdrawStandardRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError(nameof(request.EndPointAssessorOrganisationId),
               CheckOrganisationExists(request.EndPointAssessorOrganisationId), validationResult,
               ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError(nameof(request.StandardCode),
              CheckIfOrganisationStandardDoesNotExist(request.EndPointAssessorOrganisationId, request.StandardCode), validationResult,
              ValidationStatusCode.BadRequest);

            return validationResult;
        }

        private string FormatErrorMessage(string messageName)
        {
            return $"{_localizer[messageName].Value}; ";
        }

        private void RunValidationCheckAndAppendAnyError(string fieldName, string errorMessage, ValidationResponse validationResult, ValidationStatusCode statusCode)
        {
            if (errorMessage != string.Empty)
                validationResult.Errors.Add(new ValidationErrorDetail(fieldName, errorMessage.Replace("; ", ""), statusCode));
        }

        public ValidationResponse ValidatorUpdateEpaOrganisationRequest(UpdateEpaOrganisationRequest request)
        {
            var validationResult = new ValidationResponse();
            var doLiveValidation = false || request.Status == "Live" || request.ActionChoice == "MakeLive";

            RunValidationCheckAndAppendAnyError("OrganisationId", CheckOrganisationExists(request.OrganisationId), validationResult, ValidationStatusCode.NotFound);
            if (!validationResult.IsValid)
                return validationResult;

            RunValidationCheckAndAppendAnyError("CompanyNumber", CheckCompanyNumberIsMissingOrValid(request.CompanyNumber), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("CompanyNumber", CheckIfOrganisationCompanyNumberExists(request.OrganisationId, request.CompanyNumber), validationResult, ValidationStatusCode.BadRequest);

            RunValidationCheckAndAppendAnyError("CharityNumber", CheckCharityNumberIsValid(request.CharityNumber), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("CharityNumber", CheckIfOrganisationCharityNumberExists(request.OrganisationId, request.CharityNumber), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("OrganisationId", CheckOrganisationIdIsPresentAndValid(request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameIsPresentAndValid(request.Name), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckIfOrganisationUkprnExistsForOtherOrganisations(request.Ukprn, request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameNotExistsExcludingOrganisation(request.Name, request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckUkprnIsValid(request.Ukprn), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("RecognitionNumber", CheckRecognitionNumberExists(request.RecognitionNumber), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("RecognitionNumber", CheckRecognitionNumberInUse(request.RecognitionNumber, request.OrganisationId), validationResult, ValidationStatusCode.AlreadyExists);

            if (!doLiveValidation)
            {
                RunValidationCheckAndAppendAnyError("OrganisationTypeId", CheckOrganisationTypeIsNullOrExists(request.OrganisationTypeId), validationResult, ValidationStatusCode.BadRequest);
            }
            else
            {
                var contacts = _registerQueryRepository.GetAssessmentOrganisationContacts(request.OrganisationId).Result;
                
                RunValidationCheckAndAppendAnyError("OrganisationTypeId", CheckOrganisationTypeExists(request.OrganisationTypeId), validationResult, ValidationStatusCode.BadRequest);
                RunValidationCheckAndAppendAnyError("Address", CheckAddressDetailsForOrganisation(request.Address1, request.Address2, request.Address3, request.Address4), validationResult, ValidationStatusCode.BadRequest);
                RunValidationCheckAndAppendAnyError("Postcode", CheckPostcodeIsPresentForOrganisation(request.Postcode), validationResult, ValidationStatusCode.BadRequest);
                RunValidationCheckAndAppendAnyError("Ukprn", CheckUkprnForOrganisation(request.Ukprn), validationResult, ValidationStatusCode.BadRequest);
                RunValidationCheckAndAppendAnyError("ContactsCount", CheckContactCountForOrganisation(contacts?.Count()), validationResult, ValidationStatusCode.BadRequest);                
            }

            return validationResult;
        }

        public ValidationResponse ValidatorSearchStandardsRequest(SearchStandardsValidationRequest request)
        {
            var validationResult = new ValidationResponse();
            RunValidationCheckAndAppendAnyError("StandardSearchString", CheckSearchStringForStandardsIsValid(request.Searchstring), validationResult, ValidationStatusCode.BadRequest);
            return validationResult;

        }
    }
}
