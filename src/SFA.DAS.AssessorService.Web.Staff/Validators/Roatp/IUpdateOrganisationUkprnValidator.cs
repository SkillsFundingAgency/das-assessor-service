using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    public interface IUpdateOrganisationUkprnValidator
    {
        List<ValidationErrorDetail> IsDuplicateUkprn(UpdateOrganisationUkprnViewModel viewModel);
    }
}
