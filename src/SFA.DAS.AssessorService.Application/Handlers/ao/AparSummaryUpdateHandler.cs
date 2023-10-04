using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class AparSummaryUpdateHandler : IRequestHandler<AparSummaryUpdateRequest, int?>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<AparSummaryUpdateHandler> _logger;

        public AparSummaryUpdateHandler(IRegisterQueryRepository registerQueryRepository, ILogger<AparSummaryUpdateHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<int?> Handle(AparSummaryUpdateRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AparSummaryUpdateRequest");

            return await _registerQueryRepository.AparSummaryUpdate();
        }
    }
}
