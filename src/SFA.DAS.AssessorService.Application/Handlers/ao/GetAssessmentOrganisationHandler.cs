using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAssessmentOrganisationHandler : IRequestHandler<GetAssessmentOrganisationRequest, AssessmentOrganisationDetails>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationHandler> _logger;

        public GetAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<AssessmentOrganisationDetails> Handle(GetAssessmentOrganisationRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling AssessmentOrganisation Request for [{request.OrganisationId}]");
            var res = await _registerQueryRepository.GetAssessmentOrganisation(request.OrganisationId);
            return res;
        }
    }
}
