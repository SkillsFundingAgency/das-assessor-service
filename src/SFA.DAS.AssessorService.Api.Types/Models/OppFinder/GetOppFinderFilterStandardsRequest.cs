using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOppFinderFilterStandardsRequest : IRequest<GetOppFinderFilterStandardsResponse>
    {
        public string SearchTerm { get; set; }
        public string SectorFilters { get; set; }
        public string LevelFilters { get; set; }
    }
}
