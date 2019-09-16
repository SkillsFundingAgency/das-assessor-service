using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GatherStandardsHandler : IRequestHandler<GatherStandardsRequest, string[]>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStandardService _standardService;
        private readonly IStandardRepository _standardRepository;
        private readonly ILogger<GatherStandardsHandler> _logger;

        public GatherStandardsHandler(IUnitOfWork unitOfWork, IStandardService standardService, IStandardRepository standardRepository, ILogger<GatherStandardsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _standardService = standardService;
            _standardRepository = standardRepository;
            _logger = logger;
        }

        public async Task<string[]> Handle(GatherStandardsRequest request, CancellationToken cancellationToken)
        {
            var minimumHoursBetweenCollations = 23;
            var dateOfLastCollation = await _standardRepository.GetDateOfLastStandardCollation();
            if (dateOfLastCollation == null || dateOfLastCollation.Value.AddHours(minimumHoursBetweenCollations) < DateTime.Now)
            {
                try
                {
                    _unitOfWork.Begin();

                    var ifaStandards = await _standardService.GetIfaStandards();

                    var approvedIfaStandards = ifaStandards.Where(p => p.LarsCode != 0).ToList();
                    _logger.LogInformation("Gathering approved Standard details from all sources in the handler");
                    var approvedStandardDetails = await _standardService.GatherAllApprovedStandardDetails(approvedIfaStandards);
                    _logger.LogInformation("Upserting gathered approved Standards");
                    var approvedResponse = await _standardRepository.UpsertApprovedStandards(approvedStandardDetails.ToList());
                    _logger.LogInformation("Processing of approved Standards upsert complete");

                    var nonApprovedIfaStandards = ifaStandards.Where(p => p.LarsCode == 0).ToList();
                    _logger.LogInformation("Gathering non-approved Standard details from all sources in the handler");
                    var nonApprovedStandardDetails = _standardService.GatherAllNonApprovedStandardDetails(nonApprovedIfaStandards);
                    _logger.LogInformation("Upserting gathered non-approved Standards");
                    var nonApprovedResponse = await _standardRepository.UpsertNonApprovedStandards(nonApprovedStandardDetails.ToList());
                    _logger.LogInformation("Processing of non-approved Standards upsert complete");

                    _unitOfWork.Commit();

                    return new string[] { approvedResponse, nonApprovedResponse };
                }
                catch(Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logger.LogError(ex, "Processing of Standards could not be completed");
                    throw;
                }
            }

            return new string[] { $"Collation was last done on {dateOfLastCollation.Value}.  There is a minimum of {minimumHoursBetweenCollations} hours between collation runs" };
        }
    }
}
