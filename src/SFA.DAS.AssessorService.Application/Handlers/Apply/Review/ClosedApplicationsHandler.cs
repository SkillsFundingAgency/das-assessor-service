using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class ClosedApplicationsHandler : IRequestHandler<ClosedApplicationsRequest, List<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;

        public ClosedApplicationsHandler(IApplyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ApplicationSummaryItem>> Handle(ClosedApplicationsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetClosedApplications();
        }
    }
}
