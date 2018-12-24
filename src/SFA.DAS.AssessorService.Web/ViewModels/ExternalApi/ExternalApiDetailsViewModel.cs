using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.ViewModels.ExternalApi
{
    public class ExternalApiDetailsViewModel
    {
        public IEnumerable<AzureUser> PrimaryContacts { get; set; }

        public AzureUser LoggedInUser { get; set; }

        public IEnumerable<AzureSubscription> SubscriptionsToShow { get; set; }

    }
}
