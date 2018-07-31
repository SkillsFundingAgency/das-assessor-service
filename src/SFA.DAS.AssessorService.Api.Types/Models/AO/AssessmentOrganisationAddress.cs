using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class AssessmentOrganisationAddress
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public string Street { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
    }
}
