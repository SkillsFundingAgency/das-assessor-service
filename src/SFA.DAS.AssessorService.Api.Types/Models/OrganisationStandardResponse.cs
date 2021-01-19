using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class OrganisationStandardResponse : OrganisationResponse
    {
        public List<OrganisationStandardDeliveryArea> DeliveryAreasDetails { get; set; }
        public OrganisationStandard OrganisationStandard { get ; set ; }
    }
}