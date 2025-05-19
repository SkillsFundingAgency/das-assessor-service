using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.Learner
{
    public class GetPipelinesCountHandler : IRequestHandler<GetPipelinesCountRequest, int>
    {
        private readonly IApiConfiguration _config;
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<GetPipelinesCountHandler> _logger;
        

        public GetPipelinesCountHandler(IApiConfiguration config, ILearnerRepository learnerRepository, ILogger<GetPipelinesCountHandler> logger)
        {
            _config = config;
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetPipelinesCountRequest request, CancellationToken cancellationToken)
        {
            if (request.StandardCode.HasValue)
            {
                _logger.LogInformation($"GetPipelinesCountHandler: EpaoId = {request.EpaoId}, StandardCode = {request.StandardCode}");
            }
            else
            {
                _logger.LogInformation($"GetPipelinesCountHandler: EpaoId = {request.EpaoId}");
            }
            
            return await _learnerRepository.GetEpaoPipelinesCount(request.EpaoId, request.StandardCode, _config.PipelineCutoff);
        }
    }
}