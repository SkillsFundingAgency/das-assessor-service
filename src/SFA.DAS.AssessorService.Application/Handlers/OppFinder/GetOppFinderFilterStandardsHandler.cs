using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetOppFinderFilterStandardsHandler : IRequestHandler<GetOppFinderFilterStandardsRequest, GetOppFinderFilterStandardsResponse>
    {
        private readonly ILogger<GetOppFinderFilterStandardsHandler> _logger;
        private readonly IOppFinderRepository _oppFinderRepository;
        public GetOppFinderFilterStandardsHandler(ILogger<GetOppFinderFilterStandardsHandler> logger, IOppFinderRepository oppFinderRepository)
        {
            _logger = logger;
            _oppFinderRepository = oppFinderRepository;
        }

        public async Task<GetOppFinderFilterStandardsResponse> Handle(GetOppFinderFilterStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiving filter standards results");
            var filterResult = await _oppFinderRepository.GetOppFinderFilterStandards(request.SearchTerm, request.SectorFilters, request.LevelFilters);

            var sectorFilterResults = filterResult.MatchingSectorFilterResults
                .ToList()
                .ConvertAll(p => new OppFinderFilterResult { Category = p.Sector, Matches = p.MatchingSectorFilter });
                

            var levelFilterResults = filterResult.MatchingLevelFilterResults
                .ToList()
                .ConvertAll(p => new OppFinderFilterResult { Category = p.StandardLevel, Matches = p.MatchingLevelFilter });

            return new GetOppFinderFilterStandardsResponse
            {
                SectorFilterResults = sectorFilterResults,
                LevelFilterResults = levelFilterResults
            };
        }
    }
}
