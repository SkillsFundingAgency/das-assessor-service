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
    public class OppFinderRepository : Repository, IOppFinderRepository
    {
        public OppFinderRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }

        public async Task<bool> CreateExpressionOfInterest(string standardReference, string email, string organisationName, string contactName, string contactNumber)
        {
            var rowAffected = await _unitOfWork.Connection.ExecuteAsync(
                "INSERT INTO [ExpressionsOfInterest] ([Email],[ContactName] ,[ContactPhone],[OrganisationName],[StandardReference]) " +
                $@"VALUES (@email, @contactName, @contactNumber, @organisationName, @standardReference)",
                param: new { email, contactName, contactNumber, organisationName, standardReference },
                transaction: _unitOfWork.Transaction
            );

            return (rowAffected > 0);
        }

        public async Task<OppFinderApprovedStandardDetailsResult> GetOppFinderApprovedStandardDetails(string standardReference)
        {
            var @params = new DynamicParameters();
            @params.Add("standardReference", standardReference);

            var filterResults = (await _unitOfWork.Connection.QueryMultipleAsync(
                "OppFinder_Get_Approved_Standard_Details",
                param: @params,
                transaction: _unitOfWork.Transaction,
                commandType: CommandType.StoredProcedure));

            var filterStandardsResult = new OppFinderApprovedStandardDetailsResult
            {
                OverviewResult = filterResults.Read<OppFinderApprovedStandardOverviewResult>().FirstOrDefault(),
                RegionResults = filterResults.Read<OppFinderApprovedStandardRegionResult>()?.ToList(),
                VersionResults = filterResults.Read<OppFinderApprovedStandardVersionResult>()?.ToList()
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

        public async Task UpdateStandardSummary()
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "OppFinder_Update_StandardSummary",
                param: null,
                transaction: _unitOfWork.Transaction,
                commandTimeout: 0,
                commandType: CommandType.StoredProcedure);
        }
    }
}
