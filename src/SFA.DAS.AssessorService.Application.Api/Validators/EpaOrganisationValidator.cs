using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation.Results;
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
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterQueryRepository _registerRepository;
        private readonly IStringLocalizer<EpaOrganisationValidator> _localizer;
        public string ErrorMessageOrganisationNameAlreadyPresent { get; } = "There is already an organisation present with this name; ";

        public EpaOrganisationValidator( IRegisterQueryRepository registerRepository, IStringLocalizer<EpaOrganisationValidator> localizer) 
        {
            _registerRepository = registerRepository;
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

        public string CheckOrganisationName(string organisationName)
        {
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
                ? ErrorMessageOrganisationNameAlreadyPresent : 
                string.Empty;
        }

        public string CheckOrganisationNameNotUsedForOtherOrganisations(string name, string organisationIdToIgnore)
        {
            return _registerRepository.EpaOrganisationAlreadyUsingName(name, organisationIdToIgnore).Result 
                ? ErrorMessageOrganisationNameAlreadyPresent : 
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

            return _registerRepository.ContactIdIsValidForOrganisationId(contactId, organisationId).Result
                ?string.Empty 
                : FormatErrorMessage(EpaOrganisationValidatorMessageName.ContactIdInvalidForOrganisationId);
        }

        public string CheckIfDisplayNameIsMissing(string displayName)
        {
            return string.IsNullOrEmpty(displayName?.Trim()) 
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameIsMissing) 
                : string.Empty;
        }

        private string FormatErrorMessage(string messageName)
        {
            return $"{_localizer[messageName].Value}; ";
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

        private void RunValidationCheckAndAppendAnyError(string fieldName, string errorMessage, ValidationResponse validationResult, ValidationStatusCode statusCode)
        {
            if (errorMessage != string.Empty)
                validationResult.Errors.Add(new ValidationErrorDetail(fieldName, errorMessage.Replace("; ",""), statusCode));
        }

        public ValidationResponse ValidatorCreateEpaOrganisationContactRequest(CreateOrganisationContactRequest request)
        {
            var validationResult = new ValidationResponse();
            RunValidationCheckAndAppendAnyError("EndPointAssessorOrganisationId", CheckIfOrganisationNotFound(request.EndPointAssessorOrganisationId), validationResult, ValidationStatusCode.BadRequest);
            RunValidationCheckAndAppendAnyError("DisplayName", CheckIfDisplayNameIsMissing(request.DisplayName), validationResult, ValidationStatusCode.BadRequest);

            return validationResult;
        }

     
    }
}
