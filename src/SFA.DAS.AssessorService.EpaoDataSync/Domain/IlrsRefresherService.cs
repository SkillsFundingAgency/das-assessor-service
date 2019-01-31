using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.EpaoDataSync.Data;
using SFA.DAS.AssessorService.EpaoDataSync.Data.Types;
using SFA.DAS.AssessorService.EpaoDataSync.Logger;

namespace SFA.DAS.AssessorService.EpaoDataSync.Domain
{
    public class IlrsRefresherService : IIlrsRefresherService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IDbConnection _connection;
        private readonly IProviderEventServiceApi _eventServiceApi;

        public IlrsRefresherService(IDbConnection connection, IProviderEventServiceApi eventServiceApi,
            IAggregateLogger aggregateLogger)
        {
            _connection = connection;
            _eventServiceApi = eventServiceApi;
            _aggregateLogger = aggregateLogger;

        }

       
        public async Task UpdateIlRsTable()
        {
            _aggregateLogger.LogInfo(@"Starting to update Update IlRs Table From Submissions ....");
            var changedRecordsUlnCache = new List<long>();
            var totalNumbersEffected = 0L;
            var ilrResult =
                await _connection.QuerySingleAsync<Ilr>(
                    $"Select distinct Max(EventId) as EventId from Ilrs where EventId is not null");
            if (ilrResult != null)
            {
                var apiResults = await _eventServiceApi.GetSubmissionsEventsByEventId(ilrResult.EventId, 1);
                var numberOfPages = apiResults.TotalNumberOfPages;
                for (var pageNumber = 1; pageNumber <= numberOfPages; pageNumber++)
                {
                    if (pageNumber > 1)
                        apiResults =
                            await _eventServiceApi.GetSubmissionsEventsByEventId(ilrResult.EventId, pageNumber);

                    if (apiResults?.Items == null || !apiResults.Items.Any())
                        continue;
                    totalNumbersEffected = await ProcessFromSubmissions(totalNumbersEffected, apiResults, changedRecordsUlnCache);
                    _aggregateLogger.LogInfo(
                        $"Rows effected for page number {pageNumber} : {totalNumbersEffected}");
                }
            }

            _aggregateLogger.LogInfo(
                $"Finished inserting into Ilrs table, rows effected {totalNumbersEffected}");

            await ProcessFromLearners(changedRecordsUlnCache);
        }

      


        public async Task UpdateIlRsTable(string sinceTime)
        {
            _aggregateLogger.LogInfo(
                $"Starting to update IlRs Table from Submissions from {sinceTime} ....");
            var  changedRecordsUlnCache = new List<long>();

            var totalNumbersEffected = 0L;

            var apiResults = await _eventServiceApi.GetSubmissionsEventsByTime(sinceTime, 1);

            var numberOfPages = apiResults.TotalNumberOfPages;

            for (var pageNumber = 1; pageNumber <= numberOfPages; pageNumber++)
            {
                if (pageNumber > 1)
                        apiResults = await _eventServiceApi.GetSubmissionsEventsByTime(sinceTime, pageNumber);

                if (apiResults?.Items == null || !apiResults.Items.Any())
                    continue;

                totalNumbersEffected = await ProcessFromSubmissions(totalNumbersEffected, apiResults, changedRecordsUlnCache);
                _aggregateLogger.LogInfo(
                    $"Processed page {pageNumber} : Rows effected so far {totalNumbersEffected}");
            }

            _aggregateLogger.LogInfo(
                $"Finished updating/inserting into Ilrs table from submissions, rows effected {totalNumbersEffected}");
         
             await ProcessFromLearners(changedRecordsUlnCache);
            
        }

        private async Task ProcessFromLearners(IEnumerable<long> changedRecordsUlnCache)
        {
            _aggregateLogger.LogInfo(
                $"Starting to update IlRs Table from Learners ....");
            var totalNumbersEffected = 0L;
            foreach (var uln in changedRecordsUlnCache)
            {
                var ilrsResults = await _connection.QueryAsync<Ilr>($"Select Distinct MAX(EventId) as EventId from ILRS where Uln = @Uln", new { Uln = uln });
                var listOfIlrs = ilrsResults?.ToList();
                if (listOfIlrs == null || !listOfIlrs.Any())
                    continue;
                foreach (var listOfIlr in listOfIlrs)
                {
                    totalNumbersEffected += await UpdateFromLearners(uln,listOfIlr.EventId);
                }
            }

            _aggregateLogger.LogInfo(
                $"Finished updating/inserting into Ilrs table from learners, rows effected {totalNumbersEffected}");
        }

