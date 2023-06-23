using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationsStandardsSummary
    {
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public string EndPointAssessorUkprn { get; set; }
        public DateTime EarliestDateStandardApprovedOnRegister { get; set; }
        public DateTime EarliestStandardEffectiveFromDate { get; set; }
    }
}
