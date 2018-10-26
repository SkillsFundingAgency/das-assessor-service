using System;
using System.Drawing;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Helpers
{
    public interface IRegisterValidator
    {
        ValidationResponse CheckDateIsEmptyOrValid(string day, string month, string year, 
            string dayFieldName, string monthFieldName, string yearFieldName, string dateFieldName, string dateFieldDescription);

        ValidationResponse CheckOrganisationStandardFromDateIsWithinStandardDateRanges(DateTime? effectiveFrom,
            DateTime standardEffectiveFrom, DateTime? standardEffectiveTo, DateTime? LastDateForNewStarts);

        ValidationResponse CheckEffectiveFromIsOnOrBeforeEffectiveTo(DateTime? effectiveFrom, DateTime? effectiveTo);
    }
}