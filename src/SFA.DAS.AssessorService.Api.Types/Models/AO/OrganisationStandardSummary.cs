using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationStandardSummary
    {
        public string OrganisationId { get; set; }
        public int StandardCode { get; set; }
        public List<OrganisationStandardPeriod> Periods { get; set; }
        public List<DeliveryArea> DeliveryAreas { get; set; }
    }
}
