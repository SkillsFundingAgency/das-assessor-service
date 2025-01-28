using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class ApprovalsExtractHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ApprovalsExtractModel approvalsExtract)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[ApprovalsExtract]" +
                    "([ApprenticeshipId]" +
                    ", [FirstName]" +
                    ", [LastName]" +
                    ", [Uln]" +
                    ", [TrainingCode]" +
                    ", [TrainingCourseVersion]" +
                    ", [TrainingCourseVersionConfirmed]" +
                    ", [TrainingCourseOption]" +
                    ", [StandardUId]" +
                    ", [StartDate]" +
                    ", [EndDate]" +
                    ", [CreatedOn]" +
                    ", [UpdatedOn]" +
                    ", [StopDate]" +
                    ", [PauseDate]" +
                    ", [CompletionDate]" +
                    ", [UKPRN]" +
                    ", [LearnRefNumber]" +
                    ", [PaymentStatus]" +
                    ", [EmployerAccountId]" +
                    ", [EmployerName]) " +
                "VALUES " +
                    "(@apprenticeshipId" +
                    ", @firstName" +
                    ", @lastName" +
                    ", @uln" +
                    ", @trainingCode" +
                    ", @trainingCourseVersion" +
                    ", @trainingCourseVersionConfirmed" +
                    ", @trainingCourseOption" +
                    ", @standardUId" +
                    ", @startDate" +
                    ", @endDate" +
                    ", @createdOn" +
                    ", @updatedOn" +
                    ", @stopDate" +
                    ", @pauseDate" +
                    ", @completionDate" +
                    ", @ukprn" +
                    ", @learnRefNumber" +
                    ", @paymentStatus" +
                    ", @employerAccountId" +
                    ", @employerName)";

            DatabaseService.Execute(sqlToInsert, approvalsExtract);
        }

        public static void InsertRecords(List<ApprovalsExtractModel> approvalsExtracts)
        {
            foreach (var approvalsExtract in approvalsExtracts)
            {
                InsertRecord(approvalsExtract);
            }
        }

        public static ApprovalsExtractModel Create(int? apprenticeshipId, string firstName, string lastName, string uln, int? trainingCode, string trainingCourseVersion, bool trainingCourseVersionConfirmed, 
            string trainingCourseOption, string standardUId, DateTime? startDate, DateTime? endDate, DateTime? createdOn, DateTime? updatedOn, DateTime? stopDate, DateTime? pauseDate, DateTime? completionDate,
            int? ukprn, string learnRefNumber, int? paymentStatus, long? employerAccountId, string employerName)
        {
            return new ApprovalsExtractModel
            {
                ApprenticeshipId = apprenticeshipId,
                FirstName = firstName,
                LastName = lastName,
                ULN = uln,
                TrainingCode = trainingCode,
                TrainingCourseVersion = trainingCourseVersion,
                TrainingCourseVersionConfirmed = trainingCourseVersionConfirmed,
                TrainingCourseOption = trainingCourseOption,
                StandardUId = standardUId,
                StartDate = startDate,
                EndDate = endDate,
                CreatedOn = createdOn,
                UpdatedOn = updatedOn,
                StopDate = stopDate,
                PauseDate = pauseDate,
                CompletionDate = completionDate,
                UKPRN = ukprn,
                LearnRefNumber = learnRefNumber,
                PaymentStatus = paymentStatus,
                EmployerAccountId = employerAccountId,
                EmployerName = employerName,
            };
        }

        public static async Task<ApprovalsExtractModel> QueryFirstOrDefaultAsync(ApprovalsExtractModel approvalsExtract)
        {
            var sqlToQuery =
                "SELECT" +
                    "[ApprenticeshipId]" + 
                    ", [FirstName]" +
                    ", [LastName]" +
                    ", [Uln]" +
                    ", [TrainingCode]" +
                    ", [TrainingCourseVersion]" +
                    ", [TrainingCourseVersionConfirmed]" + 
                    ", [TrainingCourseOption]" +
                    ", [StandardUId]" +
                    ", [StartDate]" +
                    ", [EndDate]" +
                    ", [CreatedOn]" +
                    ", [UpdatedOn]" +
                    ", [StopDate]" +
                    ", [PauseDate]" +
                    ", [CompletionDate]" +
                    ", [UKPRN]" +
                    ", [LearnRefNumber]" +
                    ", [PaymentStatus]" +
                    ", [EmployerAccountId]" +
                    ", [EmployerName] " +
                "FROM [OfqualOrganisation] " +
                "WHERE ApprenticeshipId = @apprenticeshipId " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.FirstName)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.LastName)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.ULN)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.TrainingCode)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.TrainingCourseVersion)} " +
                   $"AND {NotNullQueryParam(approvalsExtract, p => p.TrainingCourseVersionConfirmed)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.TrainingCourseOption)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.StandardUId)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.StartDate)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.EndDate)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.CreatedOn)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.UpdatedOn)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.StopDate)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.PauseDate)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.CompletionDate)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.UKPRN)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.LearnRefNumber)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.PaymentStatus)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.EmployerAccountId)} " +
                   $"AND {NullQueryParam(approvalsExtract, p => p.EmployerName)}";

            return await DatabaseService.QueryFirstOrDefaultAsync<ApprovalsExtractModel>(sqlToQuery, approvalsExtract);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [ApprovalsExtract]";

            DatabaseService.Execute(sql);
        }
    }
}
