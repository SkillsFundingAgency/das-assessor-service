using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryUpdateRequestHandler : IRequestHandler<UpdateAparSummaryRequest, int?>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAparSummaryUpdateRequestHandler> _logger;

        public GetAparSummaryUpdateRequestHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAparSummaryUpdateRequestHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<int?> Handle(UpdateAparSummaryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateAparSummaryRequest request");

            return await _registerQueryRepository.AparSummaryUpdate();
        }
    }
}
