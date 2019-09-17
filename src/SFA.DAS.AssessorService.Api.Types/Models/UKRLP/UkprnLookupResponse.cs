using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.UKRLP
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}
