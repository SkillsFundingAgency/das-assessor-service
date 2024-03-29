﻿using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
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

        public Domain.Entities.CompaniesHouseSummary CompanySummary { get; set; }
        public Domain.Entities.CharityCommissionSummary CharitySummary { get; set; }
    }
}
