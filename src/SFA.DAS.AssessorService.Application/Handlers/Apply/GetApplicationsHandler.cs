using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
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
            List<Domain.Entities.Apply> result = null;

            switch(request.ApplicationType)
            {
                case ApplicationTypes.Combined:
                    result = await _applyRepository.GetCombindedApplications(request.UserId);
                    break;
                case ApplicationTypes.Organisation:
                    result = await _applyRepository.GetOrganisationApplications(request.UserId);
                    break;
                case ApplicationTypes.Standard:
                    result = await _applyRepository.GetStandardApplications(request.UserId);
                    break;
                case ApplicationTypes.OrganisationWithdrawal:
                    result = await _applyRepository.GetOrganisationWithdrawalApplications(request.UserId);
                    break;
                case ApplicationTypes.StandardWithdrawal:
                    result = await _applyRepository.GetStandardWithdrawalApplications(request.UserId);
                    break;
            }

            return result != null
                ? Mapper.Map<List<Domain.Entities.Apply>, List<ApplicationResponse>>(result)
                : null;
        }
    }
}
