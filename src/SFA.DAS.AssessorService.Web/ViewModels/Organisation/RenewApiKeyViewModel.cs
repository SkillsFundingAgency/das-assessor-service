using System;

namespace SFA.DAS.AssessorService.Web.ViewModels
{
    public class RenewApiKeyViewModel
    {
        public string CurrentKey { get; set; }
        public DateTime LastRenewedDate { get; set; }
        public string SubscriptionId { get; set; }
        public long LastRenewedTicks { get; set; }
    }
}
