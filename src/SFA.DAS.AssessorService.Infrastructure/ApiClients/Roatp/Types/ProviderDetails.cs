using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp.Types
{
    public class ProviderDetails
    {
        public string UKPRN { get; set; }
        public string ProviderName { get; set; }
        public string ProviderStatus { get; set; }
        public List<ProviderContact> ContactDetails { get; set; }
        public DateTime? VerificationDate { get; set; }
        public List<ProviderAlias> ProviderAliases { get; set; }
        public List<VerificationDetails> VerificationDetails { get; set; }
    }
}
