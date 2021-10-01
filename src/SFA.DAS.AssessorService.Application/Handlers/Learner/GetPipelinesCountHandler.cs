using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Learner
{
    public class GetPipelinesCountHandler : IRequestHandler<GetPipelinesCountRequest, int>
    {
        private readonly ILearnerRepository _learnerRepository;
        private readonly ILogger<GetPipelinesCountHandler> _logger;

        public GetPipelinesCountHandler(ILearnerRepository learnerRepository, ILogger<GetPipelinesCountHandler> logger)
        {
            _learnerRepository = learnerRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetPipelinesCountRequest request, CancellationToken cancellationToken)
        {
            return await _learnerRepository.GetEpaoPipelinesCount(request.EndpointAssessmentOrganisationId, request.StandardCode);
        }
    }
}