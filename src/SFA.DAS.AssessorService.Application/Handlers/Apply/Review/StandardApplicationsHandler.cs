﻿using System.Collections.Generic;
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
    public class StandardApplicationsHandler : BaseHandler, IRequestHandler<StandardApplicationsRequest, PaginatedList<ApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;
        private readonly ILogger<StandardApplicationsHandler> _logger;

        public StandardApplicationsHandler(IApplyRepository repository, ILogger<StandardApplicationsHandler> logger, IMapper mapper)
            :base(mapper)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> Handle(StandardApplicationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving standard applications");

            var standardApplicationsResult = await _repository.GetStandardApplications(request.OrganisationId, request.ReviewStatus, request.SortColumn, request.SortAscending,
                request.PageSize, request.PageIndex);

            var items = _mapper.Map<IEnumerable<ApplicationListItem>, IEnumerable<ApplicationSummaryItem>>(standardApplicationsResult.PageOfResults);

            return new PaginatedList<ApplicationSummaryItem>(items.ToList(),
                    standardApplicationsResult.TotalCount, request.PageIndex, request.PageSize, request.PageSetSize);
        }
    }
}
