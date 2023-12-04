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
                    "CoronationEmblem = @coronationEmblem, " +
                    "Status = @status, " +
                    "TypicalDuration = @typicalDuration, " +
                    "MaxFunding = @maxFunding, " +
                    "IsActive = @isActive, " +
                    "LastDateStarts = @lastDateStarts, " +
                    "EffectiveFrom = @effectiveFrom, " +
                    "EffectiveTo = @effectiveTo, " +
                "WHERE StandardUId = @standardUId",
                param: new { standard.StandardUId, standard.IfateReferenceNumber, standard.LarsCode, standard.Title, standard.Version, standard.Level, standard.CoronationEmblem, standard.Status, standard.TypicalDuration, standard.MaxFunding, standard.IsActive, standard.LastDateStarts, standard.EffectiveFrom, standard.EffectiveTo },
                transaction: _unitOfWork.Transaction);
        }

        public async Task<IEnumerable<Standard>> GetAllStandards()
        {
			var sql = @"SELECT [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
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
[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
[EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
[VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding],[EPAChanged],[StandardPageUrl] 
FROM 
(
	SELECT ROW_NUMBER() OVER(PARTITION BY [IFateReferenceNumber] ORDER BY [dbo].[ExpandedVersion](Version) DESC) AS RowNum,
	[StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
	[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
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
[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
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
[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
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
[Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
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
                        SELECT ROW_NUMBER() OVER (PARTITION BY [IfateReferenceNumber] ORDER BY [dbo].[ExpandedVersion](Version) DESC ) rownumber
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
                                     [Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
                                     [EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
                                     [VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding]  
                        FROM [Standards] WHERE [LarsCode] = @larsCode ORDER BY [dbo].[ExpandedVersion](Version) desc";

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Standard>(
                sql,
                param: new { larsCode },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        private async Task<Standard> GetLatestStandardVersionByIFateReferenceNumberInternal(string iFateReferenceNumber)
        {
            var sql = @"SELECT TOP 1 [StandardUId],[IFateReferenceNumber],[LarsCode],[Title],[Version],
                                     [Level],[CoronationEmblem],[Status],[TypicalDuration],[MaxFunding],[IsActive],[LastDateStarts],
                                     [EffectiveFrom],[EffectiveTo],[VersionEarliestStartDate],[VersionLatestStartDate],[VersionLatestEndDate],
                                     [VersionApprovedForDelivery],[ProposedTypicalDuration],[ProposedMaxFunding]
                        FROM [Standards] WHERE [IfateReferenceNumber] = @iFateReferenceNumber ORDER BY [dbo].[ExpandedVersion](Version) desc";

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Standard>(
                sql,
                param: new { iFateReferenceNumber },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        private async Task<Standard> GetStandardByIFateReferenceNumberAndVersionInternal(string ifateReferenceNumber, string version)
        {
            var sql = @"SELECT [StandardUId], [IfateReferenceNumber], [LarsCode], [Title], [Version], [Level], [CoronationEmblem], [Status], [TypicalDuration], 
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
            var sql = @"SELECT [StandardUId], [IfateReferenceNumber], [LarsCode], [Title], [Version], [Level], [CoronationEmblem], [Status], [TypicalDuration], 
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

        public async Task<int> LoadOfqualStandards(DateTime dateTimeUtc)
        {
            var @params = new DynamicParameters();
            @params.Add("dateTimeUtc", dateTimeUtc);
            @params.Add("inserted", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await _unitOfWork.Connection.ExecuteAsync(
                "Load_Ofqual_Standards",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            int result = @params.Get<int>("inserted");

            return result;
        }

        public async Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(
            string endPointAssessorOrganisationId, int pageSize, int pageIndex)
        {
            return await GetEpaoRegisteredStandards(endPointAssessorOrganisationId, true, pageSize, pageIndex);
        }

        public async Task<EpoRegisteredStandardsResult> GetEpaoRegisteredStandards(
            string endPointAssessorOrganisationId, bool requireAtLeastOneVersion, int pageSize, int pageIndex)
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
                    RequireAtLeastOneVersion = requireAtLeastOneVersion,
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
                        AND (osv.EffectiveTo IS NULL OR osv.EffectiveTo > GETDATE()) 
                        ORDER BY s.VersionMajor, s.VersionMinor";

            var results = await _unitOfWork.Connection.QueryAsync<OrganisationStandardVersion>(
                sql,
                param: new { endPointAssessorOrganisationId, larsCode },
                transaction: _unitOfWork.Transaction);

            return results;
        }

        public async Task<IEnumerable<StandardVersion>> GetEpaoRegisteredStandardVersionsByIFateReferenceNumber(string endPointAssessorOrganisationId, string iFateReferenceNumber)
        {

            var sql = @"SELECT osv.StandardUId, os.StandardCode as LarsCode, s.Title, s.Level, s.IFateReferenceNumber, s.Version, osv.EffectiveFrom, osv.EffectiveTo, osv.DateVersionApproved, osv.Status
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

        public async Task<EpaoPipelineStandardsResult> GetEpaoPipelineStandards(string endPointAssessorOrganisationId, string standardFilterId, string providerFilterId, string epaDateFilterId,
            int pipelineCutoff, string orderBy, string orderDirection, int pageSize, int? pageIndex)
        {
            IEnumerable<EpaoPipelineStandard> epaoPipelines;
            var epaoPipelineStandardsResult = new EpaoPipelineStandardsResult
            {
                PageOfResults = new List<EpaoPipelineStandard>(),
                TotalCount = 0
            };

            if (string.IsNullOrWhiteSpace(standardFilterId) || standardFilterId.Trim().ToUpper() == "ALL")
            {
                standardFilterId = null;
            }
            if (string.IsNullOrWhiteSpace(providerFilterId) || providerFilterId.Trim().ToUpper() == "ALL")
            {
                providerFilterId = null;
            }
            if (string.IsNullOrWhiteSpace(epaDateFilterId) || epaDateFilterId.Trim().ToUpper() == "ALL")
            {
                epaDateFilterId = null;
            }

            var skip = ((pageIndex ?? 1) - 1) * pageSize;
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandard>(
                "GetEPAO_Pipelines",
                param: new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    standardFilterId = standardFilterId,
                    providerFilterId = providerFilterId,
                    epaDateFilterId = epaDateFilterId,
                    pipelineCutoff
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

        public async Task<IEnumerable<EpaoPipelineStandardFilter>> GetEpaoPipelineStandardsStandardFilter(string endPointAssessorOrganisationId, int pipelineCutOff)
        {
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandardFilter>(
                "GetEPAO_Pipelines_StandardFilter",
                param: new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    pipelineCutOff = pipelineCutOff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result?.ToList();
        }

        public async Task<IEnumerable<EpaoPipelineStandardFilter>> GetEpaoPipelineStandardsProviderFilter(string endPointAssessorOrganisationId, int pipelineCutOff)
        {
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandardFilter>(
                "GetEPAO_Pipelines_ProviderFilter",
                param: new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    pipelineCutOff = pipelineCutOff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result?.ToList();
        }

        public async Task<IEnumerable<EpaoPipelineStandardFilter>> GetEpaoPipelineStandardsEPADateFilter(string endPointAssessorOrganisationId, int pipelineCutOff)
        {
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandardFilter>(
                "GetEPAO_Pipelines_EPADateFilter",
                param: new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    pipelineCutOff = pipelineCutOff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result?.ToList();
        }

        public async Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId, string standardFilterId, string providerFilterId, string epaDateFilterId, int pipelineCutoff)
        {
            var result = await _unitOfWork.Connection.QueryAsync<EpaoPipelineStandardExtract>(
                "GetEPAO_Pipelines_Extract",
                param: new
                {
                    epaOrgId = endPointAssessorOrganisationId,
                    standardFilterId = standardFilterId,
                    providerFilterId = providerFilterId,
                    epaDateFilterId = epaDateFilterId,
                    pipelineCutoff
                },
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
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
            dataTable.Columns.Add("TrailBlazerContact");
            dataTable.Columns.Add("Route");
            dataTable.Columns.Add("VersionMajor");
            dataTable.Columns.Add("VersionMinor");
            dataTable.Columns.Add("IntegratedDegree");
            dataTable.Columns.Add("EqaProviderName");
            dataTable.Columns.Add("EqaProviderContactName");
            dataTable.Columns.Add("EqaProviderContactEmail]");
            dataTable.Columns.Add("OverviewOfRole]");
            dataTable.Columns.Add("CoronationEmblem");

            foreach (var standard in standards)
            {
                dataTable.Rows.Add(standard.StandardUId, standard.IfateReferenceNumber, standard.LarsCode, standard.Title, standard.Version, standard.Level,
                    standard.Status, standard.TypicalDuration, standard.MaxFunding, standard.IsActive, standard.LastDateStarts, standard.EffectiveFrom, standard.EffectiveTo,
                    standard.VersionEarliestStartDate, standard.VersionLatestStartDate, standard.VersionLatestEndDate, standard.VersionApprovedForDelivery,
                    standard.ProposedTypicalDuration, standard.ProposedMaxFunding, standard.EPAChanged, standard.StandardPageUrl, standard.TrailBlazerContact, standard.Route, 
                    standard.VersionMajor, standard.VersionMinor,
                    standard.IntegratedDegree, standard.EqaProviderName, standard.EqaProviderContactName, standard.EqaProviderContactEmail, standard.OverviewOfRole, 
                    standard.CoronationEmblem);
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

        public async Task<bool> GetCoronationEmblemForStandardReferenceAndVersion(string standardReference, string version)
        {
            var sql = @"SELECT [CoronationEmblem]
                       FROM [Standards] 
                       WHERE [IFateReferenceNumber] = @standardReference AND Version = @version";

            var result = await _unitOfWork.Connection.QuerySingleAsync<bool>(
                sql,
                param: new { standardReference, version },
                transaction: _unitOfWork.Transaction);

            return result;
        }

        public async Task<string> GetTitleForStandardReferenceAndVersion(string standardReference, string version)
        {
            var sql = @"SELECT [Title]
                       FROM [Standards] 
                       WHERE [IFateReferenceNumber] = @standardReference AND Version = @version";

            var results = await _unitOfWork.Connection.QueryAsync<string>(
                sql,
                param: new { standardReference, version },
                transaction: _unitOfWork.Transaction);

            return results.First();
        }
    }
}
