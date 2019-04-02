using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CreateOrganisationAndContactFromApplyResponse
    {
        public bool IsEpaoApproved { get; set; }
        public List<string> WarningMessages { get; set; }
        public bool ApplySourceIsEpao { get; set; }
        public string EpaOrganisationId { get; set; }
    }
}
