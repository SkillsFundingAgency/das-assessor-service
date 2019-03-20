using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetEpaoRegisteredStandardsHandler : IRequestHandler<GetEpaoRegisteredStandardsRequest, PaginatedList<GetEpaoRegisteredStandardsResponse>>
    {
        private readonly ILogger<GetEpaoRegisteredStandardsHandler> _logger;
        private readonly IStandardRepository _standardRepository;
        public GetEpaoRegisteredStandardsHandler(ILogger<GetEpaoRegisteredStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> Handle(GetEpaoRegisteredStandardsRequest request, CancellationToken cancellationToken)
        {
            const int pageSize = 10;
            _logger.LogInformation("Retreiving Epao registered standards");
            var result =  await _standardRepository.GetEpaoRegisteredStandards(request.EpaoId, pageSize, request.PageIndex ?? 1);

            var epaoRegisteredStandardsResult = result.PageOfResults.Select(o =>
                new GetEpaoRegisteredStandardsResponse
                {
                    Level = o.Level,
                    StandardCode = o.StandardCode,
                    StandardReference = o.StandardReference,
                    StandardName = o.StandardName
                }).ToList();
           return new PaginatedList<GetEpaoRegisteredStandardsResponse>(epaoRegisteredStandardsResult, result.TotalCount, request.PageIndex ?? 1, pageSize);
        }
    }
}