        private async Task<long> ProcessFromSubmissions(long totalNumbersEffected, SubmissionEvents apiResults, ICollection<long> changedRecordsUlnCache)
        {
            foreach (var submissionEvent in apiResults.Items)
            {
                if (submissionEvent.StandardCode == null)
                    continue;
                try
                {
                    totalNumbersEffected += await InsertFromSubmission(submissionEvent);
                }
                catch (Exception)
                {
                    totalNumbersEffected += await UpdateFromSubmission(submissionEvent);
                }
                changedRecordsUlnCache.Add(submissionEvent.Uln);
            }
            return totalNumbersEffected;
        }

        private async Task<long> UpdateFromSubmission(SubmissionEvent submissionEvent)
        {
            var totalNumbersEffected = await _connection.ExecuteAsync($"update Ilrs set " +
                                                                  "ApprenticeshipId = @apprenticeshipId, " +
                                                                  "UKPRN = @ukprn, " +
                                                                  "PlannedEndDate = @plannedEndDate, " +
                                                                  "UpdatedAt = @submittedDateTime, " +
                                                                  "EmployerAccountId = @employerReferenceNumber, " +
                                                                  "EventId = @Id " +
                                                                  "where Uln = @uln and StdCode=@standardCode",
                new
                {
                    apprenticeshipId = submissionEvent.ApprenticeshipId,
                    ukprn = submissionEvent.Ukprn,
                    plannedEndDate = submissionEvent.PlannedEndDate,
                    id = submissionEvent.Id,
                    uln = submissionEvent.Uln,
                    submittedDateTime = submissionEvent.SubmittedDateTime,
                    employerReferenceNumber = submissionEvent.EmployerReferenceNumber,
                    standardCode = submissionEvent.StandardCode
                });
            
            return totalNumbersEffected;
        }

        private async Task<long> InsertFromSubmission(SubmissionEvent submissionEvent)
        {
            var totalNumbersEffected = await _connection.ExecuteAsync(
                $"insert into Ilrs (Source,Uln, StdCode, ApprenticeshipId,UkPrn,PlannedEndDate,CreatedAt,EmployerAccountId,EventId) values (" +
                "@academicYear, " +
                "@uln, " +
                "@standardCode, " +
                "@apprenticeshipId, " +
                "@ukprn, " +
                "@plannedEndDate, " +
                "@submittedDateTime, " +
                "@employerReferenceNumber, " +
                "@id )",
                new
                {
                    academicYear = submissionEvent.AcademicYear,
                    apprenticeshipId = submissionEvent.ApprenticeshipId,
                    ukprn = submissionEvent.Ukprn,
                    plannedEndDate = submissionEvent.PlannedEndDate,
                    id = submissionEvent.Id,
                    uln = submissionEvent.Uln,
                    submittedDateTime = submissionEvent.SubmittedDateTime,
                    standardCode = submissionEvent.StandardCode,
                    employerReferenceNumber = submissionEvent.EmployerReferenceNumber,
                });

            return totalNumbersEffected;
        }

        private async Task<long> UpdateFromLearners(long uln, long eventId)
        {
            var totalNumbersEffected = 0;

            var apiResults = await _eventServiceApi.GetLatestLearnerEventForStandards(uln, eventId - 1);
            if (apiResults != null && apiResults.Any())
            {
                foreach (var apiResult in apiResults)
                {
                    var sql =
                        $"update Ilrs set Source={(apiResult.AcademicYear == null ? "Source":"'"+apiResult.AcademicYear+"'" )}, " +
                        $"ApprenticeshipId = {(apiResult.ApprenticeshipId == null ? "ApprenticeshipId" : "'"+apiResult.ApprenticeshipId+ "'")}, " +
                        $"GivenNames = @givenNames, " +
                        $"FamilyName = @familyNames, " +
                        $"EpaOrgId = {(apiResult.EPAOrgId == null ? "EpaOrgId" : "'" + apiResult.EPAOrgId + "'")}," +
                        $"CompletionStatus = {(apiResult.CompStatus == null? "CompletionStatus" : "'" + apiResult.CompStatus+ "'")  }, " +
                        $"LearnStartDate ={(apiResult.ActualStartDate == null ? "LearnStartDate" : "'" + apiResult.ActualStartDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "'")}, " +
                        $"PlannedEndDate =  {(apiResult.PlannedEndDate == null ? "PlannedEndDate" : "'" + apiResult.PlannedEndDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "'")} " +
                        $"where Uln = {apiResult.Uln} and StdCode={apiResult.StandardCode} and EventId = {apiResult.Id}";

                 
                    try
                    {
                        totalNumbersEffected += await _connection.ExecuteAsync(sql, new
                        {
                            givenNames= apiResult.GivenNames,
                            familyNames= apiResult.FamilyName
                        });
                    }
                    catch (Exception e)
                    {
                         _aggregateLogger.LogInfo(
                                                 $"{sql} : Exception {e.Message}");
                    }
                }
            }
            
            return totalNumbersEffected;
        }

    }
}
