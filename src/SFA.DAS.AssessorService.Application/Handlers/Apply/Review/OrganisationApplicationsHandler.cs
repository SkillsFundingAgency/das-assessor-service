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
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class OrganisationApplicationsHandler : BaseHandler, IRequestHandler<OrganisationApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<OrganisationApplicationsHandler> _logger;

        public OrganisationApplicationsHandler(IApplyRepository repository, ILogger<OrganisationApplicationsHandler> logger, IMapper mapper)
            :base(mapper)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(OrganisationApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving organisation applications");

            var organisationApplicationsResult = await _repository.GetOrganisationApplications(request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            var items = _mapper.Map<IEnumerable<ApplicationListItem>, IEnumerable<ApplicationSummaryItem>>(organisationApplicationsResult.PageOfResults);

            return new PaginatedList<ApplicationSummaryItem>(items.ToList(),
                    organisationApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }
    }
}
