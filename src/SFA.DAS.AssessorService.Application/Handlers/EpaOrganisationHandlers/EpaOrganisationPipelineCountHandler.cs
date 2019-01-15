using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class EpaOrganisationPipelineCountHandler: IRequestHandler<GetEpaOrganisationPipelineCountRequest, EpaOrganisationPipelineCountResponse>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public EpaOrganisationPipelineCountHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }
        public async Task<EpaOrganisationPipelineCountResponse> Handle(GetEpaOrganisationPipelineCountRequest request, CancellationToken cancellationToken)
        {
            var count =  await _organisationQueryRepository.GetEpaoPipelineCount(request
                .EndPointAssessorOrganisationId);

            return new EpaOrganisationPipelineCountResponse(count);
        }
    }
}
