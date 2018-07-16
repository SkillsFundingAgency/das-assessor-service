using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class EpaOrganisation
    {
        public Guid Id { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorName {get; set;}
        public int OrganisationTypeId { get; set; }
        public string WebsiteLink { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public int? EndPointAssessorUkprn { get; set; }
        public string  LegalName { get; set; }
        public string Status { get; set; }

    }
}