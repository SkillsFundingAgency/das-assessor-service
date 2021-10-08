using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsFilterHandler : IRequestHandler<EpaoPipelineStandardsFiltersRequest, EpaoPipelineStandardsFiltersResponse>
    {
        private readonly ILogger<GetEpaoPipelineStandardsFilterHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public GetEpaoPipelineStandardsFilterHandler(ILogger<GetEpaoPipelineStandardsFilterHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<EpaoPipelineStandardsFiltersResponse> Handle(EpaoPipelineStandardsFiltersRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retreiving Epao pipeline standards filters.");

            EpaoPipelineStandardsFiltersResponse response = new EpaoPipelineStandardsFiltersResponse();

            var standards = await _standardRepository.GetEpaoPipelineStandardsStandardFilter(request.EpaoId);
            response.StandardFilterItems = standards;

            var providers = await _standardRepository.GetEpaoPipelineStandardsProviderFilter(request.EpaoId);
            response.ProviderFilterItems = providers;

            var dates = await _standardRepository.GetEpaoPipelineStandardsEPADateFilter(request.EpaoId);
            response.EPADateFilterItems = dates;

            return response;
        }
    }
}
