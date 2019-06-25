using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Roatp
{
    public class UpdateOrganisationApplicationDeterminedDateRequest
    {
     
        public DateTime ApplicationDeterminedDate { get; set; }
        
        public Guid OrganisationId { get; set; }
        public string LegalName { get; set; }
        public string UpdatedBy { get; set; }
    }
}
