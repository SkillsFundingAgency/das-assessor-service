using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class OrganisationContact
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public Guid OrganisationId { get; set; }
        
        public string Status { get; set; }
 
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
    }
}
