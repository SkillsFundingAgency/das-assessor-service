using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class UpdateStandardSummaryHandler : IRequestHandler<UpdateStandardSummaryRequest>
    {
        private readonly ILogger<UpdateStandardSummaryHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public UpdateStandardSummaryHandler(ILogger<UpdateStandardSummaryHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task Handle(UpdateStandardSummaryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Updating standard summary data");
            await _standardRepository.UpdateStandardSummary();
        }
    }
}
