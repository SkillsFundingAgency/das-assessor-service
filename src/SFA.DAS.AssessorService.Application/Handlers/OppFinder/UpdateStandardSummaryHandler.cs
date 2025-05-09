﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class UpdateStandardSummaryHandler : IRequestHandler<UpdateStandardSummaryRequest, Unit>
    {
        private readonly ILogger<UpdateStandardSummaryHandler> _logger;
        private readonly IOppFinderRepository _oppFinderRepository;

        public UpdateStandardSummaryHandler(ILogger<UpdateStandardSummaryHandler> logger, IOppFinderRepository oppFinderRepository)
        {
            _logger = logger;
            _oppFinderRepository = oppFinderRepository;
        }

        public async Task<Unit> Handle(UpdateStandardSummaryRequest request, CancellationToken cancellationToken)
        {
            await _oppFinderRepository.UpdateStandardSummary();
			return Unit.Value;
		}
    }
}
