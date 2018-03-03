using System;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Domain.DomainModels
{ 
    public class ContactCreateDomainModel
    {      
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }

        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public string Status { get; } = ContactStatus.Live;
    }
}
