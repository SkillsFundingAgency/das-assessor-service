using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class ApplicationReviewStatusCountsHandler : IRequestHandler<ApplicationReviewStatusCountsRequest, ApplicationReviewStatusCounts>
    {
        private readonly IApplyRepository _repository;

        public ApplicationReviewStatusCountsHandler(IApplyRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApplicationReviewStatusCounts> Handle(ApplicationReviewStatusCountsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetApplicationReviewStatusCounts();
        }
    }
}
