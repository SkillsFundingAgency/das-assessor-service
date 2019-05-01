using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoPipelineStandardsExtractHandler : IRequestHandler<EpaoPipelineStandardsExtractRequest, List<EpaoPipelineStandardsExtractResponse>>
    {
        private readonly ILogger<GetEpaoPipelineStandardsExtractHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public GetEpaoPipelineStandardsExtractHandler(ILogger<GetEpaoPipelineStandardsExtractHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }
        public async Task<List<EpaoPipelineStandardsExtractResponse>> Handle(EpaoPipelineStandardsExtractRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Extracting Epao pipeline information");
            var result = await _standardRepository.GetEpaoPipelineStandardsExtract(request.EpaoId);

            var response = result.Select(o =>
                new EpaoPipelineStandardsExtractResponse
                {
                    EstimatedDate = o.EstimateDate.UtcToTimeZoneTime().Date.ToString("MMM yyyy"),
                    Pipeline = o.Pipeline,
                    StandardName= o.Title,
                    ProviderUkPrn = o.ProviderUkPrn
                }).ToList();

            return response;
        }
    }
}
