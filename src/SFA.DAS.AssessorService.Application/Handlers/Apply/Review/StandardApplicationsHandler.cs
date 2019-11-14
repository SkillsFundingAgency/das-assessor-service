using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class StandardApplicationsHandler : IRequestHandler<StandardApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<StandardApplicationsHandler> _logger;

        public StandardApplicationsHandler(IApplyRepository repository, ILogger<StandardApplicationsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(StandardApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving standard applications");

            var standardApplicationsResult = await _repository.GetStandardApplications(request.OrgansiationId, request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            return new PaginatedList<ApplicationSummaryItem>(standardApplicationsResult.PageOfResults.ToList(),
                    standardApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }
    }
}
