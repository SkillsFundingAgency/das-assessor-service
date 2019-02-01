using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Data
{
    public class StandardRepository : IStandardRepository
    {

        private readonly IWebConfiguration _configuration;
        private readonly AssessorDbContext _assessorDbContext;
        private readonly IDbConnection _connection;

        public StandardRepository(IWebConfiguration configuration, AssessorDbContext assessorDbContext, IDbConnection connection)
        {
            _configuration = configuration;
            _assessorDbContext = assessorDbContext;
            _connection = connection;
            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
        }

        public async Task<List<StandardCollation>> GetStandardCollations()
        {
                var connectionString = _configuration.SqlConnectionString;

                using (var connection = new SqlConnection(connectionString))
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync();

                    var standards = await connection.QueryAsync<StandardCollation>("select * from [StandardCollation]");
                    return standards.ToList();
                } 
        }


        public async Task<DateTime?> GetDateOfLastStandardCollation()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                const string sql = "select top 1 coalesce(max(DateUpdated), max(DateAdded)) maxDate  from StandardCollation";
                var dateOfLastCollation = await connection.QuerySingleAsync<DateTime?>(sql);
                return dateOfLastCollation;
            }
        }

        public async Task<string> UpsertStandards(List<StandardCollation> standards)
        {

            var countInserted = 0;
            var countUpdated = 0;
            var countRemoved = 0;

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var currentStandards = await GetStandardCollations();
                countRemoved = UpdateContactsThatAreDeleted(connection, standards, currentStandards);

                foreach (var standard in standards)
                {
                    var isNew = true;
                    var standardData = JsonConvert.SerializeObject(standard.StandardData);
                    if (currentStandards.Any(x => x.StandardId == standard.StandardId))
                        isNew = false;

                    if (isNew)
                    {
                        countInserted++;
                        InsertNewStandard(connection, standard, standardData);
                    }
                    else
                    {
                        countUpdated++;
                        UpdateCurrentStandard(connection, standard, standardData);
                    }
                }
            }

            return $"details of update: Number of Inserts: {countInserted}; Number of Updates: {countUpdated}; Number of Removes: {countRemoved}";
        }

        public async Task<int> GetEpaoStandardsCount(string endPointAssessorOrganisationId)
        {

            var epaoId = new SqlParameter("@EPAOId", endPointAssessorOrganisationId);
            var count = new SqlParameter("@Count", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            await _assessorDbContext.Database.ExecuteSqlCommandAsync("EXEC EPAO_Standards_Count @EPAOId, @Count out", epaoId, count);
            return (int)count.Value;
        }

        public async Task<int> GetEpaoPipelineCount(string endPointAssessorOrganisationId)
        {
            var result = await _connection.QueryAsync<EpaoPipelineStandard>("GetEPAO_Pipelines", new
            {
                EPAOId = endPointAssessorOrganisationId
            },
                commandType: CommandType.StoredProcedure);

            var epaoPipelines = result?.ToList();
            if (epaoPipelines != null && epaoPipelines.Any())
            {
                return epaoPipelines.Select(x => x.TotalRows).First();
            }

            return 0;
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
            var result = await _connection.QueryAsync<EPORegisteredStandards>("EPAO_Registered_Standards", new
            {
                EPAOId = endPointAssessorOrganisationId,
                Skip = skip,
                Take = pageSize
            }, commandType: CommandType.StoredProcedure);
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
            var result = await _connection.QueryAsync<EpaoPipelineStandard>("GetEPAO_Pipelines", new
            {
                EPAOId = endPointAssessorOrganisationId
            },
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
        private static void UpdateCurrentStandard(SqlConnection connection, StandardCollation standard, string standardData)
        {
            connection.Execute(
                "Update [StandardCollation] set ReferenceNumber = @referenceNumber, Title = @Title, StandardData = @StandardData, DateUpdated=getutcdate(), DateRemoved=null, IsLive = 1 " +
                "where StandardId = @standardId",
                new {standard.StandardId, standard.ReferenceNumber, standard.Title, standardData}
            );
        }

        private static void InsertNewStandard(SqlConnection connection, StandardCollation standard, string standardData)
        {
            connection.Execute(
                "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                $@"VALUES (@standardId, @referenceNumber, @Title, @standardData)",
                new {standard.StandardId, standard.ReferenceNumber, standard.Title, standardData}
            );
        }

        private static int UpdateContactsThatAreDeleted(SqlConnection connection, List<StandardCollation> standards,
            List<StandardCollation> currentStandards)
        {
            var countRemoved = 0;
            var deletedStandards = new List<StandardCollation>();

            foreach (var standard in currentStandards)
            {
                if (standards.All(s => s.StandardId != standard.StandardId))
                    deletedStandards.Add(standard);
            }

            foreach (var standard in deletedStandards)
            {
                countRemoved++;
                connection.Execute(
                    "Update [StandardCollation] set IsLive=0, DateRemoved=getutcdate() " +
                    "where StandardId = @standardId",
                    new {standard.StandardId}
                );
            }
            return countRemoved;
        }
    }
}
