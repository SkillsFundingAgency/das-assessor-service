using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class FeedbackAddedApplicationsHandler : IRequestHandler<FeedbackAddedApplicationsRequest, List<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;

        public FeedbackAddedApplicationsHandler(IApplyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ApplicationSummaryItem>> Handle(FeedbackAddedApplicationsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetFeedbackAddedApplications();
        }
    }
}
