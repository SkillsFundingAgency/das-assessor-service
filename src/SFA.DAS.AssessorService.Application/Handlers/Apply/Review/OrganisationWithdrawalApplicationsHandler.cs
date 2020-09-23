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
    public class OrganisationWithdrawalApplicationsHandler : IRequestHandler<OrganisationWithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<OrganisationWithdrawalApplicationsHandler> _logger;

        public OrganisationWithdrawalApplicationsHandler(IApplyRepository repository, ILogger<OrganisationWithdrawalApplicationsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(OrganisationWithdrawalApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving organisation withdrawal applications");

            var organisationApplicationsResult = await _repository.GetOrganisationWithdrawalApplications(request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            return new PaginatedList<ApplicationSummaryItem>(organisationApplicationsResult.PageOfResults.ToList(),
                    organisationApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }
    }
}
