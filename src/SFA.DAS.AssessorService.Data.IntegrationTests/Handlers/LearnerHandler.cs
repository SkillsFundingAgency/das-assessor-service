using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class LearnerHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(LearnerModel learner)
        {
            var sql =
            "SET IDENTITY_INSERT [Learner] ON; " +
            "INSERT INTO [dbo].[Learner] " +
                "([Id]" +
                ", [Uln]" +
                ", [GivenNames]" +
                ", [FamilyName]" +
                ", [UkPrn]" +
                ", [StdCode]" +
                ", [LearnStartDate]" +
                ", [EpaOrgId]" +
                ", [FundingModel]" +
                ", [Source]" +
                ", [LearnRefNumber]" +
                ", [CompletionStatus]" +
                ", [PlannedEndDate]" +
                ", [DelLocPostCode]" +
                ", [LearnActEndDate]" +
                ", [WithdrawReason]" +
                ", [Outcome]" +
                ", [AchDate]" +
                ", [OutGrade]" +
                ", [Version]" +
                ", [VersionConfirmed]" +
                ", [CourseOption]" +
                ", [StandardUId]" +
                ", [StandardReference]" +
                ", [StandardName]" +
                ", [LastUpdated]" +
                ", [EstimatedEndDate]" +
                ", [ApprovalsStopDate]" +
                ", [ApprovalsPauseDate]" +
                ", [ApprovalsCompletionDate]" +
                ", [ApprovalsPaymentStatus]" +
                ", [LatestIlrs]" +
                ", [LatestApprovals]" +
                ", [EmployerAccountId]" +
                ", [EmployerName]" +
                ", [IsTransfer]" +
                ", [DateTransferIdentified]) " +
            "VALUES " +
                "(@id" +
                ", @uln" +
                ", @givenNames" +
                ", @familyName" +
                ", @ukPrn" +
                ", @stdCode" +
                ", @learnStartDate" +
                ", @epaOrgId" +
                ", @fundingModel" +
                ", @source" +
                ", @learnRefNumber" +
                ", @completionStatus" +
                ", @plannedEndDate" +
                ", @delLocPostCode" +
                ", @learnActEndDate" +
                ", @withdrawReason" +
                ", @outcome" +
                ", @achDate" +
                ", @outGrade" +
                ", @version" +
                ", @versionConfirmed" +
                ", @courseOption" +
                ", @standardUId" +
                ", @standardReference" +
                ", @standardName" +
                ", @lastUpdated" +
                ", @estimatedEndDate" +
                ", @approvalsStopDate" +
                ", @approvalsPauseDate" +
                ", @approvalsCompletionDate" +
                ", @approvalsPaymentStatus" +
                ", @latestIlrs" +
                ", @latestApprovals" +
                ", @employerAccountId" +
                ", @employerName" +
                ", @isTransfer" +
                ", @dateTransferIdentified); " +
            "SET IDENTITY_INSERT [Learner] OFF;";

            DatabaseService.Execute(sql, learner);
        }

        public static LearnerModel Create(
            Guid? id, long? uln, string givenNames, string familyName, int? ukPrn, int? stdCode,
            DateTime? learnStartDate, string epaOrgId, int? fundingModel, long? apprenticeshipId, string source,
            string learnRefNumber, int? completionStatus, DateTime? plannedEndDate, string delLocPostCode,
            DateTime? learnActEndDate, int? withdrawReason, int? outcome, DateTime? achDate, string outGrade,
            string version, int versionConfirmed, string courseOption, string standardUId, string standardReference,
            string standardName, DateTime? lastUpdated, DateTime? estimatedEndDate, DateTime? approvalsStopDate,
            DateTime? approvalsPauseDate, DateTime? approvalsCompletionDate, short? approvalsPaymentStatus,
            DateTime? latestIlrs, DateTime? latestApprovals, long? employerAccountId, string employerName,
            int isTransfer, DateTime? dateTransferIdentified)
        {
            return new LearnerModel
            {
                Id = id,
                Uln = uln,
                GivenNames = givenNames,
                FamilyName = familyName,
                UkPrn = ukPrn,
                StdCode = stdCode,
                LearnStartDate = learnStartDate,
                EpaOrgId = epaOrgId,
                FundingModel = fundingModel,
                ApprenticeshipId = apprenticeshipId,
                Source = source,
                LearnRefNumber = learnRefNumber,
                CompletionStatus = completionStatus,
                PlannedEndDate = plannedEndDate,
                DelLocPostCode = delLocPostCode,
                LearnActEndDate = learnActEndDate,
                WithdrawReason = withdrawReason,
                Outcome = outcome,
                AchDate = achDate,
                OutGrade = outGrade,
                Version = version,
                VersionConfirmed = versionConfirmed,
                CourseOption = courseOption,
                StandardUId = standardUId,
                StandardReference = standardReference,
                StandardName = standardName,
                LastUpdated = lastUpdated,
                EstimatedEndDate = estimatedEndDate,
                ApprovalsStopDate = approvalsStopDate,
                ApprovalsPauseDate = approvalsPauseDate,
                ApprovalsCompletionDate = approvalsCompletionDate,
                ApprovalsPaymentStatus = approvalsPaymentStatus,
                LatestIlrs = latestIlrs,
                LatestApprovals = latestApprovals,
                EmployerAccountId = employerAccountId,
                EmployerName = employerName,
                IsTransfer = isTransfer,
                DateTransferIdentified = dateTransferIdentified
            };
        }


        public static async Task<LearnerModel> QueryFirstOrDefaultAsync(LearnerModel learner)
        {
            var sqlToQuery =
            "SELECT " +
                "[Id]" +
                ", [Uln]" +
                ", [GivenNames]" +
                ", [FamilyName]" +
                ", [UkPrn]" +
                ", [StdCode]" +
                ", [LearnStartDate]" +
                ", [EpaOrgId]" +
                ", [FundingModel]" +
                ", [Source]" +
                ", [LearnRefNumber]" +
                ", [CompletionStatus]" +
                ", [PlannedEndDate]" +
                ", [DelLocPostCode]" +
                ", [LearnActEndDate]" +
                ", [WithdrawReason]" +
                ", [Outcome]" +
                ", [AchDate]" +
                ", [OutGrade]" +
                ", [Version]" +
                ", [VersionConfirmed]" +
                ", [CourseOption]" +
                ", [StandardUId]" +
                ", [StandardReference]" +
                ", [StandardName]" +
                ", [LastUpdated]" +
                ", [EstimatedEndDate]" +
                ", [ApprovalsStopDate]" +
                ", [ApprovalsPauseDate]" +
                ", [ApprovalsCompletionDate]" +
                ", [ApprovalsPaymentStatus]" +
                ", [LatestIlrs]" +
                ", [LatestApprovals]" +
                ", [EmployerAccountId]" +
                ", [EmployerName]" +
                ", [IsTransfer]" +
                ", [DateTransferIdentified] " +
             "FROM [Learner] " +
            $"WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                $"AND {NullQueryParam(learner, p => p.Uln)} " +
                $"AND {NullQueryParam(learner, p => p.GivenNames)} " +
                $"AND {NullQueryParam(learner, p => p.FamilyName)} " +
                $"AND {NullQueryParam(learner, p => p.UkPrn)} " +
                $"AND {NullQueryParam(learner, p => p.StdCode)} " +
                $"AND {NullQueryParam(learner, p => p.LearnStartDate)} " +
                $"AND {NullQueryParam(learner, p => p.EpaOrgId)} " +
                $"AND {NullQueryParam(learner, p => p.FundingModel)} " +
                $"AND {NullQueryParam(learner, p => p.Source)} " +
                $"AND {NullQueryParam(learner, p => p.LearnRefNumber)} " +
                $"AND {NullQueryParam(learner, p => p.CompletionStatus)} " +
                $"AND {NullQueryParam(learner, p => p.PlannedEndDate)} " +
                $"AND {NullQueryParam(learner, p => p.DelLocPostCode)} " +
                $"AND {NullQueryParam(learner, p => p.LearnActEndDate)} " +
                $"AND {NullQueryParam(learner, p => p.WithdrawReason)} " +
                $"AND {NullQueryParam(learner, p => p.Outcome)} " +
                $"AND {NullQueryParam(learner, p => p.AchDate)} " +
                $"AND {NullQueryParam(learner, p => p.OutGrade)} " +
                $"AND {NullQueryParam(learner, p => p.Version)} " +
                $"AND {NotNullQueryParam(learner, p => p.VersionConfirmed)} " +
                $"AND {NullQueryParam(learner, p => p.CourseOption)} " +
                $"AND {NullQueryParam(learner, p => p.StandardUId)} " +
                $"AND {NullQueryParam(learner, p => p.StandardReference)} " +
                $"AND {NullQueryParam(learner, p => p.StandardName)} " +
                $"AND {NullQueryParam(learner, p => p.LastUpdated)} " +
                $"AND {NullQueryParam(learner, p => p.EstimatedEndDate)} " +
                $"AND {NullQueryParam(learner, p => p.ApprovalsStopDate)} " +
                $"AND {NullQueryParam(learner, p => p.ApprovalsPauseDate)} " +
                $"AND {NullQueryParam(learner, p => p.ApprovalsCompletionDate)} " +
                $"AND {NullQueryParam(learner, p => p.ApprovalsPaymentStatus)} " +
                $"AND {NullQueryParam(learner, p => p.LatestIlrs)} " +
                $"AND {NullQueryParam(learner, p => p.LatestApprovals)} " +
                $"AND {NullQueryParam(learner, p => p.EmployerAccountId)} " +
                $"AND {NullQueryParam(learner, p => p.EmployerName)} " +
                $"AND {NotNullQueryParam(learner, p => p.IsTransfer)} " +
                $"AND {NullQueryParam(learner, p => p.DateTransferIdentified)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<LearnerModel, LearnerModel>(sqlToQuery, learner);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery = "SELECT COUNT(1) FROM [Learner]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(Guid id)
        {
            var sql = "DELETE FROM Learner WHERE Id = @id";
            DatabaseService.Execute(sql, new { id });
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM Learner";
            DatabaseService.Execute(sql);
        }
    }
}