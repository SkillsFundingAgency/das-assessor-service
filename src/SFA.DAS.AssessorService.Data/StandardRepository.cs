using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public async Task InsertStandards(IEnumerable<Standard> standards)
        {
            var bulkCopyOptions = SqlBulkCopyOptions.TableLock;
            var dataTable = ConstructStandardsDataTable(standards);

            using (var bulkCopy = new SqlBulkCopy(_unitOfWork.Connection as SqlConnection, bulkCopyOptions, _unitOfWork.Transaction as SqlTransaction))
            {
                bulkCopy.DestinationTableName = "Standards";
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        public async Task InsertOptions(IEnumerable<StandardOption> optionsToInsert)
        {
            var bulkCopyOptions = SqlBulkCopyOptions.TableLock;
            var dataTable = ConstructStandardOptionsDataTable(optionsToInsert);

            using (var bulkCopy = new SqlBulkCopy(_unitOfWork.Connection as SqlConnection, bulkCopyOptions, _unitOfWork.Transaction as SqlTransaction))
            {
                bulkCopy.DestinationTableName = "StandardOptions";
                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }

        public async Task DeleteAllStandards()
        {
            await _unitOfWork.Connection.ExecuteAsync("DELETE FROM Standards", transaction: _unitOfWork.Transaction);
        }

        public async Task DeleteAllOptions()
        {
            await _unitOfWork.Connection.ExecuteAsync("DELETE FROM StandardOptions", transaction: _unitOfWork.Transaction);
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

        public async Task<IEnumerable<Standard>> GetAllStandards()
        {
			var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] FROM [Standards]
            WHERE [VersionApprovedForDelivery] IS NOT NULL";


          var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<Standard>> GetLatestStandardVersions()
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] 
FROM 
(
	SELECT ROW_NUMBER() OVER(PARTITION BY [IFateReferenceNumber] ORDER BY Version DESC) AS RowNum,
	[StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
	[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
	[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
	[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl]
	FROM [Standards]
    WHERE [VersionApprovedForDelivery] IS NOT NULL
) AS Stds
WHERE RowNum = 1";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<Standard>> GetStandardVersionsByIFateReferenceNumber(string iFateReferenceNumber)
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] 
FROM [Standards] Where [IFateReferenceNumber] = @iFateReferenceNumber";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                param: new { iFateReferenceNumber },
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<Standard>> GetStandardVersionsByLarsCode(int larsCode)
        {
            var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] FROM [Standards] Where [LarsCode] = @larsCode";

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
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] FROM [Standards] Where [StandardUId] = @standardUId";

            var result = await _unitOfWork.Connection.QuerySingleAsync<Standard>(
                sql,
                param: new { standardUId },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        public async Task<Standard> GetStandardVersionByLarsCode(int larsCode, string version = null)
        {
            if (!string.IsNullOrWhiteSpace(version))
            {
                return await GetStandardByLarsCodeAndVersionInternal(larsCode, version);
            }

            return await GetLatestStandardVersionByLarsCodeInternal(larsCode);
        }

        public async Task<Standard> GetStandardVersionByIFateReferenceNumber(string iFateReferenceNumber, string version = null)
        {
            if (!string.IsNullOrWhiteSpace(version))
            {
                return await GetStandardByIFateReferenceNumberAndVersionInternal(iFateReferenceNumber, version);
            }

            return await GetLatestStandardVersionByIFateReferenceNumberInternal(iFateReferenceNumber);
        }

        public async Task<IEnumerable<StandardOptions>> GetAllStandardOptions()
        {
            var sql = @"SELECT [StandardUId],[Version],[IFateReferenceNumber],[LarsCode] FROM [Standards]
                        SELECT [StandardUId],[OptionName] FROM [StandardOptions]";

            var filterResults = await _unitOfWork.Connection.QueryMultipleAsync(
                sql,
                transaction: _unitOfWork.Transaction);

            var standards = filterResults.Read<Standard>();
            var options = filterResults.Read<StandardOption>();

            var results = standards.Select(s => new StandardOptions
            {
                StandardUId = s.StandardUId,
                StandardCode = s.LarsCode,
                StandardReference = s.IfateReferenceNumber,
                Version = s.Version,
                CourseOption = options.Where(o => o.StandardUId == s.StandardUId).Select(p => p.OptionName)
            });

            return results;
        }

        public async Task<IEnumerable<StandardOptions>> GetStandardOptionsForLatestStandardVersions()
        {
            var sql = @"SELECT ab1.[StandardUId]
                              ,[IfateReferenceNumber]
                              ,[LarsCode]
                              ,[Version],[OptionName] FROM (
                        SELECT ROW_NUMBER() OVER (PARTITION BY [IfateReferenceNumber] ORDER BY [Version] DESC ) rownumber
                        , [StandardUId]
                              ,[IfateReferenceNumber]
                              ,[LarsCode]
                              ,[Version]
                          FROM [dbo].[Standards]
                          ) ab1
                          JOIN StandardOptions so2 on ab1.[StandardUId] = so2.StandardUId
                          WHERE rownumber = 1
                          ORDER BY [IfateReferenceNumber],[OptionName]";

            var filterResults = await _unitOfWork.Connection.QueryMultipleAsync(
                sql,
                transaction: _unitOfWork.Transaction);

            var standardVersionOptions = filterResults.Read<StandardVersionOption>();

            var results = standardVersionOptions.GroupBy(version => new 
                { 
                    version.StandardUId, 
                    version.LarsCode, 
                    version.IfateReferenceNumber, 
                    version.Version 
                })
                .Select(group => new StandardOptions { 
                    StandardUId = group.Key.StandardUId,
                    StandardReference = group.Key.IfateReferenceNumber,
                    StandardCode = group.Key.LarsCode,
                    Version = group.Key.Version,
                    CourseOption = group.Select(opt => opt.OptionName)
                });
           
            return results;
        }

        public async Task<StandardOptions> GetStandardOptionsByStandardUId(string standardUId)
        {
            var sql = @"SELECT [StandardUId],[Version],[IFateReferenceNumber],[LarsCode] FROM [Standards] WHERE [StandardUId] = @standardUId
                        SELECT [StandardUId],[OptionName] FROM [StandardOptions] WHERE [StandardUId] = @standardUId";

            var filterResults = await _unitOfWork.Connection.QueryMultipleAsync(
                sql,
                param: new { standardUId },
                transaction: _unitOfWork.Transaction);

            var standard = filterResults.Read<Standard>().Single();
            var options = filterResults.Read<StandardOption>();

            var results = new StandardOptions
            {
                StandardUId = standard.StandardUId,
                StandardCode = standard.LarsCode,
                StandardReference = standard.IfateReferenceNumber,
                Version = standard.Version,
                CourseOption = options.Select(p => p.OptionName)
            };

            return results;
        }

        public async Task<StandardOptions> GetStandardOptionsByLarsCode(int larsCode)
        {
            var standard = await GetLatestStandardVersionByLarsCodeInternal(larsCode);
            return await GetStandardOptionsByStandardUId(standard.StandardUId);
        }

        public async Task<StandardOptions> GetStandardOptionsByIFateReferenceNumber(string iFateReferenceNumber)
        {
            var standard = await GetLatestStandardVersionByIFateReferenceNumberInternal(iFateReferenceNumber);
            return await GetStandardOptionsByStandardUId(standard.StandardUId);
        }

        private async Task<Standard> GetLatestStandardVersionByLarsCodeInternal(int larsCode)
        {
            var sql = @"SELECT TOP 1 [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
                                     [Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
                                     [EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
                                     [VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding]  
                        FROM [Standards] WHERE [LarsCode] = @larsCode ORDER BY [Version] desc";

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Standard>(
                sql,
                param: new { larsCode },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        private async Task<Standard> GetLatestStandardVersionByIFateReferenceNumberInternal(string iFateReferenceNumber)
        {
            var sql = @"SELECT TOP 1 [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
                                     [Level],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
                                     [EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
                                     [VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding]
                        FROM [Standards] WHERE [IfateReferenceNumber] = @iFateReferenceNumber ORDER BY [Version] desc";

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Standard>(
                sql,
                param: new { iFateReferenceNumber },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        private async Task<Standard> GetStandardByIFateReferenceNumberAndVersionInternal(string ifateReferenceNumber, string version)
        {
            var sql = @"SELECT [StandardUId], [IfateReferenceNumber], [LarsCode], [Title], [Version], [Level], [Status], [TypicalDuration], 
                            [MaxFunding], [IsActive], [LastDateStarts], [EffectiveTo], [EffectiveFrom], [VersionEarliestStartDate], 
                            [VersionLatestStartDate], [VersionLatestEndDate], [VersionApprovedForDelivery], [ProposedTypicalDuration], [ProposedMaxFunding]
                       FROM [Standards] WHERE [IfateReferenceNumber] = @ifateReferenceNumber AND Version = @version";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                param: new { ifateReferenceNumber, version },
                transaction: _unitOfWork.Transaction);

            return results.First();
        }

        private async Task<Standard> GetStandardByLarsCodeAndVersionInternal(int larsCode, string version)
        {
            var sql = @"SELECT [StandardUId], [IfateReferenceNumber], [LarsCode], [Title], [Version], [Level], [Status], [TypicalDuration], 
                            [MaxFunding], [IsActive], [LastDateStarts], [EffectiveTo], [EffectiveFrom], [VersionEarliestStartDate], 
                            [VersionLatestStartDate], [VersionLatestEndDate], [VersionApprovedForDelivery], [ProposedTypicalDuration], [ProposedMaxFunding]
                       FROM [Standards] WHERE LarsCode = @larsCode AND Version = @version";

            var results = await _unitOfWork.Connection.QueryAsync<Standard>(
                sql,
                param: new { larsCode, version },
                transaction: _unitOfWork.Transaction);

            return results.First();
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

        public async Task<IEnumerable<OrganisationStandardVersion>> GetEpaoRegisteredStandardVersions(string endPointAssessorOrganisationId)
        {

            var sql = @"SELECT osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version, osv.EffectiveFrom, osv.EffectiveTo, osv.DateVersionApproved, osv.Status
                        FROM [dbo].[OrganisationStandardVersion] osv
                        INNER JOIN [dbo].[OrganisationStandard] os on osv.OrganisationStandardId = os.Id
                        INNER JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId AND o.EndPointAssessorOrganisationId = @endPointAssessorOrganisationId
                        INNER JOIN [dbo].[Standards] s on osv.StandardUId = s.StandardUId
                        WHERE osv.Status = 'Live' AND os.status = 'Live'
                        AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE())
                        AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE())";

            var results = await _unitOfWork.Connection.QueryAsync<OrganisationStandardVersion>(
                sql,
                param: new { endPointAssessorOrganisationId },
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<OrganisationStandardVersion>> GetEpaoRegisteredStandardVersions(string endPointAssessorOrganisationId, int larsCode)
        {

            var sql = @"SELECT osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version, osv.EffectiveFrom, osv.EffectiveTo, osv.DateVersionApproved, osv.Status
                        FROM [dbo].[OrganisationStandardVersion] osv
                        INNER JOIN [dbo].[OrganisationStandard] os on osv.OrganisationStandardId = os.Id
                        INNER JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId AND o.EndPointAssessorOrganisationId = @endPointAssessorOrganisationId
                        INNER JOIN [dbo].[Standards] s on osv.StandardUId = s.StandardUId
                        WHERE osv.Status = 'Live' AND os.status = 'Live' AND os.StandardCode = @larsCode
                        AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE())
                        AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE())";

            var results = await _unitOfWork.Connection.QueryAsync<OrganisationStandardVersion>(
                sql,
                param: new { endPointAssessorOrganisationId, larsCode },
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(string endPointAssessorOrganisationId, string iFateReferenceNumber)
        {

            var sql = @"SELECT osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version
                        FROM [dbo].[OrganisationStandardVersion] osv
                        INNER JOIN [dbo].[OrganisationStandard] os on osv.OrganisationStandardId = os.Id
                        INNER JOIN [dbo].[Organisations] o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId AND o.EndPointAssessorOrganisationId = @endPointAssessorOrganisationId
                        INNER JOIN [dbo].[Standards] s on osv.StandardUId = s.StandardUId
                        WHERE osv.Status = 'Live' AND os.status = 'Live' AND s.IFateReferenceNumber = @iFateReferenceNumber
                        AND (os.EffectiveTo IS NULL OR os.EffectiveTo > GETDATE())
                        AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE())";

            var results = await _unitOfWork.Connection.QueryAsync<StandardVersion>(
                sql,
                param: new { endPointAssessorOrganisationId, iFateReferenceNumber },
                transaction: _unitOfWork.Transaction);

            return results;
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

        private DataTable ConstructStandardsDataTable(IEnumerable<Standard> standards)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("StandardUId");
            dataTable.Columns.Add("IfateReferenceNumber");
            dataTable.Columns.Add("LarsCode");
            dataTable.Columns.Add("Title");
            dataTable.Columns.Add("Version");
            dataTable.Columns.Add("Level");
            dataTable.Columns.Add("Status");
            dataTable.Columns.Add("TypicalDuration");
            dataTable.Columns.Add("MaxFunding");
            dataTable.Columns.Add("IsActive");
            dataTable.Columns.Add("LastDateStarts");
            dataTable.Columns.Add("EffectiveFrom");
            dataTable.Columns.Add("EffectiveTo");
            dataTable.Columns.Add("VersionEarliestStartDate");
            dataTable.Columns.Add("VersionLatestStartDate");
            dataTable.Columns.Add("VersionLatestEndDate");
            dataTable.Columns.Add("VersionApprovedForDelivery");
            dataTable.Columns.Add("ProposedTypicalDuration");
            dataTable.Columns.Add("ProposedMaxFunding");
            dataTable.Columns.Add("EPAChanged");
            dataTable.Columns.Add("StandardPageUrl");

            foreach (var standard in standards)
            {
                dataTable.Rows.Add(standard.StandardUId, standard.IfateReferenceNumber, standard.LarsCode, standard.Title, standard.Version, standard.Level,
                    standard.Status, standard.TypicalDuration, standard.MaxFunding, standard.IsActive, standard.LastDateStarts, standard.EffectiveFrom, standard.EffectiveTo,
                    standard.VersionEarliestStartDate, standard.VersionLatestStartDate, standard.VersionLatestEndDate, standard.VersionApprovedForDelivery,
                    standard.ProposedTypicalDuration, standard.ProposedMaxFunding, standard.EPAChanged, standard.StandardPageUrl);
            }

            return dataTable;
        }

        private DataTable ConstructStandardOptionsDataTable(IEnumerable<StandardOption> options)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("StandardUId");
            dataTable.Columns.Add("Option");

            foreach (var option in options)
            {
                dataTable.Rows.Add(option.StandardUId, option.OptionName);
            }

            return dataTable;
        }
    }
}
