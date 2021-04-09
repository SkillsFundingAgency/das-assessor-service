using Dapper;
using Microsoft.EntityFrameworkCore;
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
        public StandardRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
            SqlMapper.AddTypeHandler(typeof(StandardNonApprovedData), new StandardNonApprovedDataHandler());
        }

        public async Task Insert(Standard standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [Standards] ([StandardUId], [IfateReferenceNumber], [LarsCode], [Title], [Version], [Level], [Status], [TypicalDuration], [MaxFunding], [IsActive], [LastDateStarts], [EffectiveFrom], [EffectiveTo], [VersionEarliestStartDate], [VersionLatestStartDate], [VersionLatestEndDate], [VersionApprovedForDelivery], [ProposedTypicalDuration], [ProposedMaxFunding]) " +
                "VALUES (@standardUId, @ifateReferenceNumber, @larsCode, @title, @version, @level, @status, @typicalDuration, @maxFunding, @isActive, @lastDateStarts, @effectiveFrom, @effectiveTo, @versionEarliestStartDate, @versionLatestStartDate, @versionLatestEndDate, @versionApprovedForDelivery, @proposedTypicalDuration, @proposedMaxFunding)",
                param: new { standard.StandardUId, standard.IfateReferenceNumber, standard.LarsCode, standard.Title, standard.Version, standard.Level, standard.Status, standard.TypicalDuration, standard.MaxFunding, standard.IsActive, standard.LastDateStarts, standard.EffectiveFrom, standard.EffectiveTo, standard.VersionEarliestStartDate, standard.VersionLatestStartDate, standard.VersionLatestEndDate, standard.VersionApprovedForDelivery, standard.ProposedTypicalDuration, standard.ProposedMaxFunding },
                transaction: _unitOfWork.Transaction);
        }

        public async Task DeleteAll()
        {
            await _unitOfWork.Connection.ExecuteAsync("DELETE FROM Standards", transaction: _unitOfWork.Transaction);
        }

        public async Task Update(Standard standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "UPDATE [Standards] SET " +
                    "IfateReferenceNumber = @ifateReferenceNumber, " +
                    "LarsCode = @larsCode, " +
                    "Title = @title, " +
                    "Version = @version, " +
                    "Level = @level, " +
                    "Status = @status, " +
                    "TypicalDuration = @typicalDuration, " +
                    "MaxFunding = @maxFunding, " +
                    "IsActive = @isActive, " +
                    "LastDateStarts = @lastDateStarts, " +
                    "EffectiveFrom = @effectiveFrom, " +
                    "EffectiveTo = @effectiveTo, " +
                "WHERE StandardUId = @standardUId",
                param: new { standard.StandardUId, standard.IfateReferenceNumber, standard.LarsCode, standard.Title, standard.Version, standard.Level, standard.Status, standard.TypicalDuration, standard.MaxFunding, standard.IsActive, standard.LastDateStarts, standard.EffectiveFrom, standard.EffectiveTo },
                transaction: _unitOfWork.Transaction);
        }

        public async Task<List<StandardCollation>> GetStandardCollations()
        {
            return await GetStandardCollationsInternal(isLive: true);
        }

        public async Task<StandardCollation> GetStandardCollationByStandardId(int standardId)
        {
            var standards = await GetStandardCollationsInternal(standardIdFilter: standardId, isLive: true);
            return standards.FirstOrDefault();
        }

        public async Task<StandardCollation> GetStandardCollationByReferenceNumber(string referenceNumber)
        {
            var standards = await GetStandardCollationsInternal(referenceNumberFilter: referenceNumber, isLive: true);
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

        public async Task<Standard> GetStandardByStandardReferenceAndVersion(string standardReference, string version)
        {
            return await GetStandardsByStandardReferenceAndVersionInternal(standardReference, version);
        }

        public async Task<IEnumerable<Standard>> GetAllStandards()
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding] FROM [Standards]";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<Standard>> GetStandardVersionsByLarsCode(int larsCode)
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding] FROM [Standards] Where [LarsCode] = @larsCode";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                param: new { larsCode },
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<Standard> GetStandardVersionByStandardUId(string standardUId)
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding] FROM [Standards] Where [StandardUId] = @standardUId";

            var result = await _unitOfWork.Connection.QuerySingleAsync<Standard>(
                sql,
                param: new { standardUId },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        private async Task<Standard> GetStandardsByStandardReferenceAndVersionInternal(string standardReference, string version)
        {
            var sql = @"SELECT StandardUId, IfateReferenceNumber, LarsCode, Title, Version, Level, Status, TypicalDuration, 
                            MaxFunding, IsActive, LastDateStarts, EffectiveTo, EffectiveFrom, VersionEarliestStartDate, 
                            VersionLatestStartDate, VersionLatestEndDate, VersionApprovedForDelivery, ProposedTypicalDuration, ProposedMaxFunding
                       FROM [Standards] WHERE IFateReferenceNumber = @standardReference AND Version = @version";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                param: new { standardReference, version },
                transaction: _unitOfWork.Transaction);

            return results.FirstOrDefault();
        }

        public async Task<List<Option>> GetOptions(int stdCode)
        {
            return await GetOptionsInternal(new List<int> { stdCode }, true);
        }

        public async Task<List<Option>> GetOptions(List<int> stdCodes)
        {
            return await GetOptionsInternal(stdCodes, true);
        }

        private async Task<List<Option>> GetOptionsInternal(List<int> stdCodes, bool? isLive = null)
        {
            var sql = "SELECT * FROM [Options] WHERE StdCode IN @stdCodes";
            if (isLive.HasValue)
            {
                sql += " AND IsLive = @isLive";
            }

            var results = await _unitOfWork.Connection.QueryAsync<Option>(
                sql,
                param: new { stdCodes, isLive = (isLive ?? true) ? 1 : 0 },
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        private async Task<List<StandardCollation>> GetStandardCollationsInternal(int? standardIdFilter = null, string referenceNumberFilter = null, bool? isLive = null)
        {
            var standardsDictionary = new Dictionary<string, StandardCollation>();

            var sql = new SqlQuery($"SELECT * FROM [StandardCollation] {SqlQuery.PredicatePlaceholder}");

            if (standardIdFilter.HasValue)
            {
                sql.Predicates.Add("StandardId = @standardIdFilter");
            }
            else if (referenceNumberFilter != null)
            {
                sql.Predicates.Add("ReferenceNumber = @referenceNumberFilter");
            }

            if (isLive.HasValue)
            {
                sql.Predicates.Add("IsLive = @isLive");
            }

            var standards = await _unitOfWork.Connection.QueryAsync<StandardCollation>(
                sql.SqlWithOptionalPredicates(),
                param: new { standardIdFilter, referenceNumberFilter, isLive = (isLive ?? true) ? 1 : 0 },
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
            var options = await GetOptionsInternal(standardCodes, isLive);

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

        private async Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollationsInternal(string referenceNumberFilter = null, bool? isLive = null)
        {
            var sql = new SqlQuery($"SELECT * FROM StandardNonApprovedCollation {SqlQuery.PredicatePlaceholder}");
            if (referenceNumberFilter != null)
            {
                sql.Predicates.Add("ReferenceNumber = @referenceNumberFilter");
            }

            if (isLive.HasValue)
            {
                sql.Predicates.Add("IsLive = @isLive");
            }

            var results = await _unitOfWork.Connection.QueryAsync<StandardNonApprovedCollation>(
                sql.SqlWithOptionalPredicates(),
                param: new { referenceNumberFilter, isLive = (isLive ?? true) ? 1 : 0 },
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<string> UpsertApprovedStandards(List<StandardCollation> latestStandards)
        {
            // retrieving both live and non-live, so that an existing non-live approved standard can be re-activated
            var existingStandards = await GetStandardCollationsInternal(isLive: null);

            var standardsToBeRemoved = existingStandards
                .Where(existingStandard => !latestStandards.Any(latestStandard => existingStandard.StandardId.Equals(latestStandard.StandardId)))
                .ToList();

            foreach (var standard in standardsToBeRemoved)
            {
                await UpdateExistingStandardToRemoved(standard);
            }

            var standardsToBeUpdated = latestStandards
                .Where(latestStandard => existingStandards.Any(existingStandard => existingStandard.StandardId.Equals(latestStandard.StandardId)))
                .ToList();

            foreach (var standard in standardsToBeUpdated)
            {
                await UpdateExistingStandard(standard);
            }

            var standardsToBeAdded = latestStandards
                .Where(latestStandard => !existingStandards.Any(existingStandard => existingStandard.StandardId.Equals(latestStandard.StandardId)))
                .ToList();

            foreach (var standard in standardsToBeAdded)
            {
                await InsertNewStandard(standard);
            }

            return $"Approved Standards: Added: {standardsToBeAdded.Count}; Updated: {standardsToBeUpdated.Count}; Removed: {standardsToBeRemoved.Count}";
        }

        public async Task<string> UpsertNonApprovedStandards(List<StandardNonApprovedCollation> latestStandards)
        {
            // retrieving both live and non-live, so that an existing non-live non-approved standard can be re-activated
            var existingStandards = await GetStandardNonApprovedCollationsInternal(isLive: null);

            var standardsToBeRemoved = existingStandards
                .Where(existingStandard => !latestStandards.Any(latestStandard => existingStandard.ReferenceNumber.Equals(latestStandard.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsToBeRemoved)
            {
                await UpdateExistingStandardToRemoved(standard);
            }

            var standardsToBeUpdated = latestStandards
                .Where(latestStandard => existingStandards.Any(existingStandard => existingStandard.ReferenceNumber.Equals(latestStandard.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsToBeUpdated)
            {
                await UpdateExistingStandard(standard);
            }

            var standardsToBeAdded = latestStandards
                .Where(ls => !existingStandards.Any(es => es.ReferenceNumber.Equals(ls.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsToBeAdded)
            {
                await InsertNewStandard(standard);
            }

            return $"Non-approved Standards: Added: {standardsToBeAdded.Count}; Updated: {standardsToBeUpdated.Count}; Removed: {standardsToBeRemoved.Count}";
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

        public async Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(
            string endPointAssessorOrganisationId, int pageSize, int pageIndex)
        {
            var epoRegisteredStandardsResult = new EpoRegisteredStandardsResult
            {
                PageOfResults = new List<EPORegisteredStandards>(),
                TotalCount = 0
            };
            var total = await GetEpaoStandardsCount(endPointAssessorOrganisationId);
            var skip = (pageIndex - 1) * pageSize;
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
                    epaOrgId = endPointAssessorOrganisationId
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
                    epaOrgId = endPointAssessorOrganisationId
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
            // retrieving both live and non-live, so that an existing non-live option can be re-activated
            var existingOptions = await GetOptionsInternal(new List<int> { standard.StandardId.Value }, isLive: null);

            var optionsToBeRemoved = removingStandard
                ? existingOptions
                : existingOptions
                    .Where(existingOptionName => !standard.Options.Any(optionName => optionName.Equals(existingOptionName.OptionName, StringComparison.InvariantCulture)));

            foreach (var option in optionsToBeRemoved)
            {
                await UpdateExistingOptionToRemoved(standard, option.OptionName);
            }

            // the OptionName is a natural key, the Id is created in assessor as the IFATE id of an option does not always exist
            // therefore updates are to add previously removed options back for a standard
            var optionsToBeUpdated = standard.Options
                .Where(latestOptionName => existingOptions.Any(existingOptionName => existingOptionName.OptionName.Equals(latestOptionName, StringComparison.InvariantCulture)));

            foreach (var optionName in optionsToBeUpdated)
            {
                await UpdateExistingOption(standard, optionName);
            }

            var optionsToBeInserted = standard.Options
                .Where(latestOptionName => !existingOptions.Any(existingOptionName => existingOptionName.OptionName.Equals(latestOptionName, StringComparison.InvariantCulture)));

            foreach (var optionName in optionsToBeInserted)
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
                "WHERE StdCode = @stdCode AND OptionName = @optionName COLLATE SQL_Latin1_General_CP1_CS_AS",
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
                "WHERE StdCode = @stdCode AND OptionName = @optionName COLLATE SQL_Latin1_General_CP1_CS_AS",
                param: new { StdCode = standard.StandardId, optionName },
                transaction: _unitOfWork.Transaction
            );
        }
    }
}
