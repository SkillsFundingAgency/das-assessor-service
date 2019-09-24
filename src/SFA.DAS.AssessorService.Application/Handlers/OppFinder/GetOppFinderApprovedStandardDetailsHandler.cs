using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetOppFinderApprovedStandardDetailsHandler : IRequestHandler<GetOppFinderApprovedStandardDetailsRequest, GetOppFinderApprovedStandardDetailsResponse>
    {
        private readonly ILogger<GetOppFinderApprovedStandardDetailsHandler> _logger;
        private readonly IStandardRepository _standardRepository;
        private readonly IMediator _mediator;

        public GetOppFinderApprovedStandardDetailsHandler(ILogger<GetOppFinderApprovedStandardDetailsHandler> logger, IStandardRepository standardRepository, IMediator mediator)
        {
            _logger = logger;
            _standardRepository = standardRepository;
            _mediator = mediator;
        }

        public async Task<GetOppFinderApprovedStandardDetailsResponse> Handle(GetOppFinderApprovedStandardDetailsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retreiving approved standard details: {request.StandardCode}");
            var result = await _standardRepository.GetOppFinderApprovedStandardDetails(request.StandardCode);

            string eqaProvider, eqaProviderLink = string.Empty;

            if (await _mediator.Send(new ValidationRequest { Type = "email", Value = result.OverviewResult?.EqaProviderContactEmail }))
            {
                eqaProvider = result.OverviewResult?.EqaProviderContactEmail;
                eqaProviderLink = result.OverviewResult?.EqaProviderContactEmail;
            }
            else if (await _mediator.Send(new ValidationRequest { Type = "email", Value = result.OverviewResult?.EqaProviderContactName }))
            {
                eqaProvider = result.OverviewResult?.EqaProviderContactName;
                eqaProviderLink = result.OverviewResult?.EqaProviderContactName;
            }
            else
            {
                eqaProvider = result.OverviewResult?.EqaProviderName;

                if (await _mediator.Send(new ValidationRequest { Type = "websitelink", Value = result.OverviewResult?.EqaProviderWebLink }))
                {
                    eqaProviderLink = result.OverviewResult?.EqaProviderWebLink;
                }
            }

            return new GetOppFinderApprovedStandardDetailsResponse
            {
                Title = result.OverviewResult?.StandardName,
                OverviewOfRole = result.OverviewResult?.OverviewOfRole,
                StandardLevel = result.OverviewResult?.StandardLevel != null
                    ? result.OverviewResult.StandardLevel > 0 ? result.OverviewResult.StandardLevel.ToString() : "TBC"
                    : null,
                StandardReference = result.OverviewResult?.StandardReference,
                TotalActiveApprentices = result.OverviewResult?.TotalActiveApprentices ?? 0,
                TotalCompletedAssessments = result.OverviewResult?.TotalCompletedAssessments ?? 0,
                Sector = result.OverviewResult?.Sector,
                TypicalDuration = result.OverviewResult?.TypicalDuration,
                ApprovedForDelivery = result.OverviewResult.ApprovedForDelivery,
                MaxFunding = result.OverviewResult?.MaxFunding != null
                    ? int.Parse(result.OverviewResult?.MaxFunding).ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))
                    : null,
                Trailblazer = result.OverviewResult?.Trailblazer,
                StandardPageUrl = result.OverviewResult?.StandardPageUrl,
                EqaProvider = eqaProvider,
                EqaProviderLink = eqaProviderLink,
                RegionResults = result.RegionResults?.ConvertAll(p => new OppFinderApprovedStandardDetailsRegionResult
                {
                    Region = p.Region,
                    EndPointAssessorsNames = p.EndPointAssessorsNames?
                        .TrimStart(new char[] { '[', '"' })
                        .TrimEnd(new char[] { ']', '"' })
                        .Replace("\",\"", " | "),
                    EndPointAssessors = p.EndPointAssessors,
                    ActiveApprentices = p.ActiveApprentices,
                    CompletedAssessments = p.CompletedAssessments
                })
            };
        }
    }
}
