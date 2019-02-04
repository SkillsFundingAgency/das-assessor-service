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
    public class GetEpaoPipelineCountHandler: IRequestHandler<GetEpaoPipelineCountRequest, EpaoPipelineCountResponse>
    {
        private readonly IStandardRepository _standardRepository;

        public GetEpaoPipelineCountHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }
        public async Task<EpaoPipelineCountResponse> Handle(GetEpaoPipelineCountRequest request, CancellationToken cancellationToken)
        {
            var count =  await _standardRepository.GetEpaoPipelineCount(request
                .EndPointAssessorOrganisationId);

            return new EpaoPipelineCountResponse(count);
        }
    }
}
