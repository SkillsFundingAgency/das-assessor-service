using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.models
{
    public class EpaOrganisation
    {
        public int Id { get; set; }
        public string EpaOrganisationIdentifier { get; set; }
        public string EpaOrganisationName {get; set;}
        public int OrganisationTypeId { get; set; }
        public string WebsiteLink { get; set; }
        public string ContactAddress1 { get; set; }
        public string ContactAddress2 { get; set; }
        public string ContactAddress3 { get; set; }
        public string ContactAddress4 { get; set; }
        public string ContactPostcode { get; set; }
        public int? Ukprn { get; set; }
        public string  LegalName { get; set; }
        public int StatusId { get; set; }
    }
}