using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch
{
    public class FrameworkSearchHandler : BaseHandler, IRequestHandler<FrameworkLearnerSearchRequest, List<FrameworkLearnerSearchResponse>>
    {
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;

        public FrameworkSearchHandler(IFrameworkLearnerRepository frameworkLearnerRepository, IMapper mapper)
            :base(mapper)
        {
            _frameworkLearnerRepository = frameworkLearnerRepository;
        }

        public async Task<List<FrameworkLearnerSearchResponse>> Handle(FrameworkLearnerSearchRequest request, CancellationToken cancellationToken)
        {
            var searchResults = await _frameworkLearnerRepository.Search(request.FirstName, request.LastName, request.DateOfBirth);
            return _mapper.Map<List<FrameworkLearnerSearchResponse>>(searchResults);
        }
    }
}