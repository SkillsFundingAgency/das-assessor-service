using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Services.Roatp
{
    public interface IApplicationDeterminedDateValidationService
    {
        ValidationResponse ValidateApplicationDeterminedDate(int? day, int? month, int? year);
    }
}
