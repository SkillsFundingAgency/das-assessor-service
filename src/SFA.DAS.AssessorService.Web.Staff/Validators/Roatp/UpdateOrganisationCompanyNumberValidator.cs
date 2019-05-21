using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Resources;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public class UpdateOrganisationCompanyNumberValidator : IUpdateOrganisationCompanyNumberValidator
    {
        private IRoatpApiClient _apiClient;

        public UpdateOrganisationCompanyNumberValidator(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public List<ValidationErrorDetail> IsDuplicateCompanyNumber(UpdateOrganisationCompanyNumberViewModel viewModel)
        {
            var errorMessages = new List<ValidationErrorDetail>();

         
            var duplicateCheckResponse = _apiClient.DuplicateCompanyNumberCheck(viewModel.OrganisationId, viewModel.CompanyNumber).Result;

            if (duplicateCheckResponse == null || !duplicateCheckResponse.DuplicateFound) return errorMessages;

            var duplicateErrorMessage = string.Format(RoatpOrganisationValidation.CompanyNumberDuplicateMatch,
                duplicateCheckResponse.DuplicateOrganisationName);
            errorMessages.Add(new ValidationErrorDetail("CompanyNumber", duplicateErrorMessage));
          

            return errorMessages;
        }
    }
}
