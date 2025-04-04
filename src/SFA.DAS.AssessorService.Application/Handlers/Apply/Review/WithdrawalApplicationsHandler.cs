using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class WithdrawalApplicationsHandler : BaseHandler, IRequestHandler<WithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<WithdrawalApplicationsHandler> _logger;

        public WithdrawalApplicationsHandler(IApplyRepository repository, ILogger<WithdrawalApplicationsHandler> logger, IMapper mapper)
            :base(mapper)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(WithdrawalApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving withdrawal applications");

            var organisationApplicationsResult = await _repository.GetWithdrawalApplications(request.OrganisationId, request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            var items = _mapper.Map<IEnumerable<ApplicationListItem>, IEnumerable<ApplicationSummaryItem>>(organisationApplicationsResult.PageOfResults);

            if (items.Any())
            {
                foreach (var item in items)
                {
                    item.WithdrawalType = GetWithdrawalApplicationType(item);
                }
            }

            return new PaginatedList<ApplicationSummaryItem>(items.ToList(),
                    organisationApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }

        private string GetWithdrawalApplicationType(ApplicationSummaryItem item)
        {
            if (item.StandardReference == null)
            {
                return WithdrawalTypes.Register;
            }
            else if (item.StandardApplicationType == StandardApplicationTypes.StandardWithdrawal)
            {
                return WithdrawalTypes.Standard;
            }

            return null;
        }
    }
}
