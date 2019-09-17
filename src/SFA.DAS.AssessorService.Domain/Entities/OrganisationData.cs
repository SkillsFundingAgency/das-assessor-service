using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Domain.Entities
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
        public string City { get; set; }
        public string Postcode { get; set; }
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public string ProviderName { get; set; }
        public bool RoATPApproved { get; set; }
        public bool RoEPAOApproved { get; set; }
        public string OrganisationReferenceType { get; set; }
        public string OrganisationReferenceId { get; set; }
        public List<FinancialGrade> FinancialGrades { get; set; }
        public FHADetails FHADetails { get; set; }
    }
}
