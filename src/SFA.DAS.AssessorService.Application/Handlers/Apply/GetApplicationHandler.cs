using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class GetApplicationHandler : BaseHandler, IRequestHandler<GetApplicationRequest, ApplicationResponse>
    {
        private readonly IApplyRepository _applyRepository;

        public GetApplicationHandler(IApplyRepository applyRepository, IMapper mapper)
            :base(mapper)
        {
            _applyRepository = applyRepository;
        }

        public async Task<ApplicationResponse> Handle(GetApplicationRequest request, CancellationToken cancellationToken)
        {
            var result = await _applyRepository.GetApplication(request.ApplicationId, request.UserId);
            return _mapper.Map<ApplySummary, ApplicationResponse>(result);
        }
    }
}
