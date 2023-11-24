using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryLastUpdatedHandler : IRequestHandler<GetAparSummaryLastUpdatedRequest, DateTime>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAparSummaryLastUpdatedHandler> _logger;

        public GetAparSummaryLastUpdatedHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAparSummaryLastUpdatedHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<DateTime> Handle(GetAparSummaryLastUpdatedRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAparSummaryLastUpdatedRequest request");

            return await _registerQueryRepository.GetAparSummaryLastUpdated();
        }
    }
}
