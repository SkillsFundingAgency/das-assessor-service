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
    public class GetWithdrawnApplicationsHandler : IRequestHandler<GetWithdrawnApplicationsRequest, ApplicationResponse>
    {
        private readonly IApplyRepository _applyRepository;

        public GetWithdrawnApplicationsHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<ApplicationResponse> Handle(GetWithdrawnApplicationsRequest request, CancellationToken cancellationToken)
        {
            var result = await _applyRepository.GetWithdrawnApplications(request.ApplicationId, request.StandardCode);
            return Mapper.Map<ApplySummary, ApplicationResponse>(result);
        }
    }
}
