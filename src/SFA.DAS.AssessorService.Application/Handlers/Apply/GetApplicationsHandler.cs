using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class GetApplicationsHandler : BaseHandler, IRequestHandler<GetApplicationsRequest, List<ApplicationResponse>>
    {
        private readonly IApplyRepository _applyRepository;

        public GetApplicationsHandler(IApplyRepository applyRepository, IMapper mapper)
            :base(mapper)
        {
            _applyRepository = applyRepository;
        }

        public async Task<List<ApplicationResponse>> Handle(GetApplicationsRequest request, CancellationToken cancellationToken)
        {
            List<ApplySummary> result = null;

            switch(request.ApplicationType)
            {
                case ApplicationTypes.Organisation:
                    result = await _applyRepository.GetOrganisationApplications(request.UserId);
                    break;
                case ApplicationTypes.Standard:
                    result = await _applyRepository.GetStandardApplications(request.UserId);
                    break;
                case ApplicationTypes.Withdrawal:
                    result = await _applyRepository.GetWithdrawalApplications(request.UserId);
                    break;
                case ApplicationTypes.OrganisationWithdrawal:
                    result = await _applyRepository.GetOrganisationWithdrawalApplications(request.UserId);
                    break;
                case ApplicationTypes.StandardWithdrawal:
                    result = await _applyRepository.GetStandardWithdrawalApplications(request.UserId);
                    break;
            }

            return result != null
                ? _mapper.Map<List<ApplySummary>, List<ApplicationResponse>>(result)
                : null;
        }
    }
}
