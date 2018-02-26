﻿using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types
{
    public class Organisation
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }

        public List<Link> Links { get; set; }

        public string OrganisationType { get; set; }

        public int UkPrn { get; set; }
    }
}