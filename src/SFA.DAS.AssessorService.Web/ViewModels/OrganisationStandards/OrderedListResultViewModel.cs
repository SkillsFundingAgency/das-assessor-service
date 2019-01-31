using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.ViewModels.OrganisationStandards
{
    public class OrderedListResultViewModel
    {
        public PaginatedList<EpaoPipelineStandardsResponse> Response { get; set; }

        public string OrderedBy { get; set; }
        public string OrderDirection { get; set; }
      
    }
}
