﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class StandardRepository : IStandardRepository
    {
        private readonly AssessorDbContext _assessorDbContext;
        private readonly IDbConnection _connection;

        public StandardRepository(AssessorDbContext assessorDbContext, IDbConnection connection)
        {
            _assessorDbContext = assessorDbContext;
            _connection = connection;
            SqlMapper.AddTypeHandler(typeof(StandardData), new StandardDataHandler());
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

            var standards = await _connection.QueryAsync<StandardCollation>(standardsQuery, param: new { standardIdFilter, referenceNumberFilter });

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
            var options = await _connection.QueryAsync<Option>(optionsQuery, param: new { standardCodes });

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

        public async Task<DateTime?> GetDateOfLastStandardCollation()
        {
            const string sql = "select top 1 coalesce(max(DateUpdated), max(DateAdded)) maxDate  from StandardCollation";
            var dateOfLastCollation = await _connection.QuerySingleAsync<DateTime?>(sql);
            return dateOfLastCollation;
        }

        public async Task<string> UpsertStandards(List<StandardCollation> standards)
        {
            var countInserted = 0;
            var countUpdated = 0;
            var countRemoved = 0;

            var currentStandards = await GetStandardCollations();
            countRemoved = UpdateContactsThatAreDeleted(_connection, standards, currentStandards);

            foreach (var standard in standards)
            {
                var isNew = true;
                var standardData = JsonConvert.SerializeObject(standard.StandardData);
                if (currentStandards.Any(x => x.StandardId == standard.StandardId))
                    isNew = false;

                if (isNew)
                {
                    countInserted++;
                    InsertNewStandard(_connection, standard, standardData);
                }
                else
                {
                    countUpdated++;
                    UpdateCurrentStandard(_connection, standard, standardData);
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

        public async Task<List<EpaoPipelineStandardExtract>> GetEpaoPipelineStandardsExtract(string endPointAssessorOrganisationId)
        {
            var result = await _connection.QueryAsync<EpaoPipelineStandardExtract>("GetEPAO_Pipelines_Extract", new
            {
                EPAOId = endPointAssessorOrganisationId
            },
            commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        private static void UpdateCurrentStandard(IDbConnection connection, StandardCollation standard, string standardData)
        {
            // when new ReferenceNumber is null (IFA has not supplied one) retain the current RefernceNumber
            connection.Execute(
                "Update [StandardCollation] set ReferenceNumber = case when @referenceNumber is not null then @referenceNumber else ReferenceNumber end, Title = @Title, StandardData = @StandardData, DateUpdated=getutcdate(), DateRemoved=null, IsLive = 1 " +
                "where StandardId = @standardId",
                new { standard.StandardId, standard.ReferenceNumber, standard.Title, standardData }
            );
        }

        private static void InsertNewStandard(IDbConnection connection, StandardCollation standard, string standardData)
        {
            connection.Execute(
                "INSERT INTO [StandardCollation] ([StandardId],[ReferenceNumber] ,[Title],[StandardData]) " +
                $@"VALUES (@standardId, @referenceNumber, @Title, @standardData)",
                new { standard.StandardId, standard.ReferenceNumber, standard.Title, standardData }
            );
        }

        private static int UpdateContactsThatAreDeleted(IDbConnection connection, List<StandardCollation> standards,
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
                    new { standard.StandardId }
                );
            }
            return countRemoved;
        }


    }
}
