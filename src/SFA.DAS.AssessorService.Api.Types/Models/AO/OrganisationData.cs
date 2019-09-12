using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class OrganisationData
    {
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string WebsiteLink { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string ProviderName { get; set; }
        public string City { get; set; }
        public string OrganisationReferenceType { get; set; } // "RoEPAO", "RoATP" or "EASAPI"
        public string OrganisationReferenceId { get; set; } // CSV list of known id's
        public bool RoATPApproved { get; set; }
        public bool RoEPAOApproved { get; set; }
        public string EndPointAssessmentOrgId { get; set; }
        public List<FinancialGrade> FinancialGrades { get; set; }
        public FHADetails FHADetails { get; set; }
    }

    public class FHADetails
    {
        public DateTime? FinancialDueDate { get; set; }
        public bool? FinancialExempt { get; set; }
    }

}
