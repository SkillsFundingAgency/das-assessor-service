using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.EpaoDataSync.Data;
using SFA.DAS.AssessorService.EpaoDataSync.Logger;

namespace SFA.DAS.AssessorService.EpaoDataSync.Domain
{
    public class IlrsRefresherService:IIlrsRefresherService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IDbConnection _connection;
        private readonly IProviderEventServiceApi _eventServiceApi;

        public IlrsRefresherService(IDbConnection connection,IProviderEventServiceApi eventServiceApi, IAggregateLogger aggregateLogger)
        {
            _connection = connection;
            _eventServiceApi = eventServiceApi;
            _aggregateLogger = aggregateLogger;
        }

        public async Task<long> UpdateIlRsTable(string tableName)
        {
            _aggregateLogger.LogInfo(@"Starting to update Ilrs table ....");
            long totalNumbersEffected = 0;
            var ilrResults = await _connection.QueryAsync<Ilr>($"Select distinct Uln, StdCode from {tableName} where FundingModel = 36 ");
            foreach (var ilrResult in ilrResults)
            {
                var apiResults = await _eventServiceApi.GetLatestLearnerEventForStandards(ilrResult.Uln);
                if (apiResults == null) continue;
                foreach (var apiResult in apiResults.Where(r =>
                    r.StandardCode == ilrResult.StdCode && r.Uln == ilrResult.Uln))
                {
                    totalNumbersEffected = await _connection.ExecuteAsync($"update {tableName} set " +
                                                                          "Source = @academicYear, " +
                                                                          "GivenNames = @givenNames," +
                                                                          "FamilyName = @familyName, " +
                                                                          "ApprenticeshipId = @apprenticeshipId, " +
                                                                          "EPAOrgId = @epaOrgId, " +
                                                                          "CompletionStatus = @compStatus, " +
                                                                          "LearnStartDate = @actualStartDate, " +
                                                                          "UKPRN = @ukprn, " +
                                                                          "PlannedEndDate = @plannedEndDate, " +
                                                                          "EventId = @id  " +
                                                                          "where Uln = @uln and StdCode=@standardCode",
                        new
                        {
                            academicYear = apiResult.AcademicYear,
                            givenNames = apiResult.GivenNames,
                            familyName = apiResult.FamilyName,
                            apprenticeshipId = apiResult.ApprenticeshipId,
                            epaOrgId = apiResult.EPAOrgId,
                            compStatus = apiResult.CompStatus,
                            actualStartDate = apiResult.ActualStartDate,
                            ukprn = apiResult.Ukprn,
                            plannedEndDate = apiResult.PlannedEndDate,
                            id = apiResult.Id,
                            uln = apiResult.Uln,
                            standardCode = apiResult.StandardCode
                        });
                }
            }
            _aggregateLogger.LogInfo($"Finished updating Ilrs table, rows effected {totalNumbersEffected}");
            return totalNumbersEffected;
        }
    }
}
