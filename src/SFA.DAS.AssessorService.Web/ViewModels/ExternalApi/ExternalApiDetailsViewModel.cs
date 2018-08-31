using SFA.DAS.AssessorService.Api.Types.Models.Azure;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.ExternalApi
{
    public class ExternalApiDetailsViewModel
    {
        public AzureUser User { get; set; }
        public IEnumerable<AzureProduct> AvailableProducts { get; set; }
        public string SelectedProductId { get; set; }
    }
}
