using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffSearchResult
    {
        public string EndpointAssessorOrganisationId { get; set; }
        public bool DisplayEpao { get; set; }
        public PaginatedList<StaffSearchItems> StaffSearchItems { get; set; }
    }
}
