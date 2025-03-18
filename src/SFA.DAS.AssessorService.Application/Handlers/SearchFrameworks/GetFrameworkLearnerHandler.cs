using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch
{
    public class GetFrameworkLearnerHandler : BaseHandler, IRequestHandler<GetFrameworkLearnerRequest, GetFrameworkLearnerResponse>
    {
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;

        public GetFrameworkLearnerHandler(IMapper mapper, IFrameworkLearnerRepository frameworkLearnerRepository)
            :base(mapper)
        {
            _frameworkLearnerRepository = frameworkLearnerRepository;
        }

        public async Task<GetFrameworkLearnerResponse> Handle(GetFrameworkLearnerRequest request, CancellationToken cancellationToken)
        {
            var returnValue =  await _frameworkLearnerRepository.GetFrameworkLearner(request.Id);
            return _mapper.Map<GetFrameworkLearnerResponse>(returnValue);
        }
    }
}