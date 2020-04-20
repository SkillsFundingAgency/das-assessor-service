using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class StandardRepository : Repository, IStandardRepository
    {
        public StandardRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
            SqlMapper.AddTypeHandler(typeof(StandardNonApprovedData), new StandardNonApprovedDataHandler());
        }

        public async Task<List<StandardCollation>> GetStandardCollations()
        {
            return await GetStandardCollationsInternal();
        }

        public async Task<StandardCollation> GetStandardCollationByStandardId(int standardId)
        {
            var standards = await GetStandardCollationsInternal(standardIdFilter: standardId);
            return standards.FirstOrDefault();
        }

        public async Task<StandardCollation> GetStandardCollationByReferenceNumber(string referenceNumber)
        {
            var standards = await GetStandardCollationsInternal(referenceNumberFilter: referenceNumber);
            return standards.FirstOrDefault();
        }

        public async Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollations()
        {
            return await GetStandardNonApprovedCollationsInternal();
        }

        public async Task<StandardNonApprovedCollation> GetStandardNonApprovedCollationByReferenceNumber(string referenceNumber)
        {
            var standards = await GetStandardNonApprovedCollationsInternal(referenceNumberFilter: referenceNumber);
            return standards.FirstOrDefault();
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            var sql = "SELECT * FROM [Options] WHERE IsLive = @isLive AND StdCode = @stdCode";

            var results = await _unitOfWork.Connection.QueryAsync<Option>(
                sql,
                param: new { isLive = 1, stdCode },
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<List<Option>> GetOptions(List<int> stdCodes)
        {
            var sql = "SELECT * FROM [Options] WHERE IsLive = @isLive AND StdCode IN @stdCodes";

            var results = await _unitOfWork.Connection.QueryAsync<Option>(
                sql,
                param: new { isLive = 1, stdCodes },
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        private async Task<List<StandardCollation>> GetStandardCollationsInternal(int? standardIdFilter = null, string referenceNumberFilter = null)
        {
            var standardsDictionary = new Dictionary<string, StandardCollation>();

            var sql = "SELECT * FROM [StandardCollation] WHERE IsLive = @isLive";
            if (standardIdFilter.HasValue)
            {
                sql += " AND StandardId = @standardIdFilter";
            }
            else if (referenceNumberFilter != null)
            {
                sql += " AND ReferenceNumber = @referenceNumberFilter";
            }

            var standards = await _unitOfWork.Connection.QueryAsync<StandardCollation>(
                sql, 
                param: new { isLive = 1, standardIdFilter, referenceNumberFilter }, 
                transaction: _unitOfWork.Transaction);

            foreach (var standard in standards)
            {
                string key = standard.StandardId.HasValue ? standard.StandardId.ToString() : standard.ReferenceNumber;

                if (!standardsDictionary.TryGetValue(key, out StandardCollation dictionaryEntry))
                {
                    dictionaryEntry = standard;
                    dictionaryEntry.Options = new List<string>();
                    standardsDictionary[key] = dictionaryEntry;
                }
            }

            var standardCodes = standardsDictionary.Values.Where(v => v.StandardId.HasValue).Select(v => v.StandardId.Value).ToList();
            var options = await GetOptions(standardCodes);

            foreach (var option in options)
            {
                if (standardsDictionary.TryGetValue(option.StdCode.ToString(), out StandardCollation dictionaryEntry))
                {
                    if (!string.IsNullOrEmpty(option.OptionName))
                    {
                        dictionaryEntry.Options.Add(option.OptionName);
                    }
                }
            }

            return standardsDictionary.Values.ToList();
        }

        private async Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollationsInternal(string referenceNumberFilter = null)
        {
            var sql = "SELECT * FROM StandardNonApprovedCollation WHERE IsLive = @isLive";
            if (referenceNumberFilter != null)
            {
                sql += " AND ReferenceNumber = @referenceNumberFilter";
            }

            var results = await _unitOfWork.Connection.QueryAsync<StandardNonApprovedCollation>(
                sql,
                param: new { isLive = 1, referenceNumberFilter}, 
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<DateTime?> GetDateOfLastStandardCollation()
        {
            const string sql = "SELECT TOP 1 COALESCE(MAX(DateUpdated), MAX(DateAdded)) MaxDate FROM [StandardCollation]";
            var dateOfLastCollation = await _unitOfWork.Connection.QuerySingleAsync<DateTime?>(
                sql, 
                param: null, 
                transaction: _unitOfWork.Transaction);

            return dateOfLastCollation;
        }

        public async Task<string> UpsertApprovedStandards(List<StandardCollation> latestStandards)
        {
            var existingStandards = await GetStandardCollationsInternal();

            var standardsRemoved = existingStandards
                .Where(existingLiveStandard => !latestStandards.Select(latestStandard => latestStandard.StandardId).Contains(existingLiveStandard.StandardId))
                .ToList();

            foreach (var standard in standardsRemoved)
            {
                await UpdateExistingStandardToRemoved(standard);
            }

            var standardsUpdated = latestStandards
                .Where(latestStandard => existingStandards.Any(es => es.StandardId.Equals(latestStandard.StandardId)))
                .ToList();

            foreach(var standard in standardsUpdated)
            {
                await UpdateExistingStandard(standard);
            }

            var standardsInserted = latestStandards
                .Where(latestStandard => !existingStandards.Any(existingStandard => existingStandard.StandardId.Equals(latestStandard.StandardId)))
                .ToList();

            foreach (var standard in standardsInserted)
            {
                await InsertNewStandard(standard);
            }

            return $"Approved Standard: Added: {standardsInserted.Count}; Updated: {standardsUpdated.Count}; Removed: {standardsRemoved.Count}";
        }

        public async Task<string> UpsertNonApprovedStandards(List<StandardNonApprovedCollation> latestStandards)
        {
            var existingStandards = await GetStandardNonApprovedCollations();

            var standardsDeleted = existingStandards
                .Where(es => !latestStandards.Select(ls => ls.ReferenceNumber).Contains(es.ReferenceNumber))
                .ToList();

            foreach (var standard in standardsDeleted)
            {
                await UpdateExistingStandardToRemoved(standard);
            }

            var standardsUpdated = latestStandards
                .Where(ls => existingStandards.Any(es => es.ReferenceNumber.Equals(ls.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsUpdated)
            {
                await UpdateExistingStandard(standard);
            }

            var standardsInserted = latestStandards
                .Where(ls => !existingStandards.Any(es => es.ReferenceNumber.Equals(ls.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsInserted)
            {
                await InsertNewStandard(standard);
            }

            return $"Non-approved Standards: Inserted: {standardsInserted.Count}; Updated: {standardsUpdated.Count}; Removed: {standardsDeleted.Count}";
        }

        public async Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId)
        {
            var @params = new DynamicParameters();
            @params.Add("EPAO", endPointAssessorOrganisationId);
            @params.Add("Count", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _unitOfWork.Connection.QueryAsync(
                "EPAO_Standards_Count",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return @params.Get<int>("Count");
        }

        public async Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(string endPointAssessorOrganisationId, int pageSize, int? pageIndex)
        {
            var epoRegisteredStandardsResult = new EpoRegisteredStandardsResult
            {
                PageOfResults = new List<EPORegisteredStandards>(),
                TotalCount = 0
            };
            var total = await GetEpaoStandardsCount(endPointAssessorOrganisationId);
            var skip = ((pageIndex ?? 1) - 1) * pageSize;
            var result = await _unitOfWork.Connection.QueryAsync<EPORegisteredStandards>(
                "EPAO_Registered_Standards", 
                param: new
                {
                    EPAOId = endPointAssessorOrganisationId,
                    Skip = skip,
                    Take = pageSize
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);
            var epoRegisteredStandards = result?.ToList();

            if (epoRegisteredStandards == null || !epoRegisteredStandards.Any())
                return epoRegisteredStandardsResult;
            epoRegisteredStandardsResult.TotalCount = total;
            epoRegisteredStandardsResult.PageOfResults = epoRegisteredStandards;

            return epoRegisteredStandardsResult;
        }

        public async Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId, string orderBy, string orderDirection, int pageSize, int? pageIndex)
        {
            IEnumerable<EpaoPipelineStandard> epaoPipelines;
            var epaoPipelineStandardsResult = new EpaoPipelineStandardsResult
            {
                PageOfResults = new List<EpaoPipelineStandard>(),
                TotalCount = 0
            };

            var skip = ((pageIndex ?? 1) - 1) * pageSize;
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandard>(
                "GetEPAO_Pipelines", 
                param: new
                {
                    EPAOId = endPointAssessorOrganisationId
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            if (!string.IsNullOrEmpty(orderBy) || pageSize <= 0)
            {
                if (string.IsNullOrEmpty(orderBy) && pageSize == 0)
                {
                    epaoPipelines = result?.ToList();
                }
                else
                {
                    epaoPipelines = result?.AsQueryable().OrderBy($"{orderBy} {orderDirection}").ToList().Skip(skip)
                        .Take(pageSize);
                }
            }
            else
            {
                epaoPipelines = result?.ToList().Skip(skip)
                    .Take(pageSize);
            }

            if (epaoPipelines == null || !epaoPipelines.Any())
                return epaoPipelineStandardsResult;

            epaoPipelineStandardsResult.TotalCount = epaoPipelines.Select(x => x.TotalRows).First();
            epaoPipelineStandardsResult.PageOfResults = epaoPipelines;

            return epaoPipelineStandardsResult;
        }

        public async Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId)
        {
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandardExtract>(
                "GetEPAO_Pipelines_Extract", 
                param: new
                {
                    EPAOId = endPointAssessorOrganisationId
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        private async Task UpdateExistingStandard(StandardCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [StandardCollation] SET " +
                    // when new ReferenceNumber is null (IFA has not supplied one) retain the current ReferenceNumber    
                    "ReferenceNumber = CASE WHEN @referenceNumber IS NOT NULL THEN @referenceNumber ELSE ReferenceNumber END, " + 
                    "Title = @title, " + 
                    "StandardData = @StandardData, " + 
                    "DateUpdated = GETUTCDATE(), " + 
                    "DateRemoved = NULL, " + 
                    "IsLive = 1 " +
                "WHERE StandardId = @standardId",
                param: new { standard.StandardId, standard.ReferenceNumber, standard.Title, standard.StandardData },
                transaction: _unitOfWork.Transaction
            );

            await UpsertOptions(standard);
        }

        private async Task UpdateExistingStandard(StandardNonApprovedCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [StandardNonApprovedCollation] SET " + 
                    "ReferenceNumber = @referenceNumber, " + 
                    "Title = @title, " + 
                    "StandardData = @standardData, " + 
                    "DateUpdated = GETUTCDATE(), " + 
                    "DateRemoved = NULL, " + 
                    "IsLive = 1 " +
                "WHERE ReferenceNumber = @referenceNumber",
                param: new { standard.ReferenceNumber, standard.Title, standard.StandardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task InsertNewStandard(StandardCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                "VALUES (@standardId, @referenceNumber, @title, @standardData)",
                param: new { standard.StandardId, standard.ReferenceNumber, standard.Title, standard.StandardData },
                transaction: _unitOfWork.Transaction
            );

            await UpsertOptions(standard);
        }

        private async Task InsertNewStandard(StandardNonApprovedCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [StandardNonApprovedCollation] ([ReferenceNumber] ,[Title],[StandardData]) " +
                "VALUES (@referenceNumber, @title, @standardData)",
                param: new { standard.ReferenceNumber, standard.Title, standard.StandardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task UpdateExistingStandardToRemoved(StandardCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [StandardCollation] SET " + 
                    "IsLive = 0, " + 
                    "DateRemoved = GETUTCDATE() " +
                "WHERE StandardId = @standardId",
                param: new { standard.StandardId },
                transaction: _unitOfWork.Transaction
            );

            await UpsertOptions(standard, true);
        }

        private async Task UpdateExistingStandardToRemoved(StandardNonApprovedCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                    "UPDATE [StandardNonApprovedCollation] SET " + 
                        "IsLive = 0, " + 
                        "DateRemoved = GETUTCDATE() " +
                    "WHERE ReferenceNumber = @referenceNumber",
                    param: new { standard.ReferenceNumber },
                    transaction: _unitOfWork.Transaction
                );
        }

        private async Task UpsertOptions(StandardCollation standard, bool removingStandard = false)
        {
            var existingOptions = (await GetStandardCollationsInternal(standard.StandardId.Value))?.SingleOrDefault()?.Options;

            var optionsRemoved = removingStandard 
                ? existingOptions
                : existingOptions
                    .Where(existingOptionName => !standard.Options.Exists(optionName => optionName == existingOptionName));

            foreach (var optionName in optionsRemoved)
            {
                await UpdateExistingOptionToRemoved(standard, optionName);
            }

            // the OptionName is a natural key, the Id is created in assessor as the IFATE id of an option does not always exist
            // therefore updates are to add previously removed options back for a standard
            var optionsUpdated = standard.Options
                .Where(latestOptionName => existingOptions.Exists(existingOptionName => existingOptionName == latestOptionName));

            foreach (var optionName in optionsUpdated)
            {
                await UpdateExistingOption(standard, optionName);
            }

            var optionsInserted = standard.Options
                .Where(latestOptionName => !existingOptions.Exists(existingOptionName => existingOptionName == latestOptionName));
                
            foreach (var optionName in optionsInserted)
            {
                await InsertNewOption(standard, optionName);
            }
        }

        private async Task UpdateExistingOption(StandardCollation standard, string optionName)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Options] SET " +
                    // the OptionName is a natural key and cannot be updated
                    "DateUpdated = GETUTCDATE(), " +
                    "DateRemoved = NULL, " +
                    "IsLive = 1 " +
                "WHERE StdCode = @stdCode AND OptionName = @optionName",
                param: new { StdCode = standard.StandardId, optionName },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task InsertNewOption(StandardCollation standard, string optionName)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [Options] ([StdCode] ,[OptionName]) " +
                "VALUES (@stdCode, @optionName)",
                param: new { StdCode = standard.StandardId, optionName },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task UpdateExistingOptionToRemoved(StandardCollation standard, string optionName)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Options] SET " +
                    "IsLive = 0, " +
                    "DateRemoved = GETUTCDATE() " +
                "WHERE StdCode = @stdCode AND OptionName = @optionName",
                param: new { StdCode = standard.StandardId, optionName },
                transaction: _unitOfWork.Transaction
            );
        }
    }
}
