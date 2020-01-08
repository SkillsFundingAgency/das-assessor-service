using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class GetApplicationsHandler : IRequestHandler<GetApplicationsRequest, List<ApplicationResponse>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetApplicationsHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<List<ApplicationResponse>> Handle(GetApplicationsRequest request, CancellationToken cancellationToken)
        {
            List<Domain.Entities.Apply> result;

            if (!request.CreatedBy)
                result = await _applyRepository.GetOrganisationApplications(request.UserId);
            else
                result = await _applyRepository.GetUserApplications(request.UserId);

            return Mapper.Map<List<Domain.Entities.Apply>, List<ApplicationResponse>>(result);
        }
    }
}
