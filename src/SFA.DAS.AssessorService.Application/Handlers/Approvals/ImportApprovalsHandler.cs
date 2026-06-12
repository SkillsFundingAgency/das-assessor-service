using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Approvals
{
    public class ImportApprovalsHandler : BaseHandler, IRequestHandler<ImportApprovalsRequest, Unit>
    {
        public const string TOLERANCE_SETTING_NAME = "ApprovalsExtract.StartToleranceS";
        public const string BATCHSIZE_SETTING_NAME = "ApprovalsExtract.BatchSize";

        private readonly ILogger<ImportApprovalsHandler> _logger;
        private readonly IApprovalsExtractRepository _approvalsExtractRepository;
        private readonly ISettingRepository _settingRepository;
        private readonly IOuterApiService _outerApiService;

        public ImportApprovalsHandler(
            ILogger<ImportApprovalsHandler> logger, 
            IApprovalsExtractRepository approvalsExtractRepository, 
            ISettingRepository settingRepository, 
            IOuterApiService outerApiService, IMapper mapper)
            :base(mapper)
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
                _logger.LogInformation($"Calculating Approvals changed date, initially set to null.");
                DateTime? latestApprovalsExtractTimestamp = await _approvalsExtractRepository.GetLatestExtractTimestamp();
                if(null != latestApprovalsExtractTimestamp && latestApprovalsExtractTimestamp.HasValue)
                {
                    extractStartTime = latestApprovalsExtractTimestamp.Value.AddSeconds(- await GetSettingAsInt(TOLERANCE_SETTING_NAME));
                    _logger.LogInformation($"Pulling Approvals changed since: {extractStartTime}");
                }

                // 2. Request the extract in batches.

                int batchSize = await GetSettingAsInt(BATCHSIZE_SETTING_NAME);
                int batchNumber = 0;
                int count = 0;
                GetAllLearnersResponse learnersBatch = null;

                // 3. Reset Staging Table
                await _approvalsExtractRepository.ClearApprovalsExtractStaging();

                do
                {
                    batchNumber++;
                    learnersBatch = await _outerApiService.GetAllLearners(extractStartTime, batchNumber, batchSize);
                    if (null == learnersBatch || null == learnersBatch.Learners)
                    {
                        throw new Exception($"Failed to get learners batch: sinceTime={extractStartTime?.ToString("o", System.Globalization.CultureInfo.InvariantCulture)} batchNumber={batchNumber} batchSize={batchSize}");
                    }

                    // 4. Upsert Batch to ApprovalsExtract_Staging.
                    _logger.LogInformation($"Approvals batch import loop. Starting batch {batchNumber} of {learnersBatch.TotalNumberOfBatches}");

                    await UpsertApprovalsExtractToStaging(learnersBatch.Learners);
                    count += learnersBatch.Learners.Count;
                    _logger.LogInformation($"Approvals batch import loop. Batch Completed {batchNumber} of {learnersBatch.TotalNumberOfBatches}. Total Inserted: {count}");

                } while (batchNumber < learnersBatch.TotalNumberOfBatches);

                // 5. Run Populate ApprovalsExtract From Staging
                _logger.LogInformation($"Begin Populating Approvals Extract");
                await _approvalsExtractRepository.PopulateApprovalsExtract();
                _logger.LogInformation($"Finished Populating Approvals Extract");

                // 6. Run Populate Learner
                _logger.LogInformation($"Begin Running Populate Learner");
                var learnerCount = await _approvalsExtractRepository.PopulateLearner();
                _logger.LogInformation($"Finished Running Populate Learner");

                // 7. Update providers cache

                await _approvalsExtractRepository.InsertProvidersFromApprovalsExtract();

                _logger.LogInformation($"Approvals import completed successfully. {count} record(s) read from outer api, {learnerCount} records inserted to Learner table.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Approvals import failed to complete successfully.");
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

        private async Task UpsertApprovalsExtractToStaging(List<Infrastructure.ApiClients.OuterApi.Learner> learners)
        {
            var approvalsExtract = _mapper.Map<List<Infrastructure.ApiClients.OuterApi.Learner>, List<Domain.Entities.ApprovalsExtract>>(learners);
            await _approvalsExtractRepository.UpsertApprovalsExtractToStaging(approvalsExtract);
        }
    }
}
