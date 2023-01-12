using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationStandardResponse : OrganisationResponse
    {
        public List<OrganisationStandardDeliveryArea> DeliveryAreasDetails { get; set; }
        public OrganisationStandard OrganisationStandard { get; set; }
    }
}