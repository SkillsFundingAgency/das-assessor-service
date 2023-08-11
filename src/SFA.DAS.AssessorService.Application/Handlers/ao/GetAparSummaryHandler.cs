using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryHandler : IRequestHandler<GetAparSummaryRequest, List<AparSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAparSummaryHandler> _logger;

        public GetAparSummaryHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAparSummaryHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<AparSummary>> Handle(GetAparSummaryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAPARSummary Request");
            var result = await _registerQueryRepository.GetAparSummary();
            return result.ToList();
        }
    }
}
