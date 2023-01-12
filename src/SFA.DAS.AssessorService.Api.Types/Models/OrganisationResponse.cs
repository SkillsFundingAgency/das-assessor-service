namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
    using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
    using System;

    public class OrganisationResponse
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string EndPointAssessorName { get; set; }
        public string PrimaryContact { get; set; }
        public bool RoATPApproved { get; set; }
        public bool RoEPAOApproved { get; set; }
        public string OrganisationType { get; set; }
        public string Status { get; set; }

        public string City { get; set; }
        public string Postcode { get; set; }

        public CompaniesHouseSummary CompanySummary { get; set; }
        public CharityCommissionSummary CharitySummary { get; set; }
    }
}
