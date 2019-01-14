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
    public class EpaOrganisationStandardCountHandler: IRequestHandler<GetEpaOrganisationStandardsCountRequest, EpaOrganisationStandardsCountResponse>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public EpaOrganisationStandardCountHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }
        public async Task<EpaOrganisationStandardsCountResponse> Handle(GetEpaOrganisationStandardsCountRequest request, CancellationToken cancellationToken)
        {
            var count =  await _organisationQueryRepository.GetEpaOrganisationStandardsCount(request
                .EndPointAssessorOrganisationId);

            return new EpaOrganisationStandardsCountResponse(count);
        }
    }
}
