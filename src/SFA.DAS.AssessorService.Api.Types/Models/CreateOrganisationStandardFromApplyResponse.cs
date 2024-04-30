using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationStandardFromApplyResponse
    {
        public List<string> WarningMessages { get; set; }
        public string EpaoStandardId { get; set; }
    }
}
