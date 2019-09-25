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

        public async Task<List<StandardNonApprovedCollation>> GetStandardNonApprovedCollations()
        {
            return await GetStandardNonApprovedCollationsInternal();
        }

        public async Task<StandardNonApprovedCollation> GetStandardNonApprovedCollationByReferenceNumber(string referenceNumber)
        {
            var standards = await GetStandardNonApprovedCollationsInternal(referenceNumberFilter: referenceNumber);
            return standards.FirstOrDefault();
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

        private async Task<List<StandardCollation>> GetStandardCollationsInternal(int? standardIdFilter = null, string referenceNumberFilter = null)
        {
            var standardsDictionary = new Dictionary<string, StandardCollation>();

            string standardsQuery = @"SELECT * FROM StandardCollation";

            if (standardIdFilter != null)
            {
                standardsQuery += " WHERE StandardId = @standardIdFilter";
            }
            else if (referenceNumberFilter != null)
            {
                standardsQuery += " WHERE ReferenceNumber = @referenceNumberFilter";
            }

            var standards = await _unitOfWork.Connection.QueryAsync<StandardCollation>(
                standardsQuery, 
                param: new { standardIdFilter, referenceNumberFilter }, 
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

            var optionsQuery = "SELECT * FROM Options WHERE StdCode IN @standardCodes";
            var standardCodes = standardsDictionary.Values.Where(v => v.StandardId.HasValue).Select(v => v.StandardId).ToList();
            var options = await _unitOfWork.Connection.QueryAsync<Option>(
                optionsQuery, 
                param: new { standardCodes }, 
                transaction: _unitOfWork.Transaction);

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
            string standardsQuery = @"SELECT * FROM StandardNonApprovedCollation";

            if (referenceNumberFilter != null)
            {
                standardsQuery += " WHERE ReferenceNumber = @referenceNumberFilter";
            }

            var results = await _unitOfWork.Connection.QueryAsync<StandardNonApprovedCollation>(
                standardsQuery, 
                param: new { referenceNumberFilter }, 
                transaction: _unitOfWork.Transaction);

            return results.ToList();
        }

        public async Task<DateTime?> GetDateOfLastStandardCollation()
        {
            const string sql = "select top 1 coalesce(max(DateUpdated), max(DateAdded)) maxDate  from StandardCollation";
            var dateOfLastCollation = await _unitOfWork.Connection.QuerySingleAsync<DateTime?>(
                sql, 
                param: null, 
                transaction: _unitOfWork.Transaction);

            return dateOfLastCollation;
        }

        public async Task<string> UpsertApprovedStandards(List<StandardCollation> latestStandards)
        {
            var existingStandards = await GetStandardCollations();

            var standardsDeleted = existingStandards
                .Where(es => !latestStandards.Select(ls => ls.StandardId).Contains(es.StandardId))
                .ToList();

            foreach (var standard in standardsDeleted)
            {
                await UpdateExistingStandardToRemoved(standard);
            }

            var standardsUpdated = latestStandards
                .Where(ls => existingStandards.Any(es => es.StandardId.Equals(ls.StandardId)))
                .ToList();

            foreach(var standard in standardsUpdated)
            {
                await UpdateExistingStandard(standard, JsonConvert.SerializeObject(standard.StandardData));
            }

            var standardsInserted = latestStandards
                .Where(ls => !existingStandards.Any(es => es.StandardId.Equals(ls.StandardId)))
                .ToList();

            foreach (var standard in standardsInserted)
            {
                await InsertNewStandard(standard, JsonConvert.SerializeObject(standard.StandardData));
            }

            return $"details of approved update: Number of Inserts: {standardsInserted.Count}; Number of Updates: {standardsUpdated.Count}; Number of Removes: {standardsDeleted.Count}";
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
                await UpdateExistingStandard(standard, JsonConvert.SerializeObject(standard.StandardData));
            }

            var standardsInserted = latestStandards
                .Where(ls => !existingStandards.Any(es => es.ReferenceNumber.Equals(ls.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var standard in standardsInserted)
            {
                await InsertNewStandard(standard, JsonConvert.SerializeObject(standard.StandardData));
            }

            return $"details of non-approved update: Number of Inserts: {standardsInserted.Count}; Number of Updates: {standardsUpdated.Count}; Number of Removes: {standardsDeleted.Count}";
        }

        public async Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId)
        {
            var @params = new DynamicParameters();
            @params.Add("EPAOId", endPointAssessorOrganisationId);
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

        public async Task<OppFinderApprovedStandardDetailsResult> GetOppFinderApprovedStandardDetails(int standardCode)
        {
            var @params = new DynamicParameters();
            @params.Add("standardCode", standardCode);
            
            var filterResults = (await _unitOfWork.Connection.QueryMultipleAsync(
                "OppFinder_Get_Approved_Standard_Details",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var filterStandardsResult = new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = filterResults.Read<OppFinderApprovedStandardOverviewResult>().FirstOrDefault(),
                RegionResults = filterResults.Read<OppFinderApprovedStandardRegionResult>().ToList()
            };

            return filterStandardsResult;
        }

        public async Task<OppFinderFilterStandardsResult> GetOppFinderFilterStandards(string searchTerm, string sectorFilters, string levelFilters)
        {
            var @params = new DynamicParameters();
            @params.Add("searchTerm", searchTerm);
            @params.Add("sectorFilters", sectorFilters);
            @params.Add("levelFilters", levelFilters);

            var filterResults = (await _unitOfWork.Connection.QueryMultipleAsync(
                "OppFinder_Filter_Standards",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var filterStandardsResult = new OppFinderFilterStandardsResult
            {
                MatchingSectorFilterResults = filterResults.Read<OppFinderSectorFilterResult>().ToList(),
                MatchingLevelFilterResults = filterResults.Read<OppFinderLevelFilterResult>().ToList()
            };

            return filterStandardsResult;
        }

        public async Task<OppFinderApprovedStandardsResult> GetOppFinderApprovedStandards(string searchTerm, string sectorFilters, string levelFilters, 
            string sortColumn, int sortAscending, int pageSize, int pageIndex)
        {
            var @params = new DynamicParameters();
            @params.Add("searchTerm", searchTerm);
            @params.Add("sectorFilters", sectorFilters);
            @params.Add("levelFilters", levelFilters);
            @params.Add("sortColumn", sortColumn);
            @params.Add("sortAscending", sortAscending);
            @params.Add("pageSize", pageSize);
            @params.Add("pageIndex", pageIndex);
            @params.Add("totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var approvedResults = (await _unitOfWork.Connection.QueryAsync<OppFinderApprovedStandard>(
                "OppFinder_List_Approved_Standards",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var approvedStandardsResult = new OppFinderApprovedStandardsResult
            {
                PageOfResults = approvedResults?.ToList() ?? new List<OppFinderApprovedStandard>(),
                TotalCount = @params.Get<int?>("totalCount") ?? 0
            };
 
            return approvedStandardsResult;
        }

        public async Task<OppFinderNonApprovedStandardsResult> GetOppFinderNonApprovedStandards(string searchTerm, string sectorFilters, string levelFilters, 
            string sortColumn, int sortAscending, int pageSize, int pageIndex, string nonApprovedType)
        {
            var @params = new DynamicParameters();
            @params.Add("searchTerm", searchTerm);
            @params.Add("sectorFilters", sectorFilters);
            @params.Add("levelFilters", levelFilters);
            @params.Add("sortColumn", sortColumn);
            @params.Add("sortAscending", sortAscending);
            @params.Add("pageSize", pageSize);
            @params.Add("pageIndex", pageIndex);
            @params.Add("nonApprovedtype", nonApprovedType);
            @params.Add("totalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var nonApprovedResults = (await _unitOfWork.Connection.QueryAsync<OppFinderNonApprovedStandard>(
                "OppFinder_List_NonApproved_Standards",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var nonApprovedStandardsResult = new OppFinderNonApprovedStandardsResult
            {
                PageOfResults = nonApprovedResults?.ToList() ?? new List<OppFinderNonApprovedStandard>(),
                TotalCount = @params.Get<int?>("totalCount") ?? 0
            };

            return nonApprovedStandardsResult;
        }

        private async Task UpdateExistingStandard(StandardCollation standard, string standardData)
        {
            // when new ReferenceNumber is null (IFA has not supplied one) retain the current ReferenceNumber
            await _unitOfWork.Connection.ExecuteAsync(
                "Update [StandardCollation] set ReferenceNumber = case when @referenceNumber is not null then @referenceNumber else ReferenceNumber end, Title = @Title, StandardData = @StandardData, DateUpdated=getutcdate(), DateRemoved=null, IsLive = 1 " +
                "where StandardId = @standardId",
                param: new { standard.StandardId, standard.ReferenceNumber, standard.Title, standardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task UpdateExistingStandard(StandardNonApprovedCollation standard, string standardData)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "Update [StandardNonApprovedCollation] set ReferenceNumber = @referenceNumber, Title = @Title, StandardData = @StandardData, DateUpdated=getutcdate(), DateRemoved=null, IsLive = 1 " +
                "where ReferenceNumber = @referenceNumber",
                param: new { standard.ReferenceNumber, standard.Title, standardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task InsertNewStandard(StandardCollation standard, string standardData)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                $@"VALUES (@standardId, @referenceNumber, @Title, @standardData)",
                param: new { standard.StandardId, standard.ReferenceNumber, standard.Title, standardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task InsertNewStandard(StandardNonApprovedCollation standard, string standardData)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [StandardNonApprovedCollation] ([ReferenceNumber] ,[Title],[StandardData]) " +
                $@"VALUES (@referenceNumber, @Title, @standardData)",
                param: new { standard.ReferenceNumber, standard.Title, standardData },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task UpdateExistingStandardToRemoved(StandardCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "Update [StandardCollation] set IsLive=0, DateRemoved=getutcdate() " +
                "where StandardId = @standardId",
                param: new { standard.StandardId },
                transaction: _unitOfWork.Transaction
            );
        }

        private async Task UpdateExistingStandardToRemoved(StandardNonApprovedCollation standard)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                    "Update [StandardNonApprovedCollation] set IsLive=0, DateRemoved=getutcdate() " +
                    "where ReferenceNumber = @referenceNumber",
                    param: new { standard.ReferenceNumber },
                    transaction: _unitOfWork.Transaction
                );
        }
    }
}
