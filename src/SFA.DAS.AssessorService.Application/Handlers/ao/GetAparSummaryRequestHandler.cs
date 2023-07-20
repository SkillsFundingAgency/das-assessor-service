using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAparSummaryRequestHandler : IRequestHandler<GetAparSummaryRequest, List<AssessmentOrganisationListSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAparSummaryRequestHandler> _logger;

        public GetAparSummaryRequestHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAparSummaryRequestHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<AssessmentOrganisationListSummary>> Handle(GetAparSummaryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAPARSummary Request");
            var result = await _registerQueryRepository.GetAparSummary();
            return result.ToList();
        }
    }
}
