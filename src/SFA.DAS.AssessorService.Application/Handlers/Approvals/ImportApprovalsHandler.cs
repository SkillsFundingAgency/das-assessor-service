using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Approvals
{
    public class ImportApprovalsHandler : IRequestHandler<ImportApprovalsRequest>
    {
        public const string TOLERANCE_SETTING_NAME = "ApprovalsExtract.StartToleranceS";
        public const string BATCHSIZE_SETTING_NAME = "ApprovalsExtract.BatchSize";

        private readonly ILogger<ImportApprovalsHandler> _logger;
        private readonly IApprovalsExtractRepository _approvalsExtractRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IOuterApiService _outerApiService;

        public ImportApprovalsHandler(
            ILogger<ImportApprovalsHandler> logger
            , IApprovalsExtractRepository approvalsExtractRepository
            , ISettingRepository settingRepository
            , IOuterApiService outerApiService
            )
        {
            _logger = logger;
            _approvalsExtractRepository = approvalsExtractRepository;
            _settingRepository = settingRepository;
            _outerApiService = outerApiService;
        }

        public async Task<Unit> Handle(ImportApprovalsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Commencing approvals import.");

                // 1. Figure out extract start time.

                DateTime? extractStartTime = null;
                DateTime? latestApprovalsExtractTimestamp = await _approvalsExtractRepository.GetLatestExtractTimestamp();
                if(null != latestApprovalsExtractTimestamp && latestApprovalsExtractTimestamp.HasValue)
                {
                    extractStartTime = latestApprovalsExtractTimestamp.Value.AddSeconds(- await GetSettingAsInt(TOLERANCE_SETTING_NAME));
                }

                // 2. Request the extract in batches.

                int batchSize = await GetSettingAsInt(BATCHSIZE_SETTING_NAME);
                int batchNumber = 0;
                GetAllLearnersResponse learnersBatch = null;

                do
                {
                    batchNumber++;
                    learnersBatch = await _outerApiService.GetAllLearners(extractStartTime, batchNumber, batchSize);
                    if (null == learnersBatch || null == learnersBatch.Learners)
                    {
                        throw new Exception($"Failed to get learners batch: sinceTime={extractStartTime?.ToString("o", System.Globalization.CultureInfo.InvariantCulture)} batchNumber={batchNumber} batchSize={batchSize}");
                    }

                    // 3. Upsert ApprovalsExtract batch.

                    UpsertApprovalsExtract(learnersBatch.Learners);

                } while (batchNumber < learnersBatch.TotalNumberOfBatches);

                // 4. Run Populate Learner




                _logger.LogInformation("Approvals import completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import approvals.");
                throw;
            }

            return Unit.Value;
        }

        private async Task<int> GetSettingAsInt(string name)
        {
            int value = 0;

            var settingValue = await _settingRepository.GetSetting(name);
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                _logger.LogWarning($"No setting value for '{name}' was found.");
            }
            else
            {
                if (!int.TryParse(settingValue, out int numericValue))
                {
                    _logger.LogWarning($"Value for setting '{name}' is not a number.");
                }
                else
                {
                    value = Math.Abs(numericValue);
                }
            }

            return value;
        }

        private void UpsertApprovalsExtract(List<Infrastructure.OuterApi.Learner> learners)
        {
            var approvalsExtract = Mapper.Map<List<Infrastructure.OuterApi.Learner>, List<Domain.Entities.ApprovalsExtract>>(learners);
            _approvalsExtractRepository.UpsertApprovalsExtract(approvalsExtract);
        }
    }
}
