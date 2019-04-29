using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationStandardFromApplyResponse
    {
        public List<string> WarningMessages { get; set; }
        public string EpaoStandardId { get; set; }
    }
}
