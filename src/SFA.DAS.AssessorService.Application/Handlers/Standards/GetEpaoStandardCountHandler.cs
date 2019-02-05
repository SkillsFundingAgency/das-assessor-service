using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoStandardCountHandler: IRequestHandler<GetEpaoStandardsCountRequest, EpaoStandardsCountResponse>
    {
        private readonly IStandardRepository _standardRepository;

        public GetEpaoStandardCountHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }
        public async Task<EpaoStandardsCountResponse> Handle(GetEpaoStandardsCountRequest request, CancellationToken cancellationToken)
        {
            var count =  await _standardRepository.GetEpaoStandardsCount(request
                .EndPointAssessorOrganisationId);

            return new EpaoStandardsCountResponse(count);
        }
    }
}
