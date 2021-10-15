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
        private readonly IOppFinderRepository _oppFinderRepository;

        public GetOppFinderNonApprovedStandardDetailsHandler(ILogger<GetOppFinderNonApprovedStandardDetailsHandler> logger, IOppFinderRepository oppFinderRepository)
        {
            _logger = logger;
            _oppFinderRepository = oppFinderRepository;
        }

        public async Task<GetOppFinderNonApprovedStandardDetailsResponse> Handle(GetOppFinderNonApprovedStandardDetailsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retreiving non approved standard details: {request.StandardReference}");

            var result = await _oppFinderRepository.GetOppFinderNonApprovedStandardDetails(request.StandardReference);

            if (result == null)
                return null;

            return new GetOppFinderNonApprovedStandardDetailsResponse
            {
                Title = result.Title,
                OverviewOfRole = result.OverviewOfRole,
                StandardLevel = int.Parse(result.Level) > 0 ? result.Level : "To be confirmed",
                StandardReference = result.IFateReferenceNumber,
                Sector = result.Route,
                TypicalDuration = result.TypicalDuration,
                Trailblazer = result.TrailblazerContact,
                StandardPageUrl = result.StandardPageUrl
            };
        }
    }
}
