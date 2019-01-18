using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsHandler : IRequestHandler<GetEpaoPipelineStandardsRequest, PaginatedList<GetEpaoPipelineStandardsResponse>>
    {
        private readonly ILogger<GetEpaoPipelineStandardsHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public GetEpaoPipelineStandardsHandler(ILogger<GetEpaoPipelineStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }
        public async Task<PaginatedList<GetEpaoPipelineStandardsResponse>> Handle(GetEpaoPipelineStandardsRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 10;
            _logger.LogInformation("Retreiving Epao pipeline information");
            var result =
                await _standardRepository.GetEpaoPipelineStandards(request.EpaoId, pageSize,
                    request.PageIndex);
            var epaoPipelinStandardsResult = result.PageOfResults.Select(o =>
                new GetEpaoPipelineStandardsResponse
                {
                    EstimatedDate = o.EstimateDate.UtcToTimeZoneTime().Date.ToString("MMM yyyy"),
                    Pipeline = o.Pipeline,
                    StandardName= o.Title
                    
                }).ToList();

            return new PaginatedList<GetEpaoPipelineStandardsResponse>(epaoPipelinStandardsResult, result.TotalCount, request.PageIndex ?? 1, pageSize);
        }
    }
}
