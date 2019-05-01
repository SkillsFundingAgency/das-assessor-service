

using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationUkprnValidator : IUpdateOrganisationUkprnValidator
    {
        private IRoatpApiClient _apiClient;
  
        public UpdateOrganisationUkprnValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ValidationErrorDetail> IsDuplicateUkprn(UpdateOrganisationUkprnViewModel viewModel)
        {
            var errorMessages = new List<ValidationErrorDetail>();

            long ukprnValue = 0;
            var isParsed = long.TryParse(viewModel.Ukprn, out ukprnValue);

            var duplicateCheckResponse = _apiClient.DuplicateUKPRNCheck(viewModel.OrganisationId, ukprnValue).Result;

            if (duplicateCheckResponse == null || !duplicateCheckResponse.DuplicateFound) return errorMessages;

            var duplicateErrorMessage =
                $"This is an existing UKPRN for '{duplicateCheckResponse.DuplicateOrganisationName}'";
            errorMessages.Add(new ValidationErrorDetail("UKPRN", duplicateErrorMessage));

            return errorMessages;
        }
    }
}
