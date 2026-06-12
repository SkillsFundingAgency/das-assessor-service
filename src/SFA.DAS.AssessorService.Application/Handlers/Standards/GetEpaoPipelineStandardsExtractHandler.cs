using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsExtractHandler : IRequestHandler<EpaoPipelineStandardsExtractRequest, List<EpaoPipelineStandardsExtractResponse>>
    {
        private readonly IApiConfiguration _config;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<GetEpaoPipelineStandardsExtractHandler> _logger;

        public GetEpaoPipelineStandardsExtractHandler(IApiConfiguration config, IStandardRepository standardRepository, ILogger<GetEpaoPipelineStandardsExtractHandler> logger)
        {
            _config = config;
            _standardRepository = standardRepository;
            _logger = logger;
        }
        public async Task<List<EpaoPipelineStandardsExtractResponse>> Handle(EpaoPipelineStandardsExtractRequest request, CancellationToken cancellationToken)
        {
            var result = await _standardRepository.GetEpaoPipelineStandardsExtract(request.EpaoId, request.StandardFilterId, request.ProviderFilterId, request.EPADateFilterId, _config.PipelineCutoff);

            var response = result.Select(o =>
                new EpaoPipelineStandardsExtractResponse
                {
                    EstimatedDate = o.EstimateDate.UtcToTimeZoneTime().Date.ToString("MMMM yyyy"),
                    Pipeline = o.Pipeline,
                    StandardName = o.Title,
                    StandardVersion = o.Version,
                    ProviderUkPrn = o.ProviderUkPrn,
                    ProviderName = o.ProviderName.Replace("\"", "\"\"")
                }).ToList();

            return response;
        }
    }
}
