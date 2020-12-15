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
        private readonly IIlrRepository _ilrRepository;
        private readonly ILogger<GetPipelinesCountHandler> _logger;

        public GetPipelinesCountHandler(IIlrRepository ilrRepository, ILogger<GetPipelinesCountHandler> logger)
        {
            _ilrRepository = ilrRepository;
            _logger = logger;
        }

        public async Task<int> Handle(GetPipelinesCountRequest request, CancellationToken cancellationToken)
        {
            return await _ilrRepository.GetEpaoPipelinesCount(request.EndpointAssessmentOrganisationId, request.StandardCode);
        }
    }
}