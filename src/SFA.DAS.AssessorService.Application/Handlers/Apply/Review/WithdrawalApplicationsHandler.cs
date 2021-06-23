using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class WithdrawalApplicationsHandler : IRequestHandler<WithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<WithdrawalApplicationsHandler> _logger;

        public WithdrawalApplicationsHandler(IApplyRepository repository, ILogger<WithdrawalApplicationsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(WithdrawalApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving withdrawal applications");

            var organisationApplicationsResult = await _repository.GetWithdrawalApplications(request.OrganisationId, request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            var items = Mapper.Map<IEnumerable<ApplicationListItem>, IEnumerable<ApplicationSummaryItem>>(organisationApplicationsResult.PageOfResults);

            return new PaginatedList<ApplicationSummaryItem>(items.ToList(),
                    organisationApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }
    }
}
