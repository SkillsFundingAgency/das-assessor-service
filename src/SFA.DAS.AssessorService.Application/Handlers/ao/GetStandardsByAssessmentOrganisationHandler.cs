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
    public class GetStandardsByAssessmentOrganisationHandler: IRequestHandler<GetStandardsByOrganisationRequest, List<OrganisationStandardSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetStandardsByAssessmentOrganisationHandler> _logger;

        public GetStandardsByAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetStandardsByAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<OrganisationStandardSummary>> Handle(GetStandardsByOrganisationRequest request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;
            _logger.LogInformation($@"Handling OrganisationStandards Request for OrganisationId [{organisationId}]");
            var orgStandards = await _registerQueryRepository.GetOrganisationStandardByOrganisationId(organisationId);
            var organisationStandardSummaries = orgStandards as OrganisationStandardSummary[] ?? orgStandards.ToArray();
            foreach (var orgStandard in organisationStandardSummaries)
            {
                var periods = await _registerQueryRepository.GetOrganisationStandardPeriodsByOrganisationStandard(orgStandard.OrganisationId, orgStandard.StandardCode);
                orgStandard.Periods = periods.ToList();
            }
            return organisationStandardSummaries.ToList();
        }
    }
}
