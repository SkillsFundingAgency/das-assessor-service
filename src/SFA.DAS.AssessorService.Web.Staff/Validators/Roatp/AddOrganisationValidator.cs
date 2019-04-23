namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    public class AddOrganisationValidator : IAddOrganisationValidator
    {
        private IRoatpApiClient _apiClient;
        private IRoatpOrganisationValidator _organisationValidator;

        public AddOrganisationValidator(IRoatpApiClient apiClient, IRoatpOrganisationValidator organisationValidator)
        {
            _apiClient = apiClient;
            _organisationValidator = organisationValidator;
        }

        public async Task<ValidationResponse> ValidateOrganisationDetails(AddOrganisationViewModel viewModel)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var fieldValidationErrors = _organisationValidator.IsValidLegalName(viewModel.LegalName);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }

            fieldValidationErrors = _organisationValidator.IsValidUKPRN(viewModel.UKPRN);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(viewModel.UKPRN))
                {
                    fieldValidationErrors = IsDuplicateUKPRN(viewModel.OrganisationId, viewModel.UKPRN);
                    if (fieldValidationErrors.Any())
                    {
                        validationResponse.Errors.AddRange(fieldValidationErrors);
                    }
                }
            }

            fieldValidationErrors = _organisationValidator.IsValidCompanyNumber(viewModel.CompanyNumber);
            if (fieldValidationErrors.Any())
            {         
                validationResponse.Errors.AddRange(fieldValidationErrors);    
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(viewModel.CompanyNumber))
                {
                    fieldValidationErrors = IsDuplicateCompanyNumber(viewModel.OrganisationId, viewModel.CompanyNumber);
                    if (fieldValidationErrors.Any())
                    {
                        validationResponse.Errors.AddRange(fieldValidationErrors);
                    }
                }
            }

            fieldValidationErrors = _organisationValidator.IsValidCharityNumber(viewModel.CharityNumber);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(viewModel.CharityNumber))
                {
                    fieldValidationErrors = IsDuplicateCharityNumber(viewModel.OrganisationId, viewModel.CharityNumber);
                    if (fieldValidationErrors.Any())
                    {
                        validationResponse.Errors.AddRange(fieldValidationErrors);
                    }
                }
            }

            fieldValidationErrors = _organisationValidator.IsValidTradingName(viewModel.TradingName);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }

            return validationResponse;
        }
        
        private List<ValidationErrorDetail> IsDuplicateUKPRN(Guid organisationId, string ukprn)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            bool isParsed = long.TryParse(ukprn, out ukprnValue);

            var duplicateCheckResponse = _apiClient.DuplicateUKPRNCheck(organisationId, ukprnValue).Result;

            if (duplicateCheckResponse != null && duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.UKPRNDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("UKPRN", duplicateErrorMessage));
            }

            return errorMessages;
        }
           
        private List<ValidationErrorDetail> IsDuplicateCompanyNumber(Guid organisationId, string companyNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            var duplicateCheckResponse = _apiClient.DuplicateCompanyNumberCheck(organisationId, companyNumber).Result;

            if (duplicateCheckResponse != null && duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CompanyNumberDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", duplicateErrorMessage));
            }

            return errorMessages;
        }
        
        private List<ValidationErrorDetail> IsDuplicateCharityNumber(Guid organisationId, string charityNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            var duplicateCheckResponse = _apiClient.DuplicateCharityNumberCheck(organisationId, charityNumber).Result;

            if (duplicateCheckResponse != null && duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CharityNumberDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", duplicateErrorMessage));
            }

            return errorMessages;
        }

    }
}
