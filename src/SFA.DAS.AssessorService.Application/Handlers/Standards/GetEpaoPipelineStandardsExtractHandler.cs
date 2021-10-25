using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsExtractHandler : IRequestHandler<EpaoPipelineStandardsExtractRequest, List<EpaoPipelineStandardsExtractResponse>>
    {
        private readonly IWebConfiguration _config;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<GetEpaoPipelineStandardsExtractHandler> _logger;

        public GetEpaoPipelineStandardsExtractHandler(IWebConfiguration config, IStandardRepository standardRepository, ILogger<GetEpaoPipelineStandardsExtractHandler> logger)
        {
            _config = config;
            _standardRepository = standardRepository;
            _logger = logger;
        }
        public async Task<List<EpaoPipelineStandardsExtractResponse>> Handle(EpaoPipelineStandardsExtractRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"GetEpaoPipelineStandardsExtractHandler: EpaoId = {request.EpaoId}");
            
            var result = await _standardRepository.GetEpaoPipelineStandardsExtract(request.EpaoId, _config.PipelineCutoff);

            var response = result.Select(o =>
                new EpaoPipelineStandardsExtractResponse
                {
                    EstimatedDate = o.EstimateDate.UtcToTimeZoneTime().Date.ToString("MMMM yyyy"),
                    Pipeline = o.Pipeline,
                    StandardName = o.Title,
                    ProviderUkPrn = o.ProviderUkPrn
                }).ToList();

            return response;
        }
    }
}
