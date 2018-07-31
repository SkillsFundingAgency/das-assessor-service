using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AssessmentOrganisationDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Ukprn { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string OrganisationType { get; set; }
        public string OrganisationTypeId { get; set; }
        public AssessmentOrganisationAddress Address { get; set; }

        public string Status { get; set; }
    }
}
