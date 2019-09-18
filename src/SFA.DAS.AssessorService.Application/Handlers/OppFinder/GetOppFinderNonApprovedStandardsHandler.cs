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
    public class GetOppFinderNonApprovedStandardsHandler : IRequestHandler<GetOppFinderNonApprovedStandardsRequest, GetOppFinderNonApprovedStandardsResponse>
    {
        private readonly ILogger<GetOppFinderNonApprovedStandardsHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public GetOppFinderNonApprovedStandardsHandler(ILogger<GetOppFinderNonApprovedStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<GetOppFinderNonApprovedStandardsResponse> Handle(GetOppFinderNonApprovedStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retreiving non approved standards: {request.NonApprovedType}");
            var result = await _standardRepository.GetOppFinderNonApprovedStandards(request.SearchTerm, request.SortColumn, request.SortAscending, 
                request.PageSize, request.PageIndex ?? 1, request.NonApprovedType);

            var standards = result.PageOfResults
                .ToList()
                .ConvertAll(p => Mapper.Map<OppFinderSearchResult>(p));

            return new GetOppFinderNonApprovedStandardsResponse
            {
                Standards = new PaginatedList<OppFinderSearchResult>(standards, result.TotalCount, request.PageIndex ?? 1, request.PageSize, request.PageSetSize)
            };
        }
    }
}
