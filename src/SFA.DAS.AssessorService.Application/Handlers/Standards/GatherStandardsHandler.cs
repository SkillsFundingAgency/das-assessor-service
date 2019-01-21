using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GatherStandardsHandler : IRequestHandler<GatherStandardsRequest, string>
    {
        private readonly ILogger<GatherStandardsHandler> _logger;
        private readonly IStandardService _standardService;
        private readonly IStandardRepository _standardRepository;

        public GatherStandardsHandler(IStandardService standardService, ILogger<GatherStandardsHandler> logger, IStandardRepository standardRepository)
        {
            _standardService = standardService;
            _logger = logger;
            _standardRepository = standardRepository;
        }

        public async Task<string> Handle(GatherStandardsRequest request, CancellationToken cancellationToken)
        {
            var minimumHoursBetweenCollations = 24;
            var dateOfLastCollation = await _standardRepository.GetDateOfLastStandardCollation();
            if (dateOfLastCollation == null || dateOfLastCollation.Value.AddHours(minimumHoursBetweenCollations) < DateTime.Now)
            {
                _logger.LogInformation("Gathering Standard Details from all sources in the handler");
                var results = await _standardService.GatherAllStandardDetails();
                _logger.LogInformation("Upserting gathered standards");
                var responseDetails = await _standardRepository.UpsertStandards(results.ToList());
                _logger.LogInformation("Processing of Standards Upsert complete");
                return responseDetails;
            }

            return $"Collation was last done on {dateOfLastCollation.Value}.  There is a minimum of {minimumHoursBetweenCollations} hours between collation runs";
        }
    }
}
