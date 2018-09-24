using SFA.DAS.AssessorService.Application.Interfaces;
using System.Text.RegularExpressions;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class EpaOrganisationSearchValidator : IEpaOrganisationSearchValidator
    {
        public bool IsValidEpaOrganisationId(string organisationIdToCheck)
        {
            var regex = new Regex(@"[eE][pP][aA][0-9]{4,9}$");
            return regex.Match(organisationIdToCheck).Success;
        }

        public bool IsValidUkprn(string stringToCheck)
        {
            var isValid = int.TryParse(stringToCheck, out int ukprn);
            if (!isValid) return false;

            isValid = ukprn >= 10000000 && ukprn <= 99999999;

            return isValid;
        }
    }
}
