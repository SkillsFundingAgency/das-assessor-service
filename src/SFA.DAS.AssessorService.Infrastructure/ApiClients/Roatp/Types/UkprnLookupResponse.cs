using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}
