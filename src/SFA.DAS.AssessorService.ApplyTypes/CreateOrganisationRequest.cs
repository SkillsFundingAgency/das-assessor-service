using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class CreateOrganisationRequest
    {
        public string Name { get; set; }
        public string OrganisationType { get; set; }
        public int? OrganisationUkprn { get; set; }

        public bool RoEPAOApproved { get; set; }
        public bool RoATPApproved { get; set; }

        public OrganisationDetails OrganisationDetails { get; set; }

        public Guid CreatedBy { get; set; }
        public string PrimaryContactEmail { get; set; }
    }

}
