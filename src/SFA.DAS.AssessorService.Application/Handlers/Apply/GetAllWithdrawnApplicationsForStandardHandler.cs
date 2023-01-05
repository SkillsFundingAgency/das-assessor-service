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
    public class GetAllWithdrawnApplicationsForStandardHandler : IRequestHandler<GetAllWithdrawnApplicationsForStandardRequest, List<ApplicationResponse>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetAllWithdrawnApplicationsForStandardHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<List<ApplicationResponse>> Handle(GetAllWithdrawnApplicationsForStandardRequest request, CancellationToken cancellationToken)
        {
            var result = await _applyRepository.GetAllWithdrawnApplicationsForStandard(request.OrgId, request.StandardCode);

            return result != null
                ? Mapper.Map<List<ApplySummary>, List<ApplicationResponse>>(result)
                : null;

        }
    }
}
