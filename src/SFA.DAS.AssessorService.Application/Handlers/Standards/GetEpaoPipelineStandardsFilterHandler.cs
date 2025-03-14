using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsFilterHandler : IRequestHandler<EpaoPipelineStandardsFiltersRequest, EpaoPipelineStandardsFiltersResponse>
    {
        private readonly IApiConfiguration _config;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<GetEpaoPipelineStandardsFilterHandler> _logger;
        
        public GetEpaoPipelineStandardsFilterHandler(IApiConfiguration config, IStandardRepository standardRepository, ILogger<GetEpaoPipelineStandardsFilterHandler> logger)
        {
            _config = config;
            _standardRepository = standardRepository;
            _logger = logger;
        }

        public async Task<EpaoPipelineStandardsFiltersResponse> Handle(EpaoPipelineStandardsFiltersRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"GetEpaoPipelineStandardsFilterHandler: EpaoId = {request.EpaoId}");
            
            EpaoPipelineStandardsFiltersResponse response = new EpaoPipelineStandardsFiltersResponse();

            var standards = await _standardRepository.GetEpaoPipelineStandardsStandardFilter(request.EpaoId, _config.PipelineCutoff);
            response.StandardFilterItems = standards;

            var providers = await _standardRepository.GetEpaoPipelineStandardsProviderFilter(request.EpaoId, _config.PipelineCutoff);
            response.ProviderFilterItems = providers;

            var dates = await _standardRepository.GetEpaoPipelineStandardsEPADateFilter(request.EpaoId, _config.PipelineCutoff);
            response.EPADateFilterItems = dates;

            return response;
        }
    }
}
