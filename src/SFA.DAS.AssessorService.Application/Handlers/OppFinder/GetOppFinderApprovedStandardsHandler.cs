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
    public class GetOppFinderApprovedStandardsHandler : IRequestHandler<GetOppFinderApprovedStandardsRequest, GetOppFinderApprovedStandardsResponse>
    {
        private readonly ILogger<GetOppFinderApprovedStandardsHandler> _logger;
        private readonly IStandardRepository _standardRepository;
        public GetOppFinderApprovedStandardsHandler(ILogger<GetOppFinderApprovedStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<GetOppFinderApprovedStandardsResponse> Handle(GetOppFinderApprovedStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiving approved standards");
            var standardResult = await _standardRepository.GetOppFinderApprovedStandards(request.SearchTerm, request.SectorFilters, request.LevelFilters, request.SortColumn, request.SortAscending, request.PageSize, request.PageIndex ?? 1);

            var standards = standardResult.PageOfResults
                .ToList()
                .ConvertAll(p => Mapper.Map<OppFinderApprovedSearchResult>(p));

            return new GetOppFinderApprovedStandardsResponse
            {
                Standards = new PaginatedList<OppFinderApprovedSearchResult>(standards, standardResult.TotalCount, request.PageIndex ?? 1, request.PageSize, request.PageSetSize)
            };
        }
    }
}
