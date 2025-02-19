using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch
{
    public class FrameworkSearchHandler : BaseHandler, IRequestHandler<FrameworkSearchQuery, List<FrameworkSearchResult>>
    {
        private readonly ILogger<FrameworkSearchHandler> _logger;
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;

        public FrameworkSearchHandler(IFrameworkLearnerRepository frameworkLearnerRepository, ILogger<FrameworkSearchHandler> logger, IMapper mapper)
            :base(mapper)
        {
            _frameworkLearnerRepository = frameworkLearnerRepository;
            _logger = logger;
        }

        public async Task<List<FrameworkSearchResult>> Handle(FrameworkSearchQuery request, CancellationToken cancellationToken)
        {
            var searchResults = await _frameworkLearnerRepository.Search(request.FirstName, request.LastName, request.DateOfBirth);
            return _mapper.Map<List<FrameworkSearchResult>>(searchResults);
        }
    }
}