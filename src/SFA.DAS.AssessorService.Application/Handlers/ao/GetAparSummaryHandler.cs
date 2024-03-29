﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

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
            _logger.LogInformation("Handling GetAparSummaryByUkprnRequest");
            var result = await _registerQueryRepository.GetAparSummary(request.Ukprn);
            return result.ToList();
        }
    }
}
