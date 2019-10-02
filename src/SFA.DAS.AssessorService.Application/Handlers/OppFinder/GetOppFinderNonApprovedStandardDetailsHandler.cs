using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetOppFinderNonApprovedStandardDetailsHandler : IRequestHandler<GetOppFinderNonApprovedStandardDetailsRequest, GetOppFinderNonApprovedStandardDetailsResponse>
    {
        private readonly ILogger<GetOppFinderNonApprovedStandardDetailsHandler> _logger;
        private readonly IStandardRepository _standardRepository;

        public GetOppFinderNonApprovedStandardDetailsHandler(ILogger<GetOppFinderNonApprovedStandardDetailsHandler> logger, IStandardRepository standardRepository)
        {
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<GetOppFinderNonApprovedStandardDetailsResponse> Handle(GetOppFinderNonApprovedStandardDetailsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retreiving non approved standard details: {request.StandardReference}");
            var result = await _standardRepository.GetStandardNonApprovedCollationByReferenceNumber(request.StandardReference);

            if (result == null)
                return null;

            return new GetOppFinderNonApprovedStandardDetailsResponse
            {
                Title = result.Title,
                OverviewOfRole = result.StandardData.OverviewOfRole,
                StandardLevel = (result.StandardData?.Level > 0 ? result.StandardData?.Level.ToString() : "TBC"),
                StandardReference = result.ReferenceNumber,
                Sector = result.StandardData.Category,
                TypicalDuration = result.StandardData?.Duration ?? 0,
                Trailblazer = result.StandardData.Trailblazer,
                StandardPageUrl = result.StandardData.StandardPageUrl
            };
        }
    }
}
