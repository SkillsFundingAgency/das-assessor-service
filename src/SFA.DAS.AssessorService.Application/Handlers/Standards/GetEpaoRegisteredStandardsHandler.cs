using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
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
            _logger.LogInformation("Retrieving Epao registered standards");
            var result =  await _standardRepository.GetEpaoRegisteredStandards(request.EpaoId, request.RequireAtLeastOneVersion, request.PageSize, request.PageIndex);

            var epaoRegisteredStandardsResult = result.PageOfResults.Select(o =>
                new GetEpaoRegisteredStandardsResponse
                {
                    Level = o.Level,
                    StandardCode = o.StandardCode,
                    StandardName = o.StandardName,
                    ReferenceNumber = o.ReferenceNumber,
                    NewVersionAvailable = o.NewVersionAvailable,
                    NumberOfVersions = o.NumberOfVersions
                }).ToList();

           return new PaginatedList<GetEpaoRegisteredStandardsResponse>(epaoRegisteredStandardsResult, result.TotalCount, request.PageIndex, request.PageSize);
        }
    }
}
