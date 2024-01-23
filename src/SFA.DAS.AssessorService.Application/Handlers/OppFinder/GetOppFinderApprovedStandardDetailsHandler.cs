using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Extensions;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetOppFinderApprovedStandardDetailsHandler : IRequestHandler<GetOppFinderApprovedStandardDetailsRequest, GetOppFinderApprovedStandardDetailsResponse>
    {
        private readonly ILogger<GetOppFinderApprovedStandardDetailsHandler> _logger;
        private readonly IOppFinderRepository _oppFinderRepository;
        private readonly IMediator _mediator;

        public GetOppFinderApprovedStandardDetailsHandler(ILogger<GetOppFinderApprovedStandardDetailsHandler> logger, IOppFinderRepository oppFinderRepository, IMediator mediator)
        {
            _logger = logger;
            _oppFinderRepository = oppFinderRepository;
            _mediator = mediator;
        }

        public async Task<GetOppFinderApprovedStandardDetailsResponse> Handle(GetOppFinderApprovedStandardDetailsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Retrieving approved standard details: {request.StandardReference}");

            var result = await _oppFinderRepository.GetOppFinderApprovedStandardDetails(request.StandardReference);

            if (result.OverviewResult == null)
                return null;
            string eqaProviderContactEmail = result.OverviewResult?.EqaProviderContactEmail;
            string eqaProviderContactName = result.OverviewResult?.EqaProviderContactName;
            string eqaProvider;

            if (!string.IsNullOrEmpty(eqaProviderContactEmail) && await _mediator.Send(new ValidationRequest { Type = "email", Value = eqaProviderContactEmail }))
            {
                eqaProvider = eqaProviderContactEmail;
            }
            else if (!string.IsNullOrEmpty(eqaProviderContactName) && await _mediator.Send(new ValidationRequest { Type = "email", Value = eqaProviderContactName }))
            {
                eqaProvider = eqaProviderContactName;
            }
            else
            {
                eqaProvider = result.OverviewResult?.EqaProviderName;
            }

            var approvedForDeliveryValidDate = DateTime
                .TryParse(result.OverviewResult?.ApprovedForDelivery, out DateTime approvedForDelivery);

            return new GetOppFinderApprovedStandardDetailsResponse
            {
                StandardCode = result.OverviewResult?.StandardCode,
                Title = result.OverviewResult?.StandardName,
                OverviewOfRole = result.OverviewResult?.OverviewOfRole,
                StandardLevel = result.OverviewResult?.StandardLevel != null
                    ? result.OverviewResult.StandardLevel > 0 ? result.OverviewResult.StandardLevel.ToString() : "To be confirmed"
                    : null,
                StandardReference = result.OverviewResult?.StandardReference,
                TotalActiveApprentices = result.OverviewResult?.TotalActiveApprentices ?? 0,
                TotalCompletedAssessments = result.OverviewResult?.TotalCompletedAssessments ?? 0,
                Sector = result.OverviewResult?.Sector,
                TypicalDuration = result.OverviewResult?.TypicalDuration,
                ApprovedForDelivery = approvedForDeliveryValidDate
                    ? approvedForDelivery.ToString("d MMMM yyyy")
                    : string.Empty,
                MaxFunding = result.OverviewResult?.MaxFunding != null
                    ? int.Parse(result.OverviewResult?.MaxFunding).ToString("C", CultureInfo.CreateSpecificCulture("en-GB"))
                    : null,
                Trailblazer = result.OverviewResult?.Trailblazer,
                StandardPageUrl = result.OverviewResult?.StandardPageUrl,
                EqaProvider = eqaProvider,
                RegionResults = result.RegionResults?.ConvertAll(p => new OppFinderApprovedStandardDetailsRegionResult
                {
                    Region = p.Region,
                    EndPointAssessorsNames = p.EndPointAssessorsNames?
                        .TrimStart(new char[] { '[', '"' })
                        .TrimEnd(new char[] { ']', '"' })
                        .Split(new string[] { "\",\"" }, StringSplitOptions.None),
                    EndPointAssessors = p.EndPointAssessors,
                    ActiveApprentices = p.ActiveApprentices,
                    CompletedAssessments = p.CompletedAssessments
                }),
                VersionResults = result.VersionResults?.ConvertAll(p => new OppFinderApprovedStandardDetailsVersionResult
                {
                    Version = p.Version,
                    EndPointAssessors = p.EndPointAssessors,
                    ActiveApprentices = p.ActiveApprentices,
                    CompletedAssessments = p.CompletedAssessments
                })
            };
        }
    }
}
