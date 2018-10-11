﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
using Swashbuckle.AspNetCore.Swagger;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationValidator: IEpaOrganisationValidator
    {
        private readonly IRegisterValidationRepository _registerRepository;
        private readonly IStringLocalizer<EpaOrganisationValidator> _localizer;
        public string ErrorMessageOrganisationNameAlreadyPresent { get; } = "There is already an organisation present with this name; ";

        public EpaOrganisationValidator( IRegisterValidationRepository registerRepository, IStringLocalizer<EpaOrganisationValidator> localizer) 
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

        public string CheckDisplayName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Trim().Length == 0)
                return FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameIsMissing);

            return name.Trim().Length < 2
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.DisplayNameTooShort)
                : string.Empty;
        }

        public string CheckIfEmailIsMissing(string emailName)
        {
            return string.IsNullOrEmpty(emailName?.Trim())
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailIsMissing)
                : string.Empty;
        }

        public string CheckIfEmailAlreadyPresentInAnotherOrganisation(string email, string organisationId)
        {
            return _registerRepository.EmailAlreadyPresentInAnotherOrganisation(email, organisationId).Result
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailAlreadyPresentInAnotherOrganisation)
                : string.Empty;
        }


        public string CheckIfEmailIsSuitableFormat(string email)
        {
            var regex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            var match = regex.Match(email);
            return !match.Success
                ? FormatErrorMessage(EpaOrganisationValidatorMessageName.EmailIsIncorrectFormat)
                : string.Empty;
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

            var emailMissing = CheckIfEmailIsMissing(request.Email);
            RunValidationCheckAndAppendAnyError("Email", emailMissing, validationResult, ValidationStatusCode.BadRequest);
            if (emailMissing ==string.Empty)
                RunValidationCheckAndAppendAnyError("Email", CheckIfEmailIsSuitableFormat(request.Email), validationResult, ValidationStatusCode.AlreadyExists);

            RunValidationCheckAndAppendAnyError("Email", CheckIfEmailAlreadyPresentInAnotherOrganisation(request.Email, request.EndPointAssessorOrganisationId), validationResult, ValidationStatusCode.AlreadyExists);

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
    }
}
