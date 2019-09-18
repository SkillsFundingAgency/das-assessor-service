using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetOppFinderFilterStandardsHandler : IRequestHandler<GetOppFinderFilterStandardsRequest, GetOppFinderFilterStandardsResponse>
    {
        private readonly ILogger<GetOppFinderFilterStandardsHandler> _logger;
        private readonly IStandardRepository _standardRepository;
        public GetOppFinderFilterStandardsHandler(ILogger<GetOppFinderFilterStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<GetOppFinderFilterStandardsResponse> Handle(GetOppFinderFilterStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiving filter standards results");
            var filterResult = await _standardRepository.GetOppFinderFilterStandards(request.SearchTerm, request.SectorFilters, request.LevelFilters);
            
            var sectorFilterResults = filterResult.MatchingSectorFilterResults
                .ToDictionary(key => key.Sector, value => value.MatchingSectorFilter);

            var levelFilterResults = filterResult.MatchingLevelFilterResults
                .ToDictionary(key => key.StandardLevel, value => value.MatchingLevelFilter);

            return new GetOppFinderFilterStandardsResponse
            {
                SectorFilterResults = sectorFilterResults,
                LevelFilterResults = levelFilterResults
            };
        }
    }
}
