using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using StructureMap.Diagnostics;
using Swashbuckle.AspNetCore.Swagger;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterValidationRepository _registerRepository;
        private readonly IStringLocalizer<EpaOrganisationValidator> _localizer;
        private readonly ISpecialCharacterCleanserService _cleanserService;
       
        public EpaOrganisationValidator( IRegisterValidationRepository registerRepository, ISpecialCharacterCleanserService cleanserService, IStringLocalizer<EpaOrganisationValidator> localizer) 
        {
            _registerRepository = registerRepository;
            _cleanserService = cleanserService;
            _localizer = localizer;
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

        public string CheckOrganisationName(string name)
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
            if (organisationId == null ||
                !_registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result) return string.Empty;
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationIdAlreadyUsed);
        }

        public string CheckIfOrganisationUkprnExists(long? ukprn)
        {
            if (ukprn == null || !_registerRepository.EpaOrganisationExistsWithUkprn(ukprn.Value).Result) return string.Empty;
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnAlreadyUsed);
        }

        public string CheckOrganisationTypeIsNullOrExists(int? organisationTypeId)
        {
            if (organisationTypeId == null || _registerRepository.OrganisationTypeExists(organisationTypeId.Value).Result) return string.Empty;
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationTypeIsInvalid);
        }

        public string CheckUkprnIsValid(long? ukprn)
        {
            if (ukprn == null) return string.Empty;
            var isValid = ukprn >= 10000000 && ukprn <= 99999999;
            return isValid ? string.Empty : FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnIsInvalid);
        }

        public string CheckOrganisationNameNotUsed(string name)
        {
            return _registerRepository.EpaOrganisationAlreadyUsingName(name, string.Empty).Result 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent) : 
                string.Empty;
        }

        public string CheckOrganisationNameNotUsedForOtherOrganisations(string name, string organisationIdToIgnore)
        {
            return _registerRepository.EpaOrganisationAlreadyUsingName(name, organisationIdToIgnore).Result 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.ErrorMessageOrganisationNameAlreadyPresent) : 
                string.Empty;
        }

        public string CheckIfOrganisationUkprnExistsForOtherOrganisations(long? ukprn, string organisationIdToIgnore)
        {
        if (ukprn == null || !_registerRepository.EpaOrganisationAlreadyUsingUkprn(ukprn.Value, organisationIdToIgnore).Result) return string.Empty;
            return FormatErrorMessage(EpaOrganisationValidatorMessageName.UkprnAlreadyUsed);
        }

        public string CheckIfOrganisationNotFound(string organisationId)
        {
            return organisationId != null && _registerRepository.EpaOrganisationExistsWithOrganisationId(organisationId).Result 
                ? string.Empty :
                FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationNotFound);
        }

        public string CheckIfStandardNotFound(int standardCode)
        {
            var apiClient = new AssessmentOrgsApiClient();

            try
            {
                var res = apiClient.GetStandard(standardCode).Result;
                return string.Empty;
            }
            catch
            {
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.StandardNotFound);
            }
        }

        public string CheckIfOrganisationStandardAlreadyExists(string organisationId, int standardCode)
        {
            return _registerRepository.EpaOrganisationStandardExists(organisationId, standardCode).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardAlreadyExists)
                : string.Empty;
        }

        public string CheckIfOrganisationStandardDoesNotExist(string organisationId, int standardCode)
        {
            return _registerRepository.EpaOrganisationStandardExists(organisationId, standardCode).Result
                ? string.Empty
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.OrganisationStandardDoesNotExist);
        }
        
        public string CheckIfContactIdIsEmptyOrValid(string contactId, string organisationId)
        {
            if (string.IsNullOrEmpty(contactId)) return string.Empty;

            return Guid.TryParse(contactId, out Guid newContactId) && _registerRepository.ContactIdIsValidForOrganisationId(newContactId, organisationId).Result
                ? string.Empty 
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId);
        }

        public string CheckDisplayName(string name)
        {
            var newName = _cleanserService.CleanseStringForSpecialCharacters(name);
            if (string.IsNullOrEmpty(newName) || newName.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameIsMissing);

            return newName.Trim().Length < 2
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameTooShort)
                : string.Empty;
        }

        public string CheckIfEmailIsMissing(string emailName)
        {
            return string.IsNullOrEmpty(emailName?.Trim())
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailIsMissing)
                : string.Empty;
        }


        public string CheckContactIdExists(string contactId)
        {

            if (!Guid.TryParse(contactId, out Guid newContactId))
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdDoesntExist);
            
            return _registerRepository.ContactExists(newContactId).Result
                ? string.Empty
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdDoesntExist);
        }

        public string CheckIfEmailAlreadyPresentInAnotherOrganisation(string email, string organisationId)
        {
            return _registerRepository.EmailAlreadyPresentInAnotherOrganisation(email, organisationId).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailAlreadyPresentInAnotherOrganisation)
                : string.Empty;
        }

        public string CheckIfEmailAlreadyPresentInOrganisationNotAssociatedWithContact(string email, string contactId)
        {
            if (!Guid.TryParse(contactId, out Guid newContactId))
                return string.Empty;

            return _registerRepository
                .EmailAlreadyPresentInAnOrganisationNotAssociatedWithContact(email, newContactId).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailAlreadyPresentInAnotherOrganisation)
                : string.Empty;
        }

        public string CheckIfEmailIsPresentAndInSuitableFormat(string email)
        {
            var validationResults = new EmailValidator().Validate(new EmailChecker {EmailToCheck = email});
            return validationResults.IsValid ? string.Empty : FormatErrorMessage(validationResults.Errors.First().ErrorMessage);
        }

        public ValidationResponse ValidatorCreateEpaOrganisationRequest(CreateEpaOrganisationRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationName(request.Name), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("OrganisationTypeId", CheckOrganisationTypeIsNullOrExists(request.OrganisationTypeId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckUkprnIsValid(request.Ukprn), validationResult, ValidationStatusCode.BadRequest);    
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameNotUsed(request.Name), validationResult, ValidationStatusCode.AlreadyExists);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckIfOrganisationUkprnExists(request.Ukprn), validationResult, ValidationStatusCode.AlreadyExists);

            return validationResult;
        }

        public ValidationResponse ValidatorCreateEpaOrganisationContactRequest(CreateEpaOrganisationContactRequest request)
        {
            var validationResult = new ValidationResponse();
            RunValidationCheckAndAppendAnyError("EndPointAssessorOrganisationId", CheckIfOrganisationNotFound(request.EndPointAssessorOrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("DisplayName", CheckDisplayName(request.DisplayName), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Email", CheckIfEmailIsPresentAndInSuitableFormat(request.Email), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Email", CheckIfEmailAlreadyPresentInAnotherOrganisation(request.Email, request.EndPointAssessorOrganisationId), validationResult, ValidationStatusCode.AlreadyExists);
            return validationResult;
        }

        public ValidationResponse ValidatorUpdateEpaOrganisationContactRequest(UpdateEpaOrganisationContactRequest request)
        {
            var validationResult = new ValidationResponse();

            RunValidationCheckAndAppendAnyError("ContactId", CheckContactIdExists(request.ContactId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("DisplayName", CheckDisplayName(request.DisplayName), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Email", CheckIfEmailIsPresentAndInSuitableFormat(request.Email), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Email", CheckIfEmailAlreadyPresentInOrganisationNotAssociatedWithContact(request.Email, request.ContactId), validationResult, ValidationStatusCode.AlreadyExists);
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
            RunValidationCheckAndAppendAnyError("OrganisationId", CheckIfOrganisationNotFound(request.OrganisationId), validationResult, ValidationStatusCode.NotFound);
            if (!validationResult.IsValid)
                return validationResult;

            RunValidationCheckAndAppendAnyError("OrganisationId", CheckOrganisationIdIsPresentAndValid(request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationName(request.Name), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("OrganisationTypeId", CheckOrganisationTypeIsNullOrExists(request.OrganisationTypeId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckIfOrganisationUkprnExistsForOtherOrganisations(request.Ukprn, request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Name", CheckOrganisationNameNotUsedForOtherOrganisations(request.Name, request.OrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("Ukprn", CheckUkprnIsValid(request.Ukprn), validationResult, ValidationStatusCode.BadRequest);

            return validationResult;

        }
    }
}
