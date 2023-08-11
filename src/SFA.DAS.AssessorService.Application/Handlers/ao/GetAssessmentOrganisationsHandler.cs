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
    public class GetAssessmentOrganisationsHandler : IRequestHandler<GetAssessmentOrganisationsRequest, List<AssessmentOrganisationSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationsHandler> _logger;

        public GetAssessmentOrganisationsHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationsHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<AssessmentOrganisationSummary>> Handle(GetAssessmentOrganisationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AssessmentOrganisations Request");
            var result = await _registerQueryRepository.GetAssessmentOrganisations();
            return result.ToList();
        }
    }
}
