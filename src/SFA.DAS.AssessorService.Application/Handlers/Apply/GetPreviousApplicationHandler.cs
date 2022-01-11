using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class GetPreviousApplicationsHandler : IRequestHandler<GetPreviousApplicationsRequest, List<ApplicationResponse>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetPreviousApplicationsHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<List<ApplicationResponse>> Handle(GetPreviousApplicationsRequest request, CancellationToken cancellationToken)
        {
            var result = await _applyRepository.GetPreviousApplicationsForStandard(request.OrgId, request.StandardReference);

            return result != null
                ? Mapper.Map<List<ApplySummary>, List<ApplicationResponse>>(result)
                : null;
        }
    }
}
