namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Infrastructure;
    using SFA.DAS.AssessorService.Api.Types.Models.Validation;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    public class AddOrganisationValidator : IAddOrganisationValidator
    {
        private const string CompaniesHouseNumberRegex = "[A-Za-z0-9]{2}[0-9]{6}";
        private const string CharityNumberInvalidCharactersRegex = "[^a-zA-Z0-9\\-]";
        
        private IRoatpApiClient _apiClient;

        public AddOrganisationValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<ValidationResponse> ValidateOrganisationDetails(AddOrganisationViewModel viewModel)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            var fieldValidationErrors = IsValidLegalName(viewModel.LegalName);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }

            fieldValidationErrors = IsValidUKPRN(viewModel.UKPRN);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }
            else
            {
                fieldValidationErrors = IsDuplicateUKPRN(viewModel.OrganisationId, viewModel.UKPRN);
                if (fieldValidationErrors.Any())
                {
                    validationResponse.Errors.AddRange(fieldValidationErrors);
                }
            }

            fieldValidationErrors = IsValidCompanyNumber(viewModel.CompanyNumber);
            if (fieldValidationErrors.Any())
            {         
                validationResponse.Errors.AddRange(fieldValidationErrors);    
            }
            else
            {
                fieldValidationErrors = IsDuplicateCompanyNumber(viewModel.OrganisationId, viewModel.CompanyNumber);
                if (fieldValidationErrors.Any())
                {
                    validationResponse.Errors.AddRange(fieldValidationErrors);
                }
            }

            fieldValidationErrors = IsValidCharityNumber(viewModel.CharityNumber);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }
            else
            {
                fieldValidationErrors = IsDuplicateCharityNumber(viewModel.OrganisationId, viewModel.CharityNumber);
                if (fieldValidationErrors.Any())
                {
                    validationResponse.Errors.AddRange(fieldValidationErrors);
                }
            }

            fieldValidationErrors = IsValidTradingName(viewModel.TradingName);
            if (fieldValidationErrors.Any())
            {
                validationResponse.Errors.AddRange(fieldValidationErrors);
            }

            return validationResponse;
        }
        
        private List<ValidationErrorDetail> IsValidUKPRN(string ukprn)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            bool isParsed = long.TryParse(ukprn, out ukprnValue);

            if (!isParsed)
            {
                errorMessages.Add(new ValidationErrorDetail("UKPRN", RoatpOrganisationValidation.UKPRNFormat));
            }

            if (ukprnValue < 10000000 || ukprnValue > 99999999)
            {
                errorMessages.Add(new ValidationErrorDetail("UKPRN", RoatpOrganisationValidation.UKPRNLength));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsDuplicateUKPRN(Guid organisationId, string ukprn)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            bool isParsed = long.TryParse(ukprn, out ukprnValue);

            var duplicateCheckResponse = _apiClient.DuplicateUKPRNCheck(organisationId, ukprnValue).Result;

            if (duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.UKPRNDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("UKPRN", duplicateErrorMessage));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsValidLegalName(string legalName)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(legalName))
            {
                errorMessages.Add(new ValidationErrorDetail("LegalName", RoatpOrganisationValidation.LegalNameMandatory));
                return errorMessages;
            }

            if (legalName.Length > 200)
            {
                errorMessages.Add(new ValidationErrorDetail("LegalName", RoatpOrganisationValidation.LegalNameMaxLength));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsValidTradingName(string tradingName)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(tradingName))
            {
                return errorMessages;
            }

            if (tradingName.Length > 200)
            {
                errorMessages.Add(new ValidationErrorDetail("TradingName", RoatpOrganisationValidation.TradingNameMaxLength));
            }

            return errorMessages;
        }
        
        private List<ValidationErrorDetail> IsValidCompanyNumber(string companyNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(companyNumber))
            {
                return errorMessages;
            }

            if (companyNumber.Length != 8)
            {
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", RoatpOrganisationValidation.CompanyNumberLength));
            }

            if (!Regex.IsMatch(companyNumber, CompaniesHouseNumberRegex))
            {
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", RoatpOrganisationValidation.CompanyNumberFormat));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsDuplicateCompanyNumber(Guid organisationId, string companyNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            var duplicateCheckResponse = _apiClient.DuplicateCompanyNumberCheck(organisationId, companyNumber).Result;

            if (duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CompanyNumberDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("CompanyNumber", duplicateErrorMessage));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsValidCharityNumber(string charityNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            if (String.IsNullOrWhiteSpace(charityNumber))
            {
                return errorMessages;
            }

            if (charityNumber.Length < 6 || charityNumber.Length > 14)
            {
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", RoatpOrganisationValidation.CharityNumberLength));
            }

            if (Regex.IsMatch(charityNumber, CharityNumberInvalidCharactersRegex))
            {
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", RoatpOrganisationValidation.CharityNumberFormat));
            }

            return errorMessages;
        }

        private List<ValidationErrorDetail> IsDuplicateCharityNumber(Guid organisationId, string charityNumber)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            var duplicateCheckResponse = _apiClient.DuplicateCharityNumberCheck(organisationId, charityNumber).Result;

            if (duplicateCheckResponse.DuplicateFound)
            {
                var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CharityNumberDuplicateMatch,
                    duplicateCheckResponse.DuplicateOrganisationName);
                errorMessages.Add(new ValidationErrorDetail("CharityNumber", duplicateErrorMessage));
            }

            return errorMessages;
        }

    }
}
