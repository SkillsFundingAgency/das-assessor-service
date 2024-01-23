using System;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types
{
    public class ProviderContact
    {
        public string ContactType { get; set; }
        public ContactAddress ContactAddress { get; set; }
        public ContactPersonalDetails ContactPersonalDetails { get; set; }
        public string ContactRole { get; set; }
        public string ContactTelephone1 { get; set; }
        public string ContactTelephone2 { get; set; }
        public string ContactWebsiteAddress { get; set; }
        public string ContactEmail { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
