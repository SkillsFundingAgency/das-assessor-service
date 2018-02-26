using System.Collections.Generic;

namespace SFA.DAS.AssessmentOrgs.Api.Client.Core.Types
{
    public class StandardOrganisationSummary
    {
        public string StandardCode { get; set; }

        public string Uri { get; set; }

        public IEnumerable<Period> Periods { get; set; }
    }
}