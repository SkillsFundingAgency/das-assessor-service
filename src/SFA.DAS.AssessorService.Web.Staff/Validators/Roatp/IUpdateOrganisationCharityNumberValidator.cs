using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public interface IUpdateOrganisationCharityNumberValidator
    {
        List<ValidationErrorDetail> IsDuplicateCharityNumber(UpdateOrganisationCharityNumberViewModel viewModel);
    }
}
