using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Domain.Paging;
using SFA.DAS.AssessorService.Settings;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsHandler : IRequestHandler<EpaoPipelineStandardsRequest, PaginatedList<EpaoPipelineStandardsResponse>>
    {
        private readonly IWebConfiguration _config;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<GetEpaoPipelineStandardsHandler> _logger;

        public GetEpaoPipelineStandardsHandler(IWebConfiguration config, IStandardRepository standardRepository, ILogger<GetEpaoPipelineStandardsHandler> logger)
        {
            _config = config;
            _standardRepository = standardRepository;
            _logger = logger;
        }

        public async Task<PaginatedList<EpaoPipelineStandardsResponse>> Handle(EpaoPipelineStandardsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"GetEpaoPipelineStandardsHandler: EpaoId = {request.EpaoId}, OrderBy = {request.OrderBy}, OrderDirection = {request.OrderDirection}, PageSize = {request.PageSize}, PageIndex = {request.PageIndex}");

            var result =
                await _standardRepository.GetEpaoPipelineStandards(request.EpaoId, request.StandardFilterId, request.ProviderFilterId, request.EPADateFilterId,
                    _config.PipelineCutoff, request.OrderBy, request.OrderDirection, request.PageSize, request.PageIndex);

            var epaoPipelinStandardsResult = result.PageOfResults.Select(o =>
                new EpaoPipelineStandardsResponse
                {
                    EstimatedDate = o.EstimateDate.UtcToTimeZoneTime().Date.ToString("MMMM yyyy"),
                    Pipeline = o.Pipeline,
                    StandardName = o.Title,
                    StandardCode = o.StdCode,
                    StandardVersion = o.Version,
                    TrainingProvider = o.ProviderName,
                    UKPRN = o.UKPRN
                }).ToList();

            return new PaginatedList<EpaoPipelineStandardsResponse>(epaoPipelinStandardsResult, result.TotalCount, request.PageIndex ?? 1, request.PageSize);
        }
    }
}
