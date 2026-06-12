using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class IlrHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(IlrModel ilr)
        {
            var sql =
                "INSERT INTO [Ilrs] " +
                    "([Id]" +
                    ", [Uln] " +
                    ", [GivenNames] " +
                    ", [FamilyName] " +
                    ", [Ukprn] " +
                    ", [StdCode]" +
                    ", [LearnStartDate]" +
                    ", [FundingModel]" +
                    ", [Source]" +
                    ", [CreatedAt]" +
                    ", [CompletionStatus]" +
                    ", [PlannedEndDate])" +
                "VALUES " +
                    "(@id" +
                    ", @uln" +
                    ", @givenNames" +
                    ", @familyName" +
                    ", @ukprn" +
                    ", @stdCode" +
                    ", @learnStartDate" +
                    ", @fundingModel" +
                    ", @source" +
                    ", @createdAt" +
                    ", @completionStatus" +
                    ", @plannedEndDate); ";

            DatabaseService.Execute(sql, ilr);
        }

        public static void InsertRecords(List<IlrModel> ilrs)
        {
            foreach (var ilr in ilrs)
            {
                InsertRecord(ilr);
            }
        }

        public static IlrModel Create(
            Guid? id, long uln, string givenNames, string familyName, int ukprn, int stdCode, DateTime? learnStartDate, int? fundingModel, 
            string source, DateTime? createdAt, int completionStatus, DateTime? plannedEndDate)
        {
            return new IlrModel
            {
                Id = id,
                Uln = uln,
                GivenNames = givenNames ?? "Alice",
                FamilyName = familyName ?? "Bobdotter",
                Ukprn = ukprn,
                StdCode = stdCode,
                LearnStartDate = learnStartDate,
                FundingModel = fundingModel ?? 36,
                Source = source ?? HandlerBase.GetAcademicYear(DateTime.UtcNow),
                CreatedAt = createdAt ?? DateTime.UtcNow,
                CompletionStatus = completionStatus,
                PlannedEndDate = plannedEndDate
            };
        }

        public static async Task<IlrModel> QueryFirstOrDefaultAsync(IlrModel ilr)
        {
            var sqlToQuery =
            "SELECT " +
                "[Id]" +
                ", [Uln]" +
                ", [GivenNames]" +
                ", [FamilyName]" +
                ", [Ukprn]" +
                ", [StdCode]" +
                ", [LearnStartDate]" +
                ", [FundingModel]" +
                ", [Source]" +
                ", [CreatedAt]" +
                ", [CompletionStatus]" +
                ", [PlannedEndDate] " +
             "FROM [Ilrs] " +
            $"WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                $"AND {NullQueryParam(ilr, p => p.Uln)} " +
                $"AND {NullQueryParam(ilr, p => p.GivenNames)} " +
                $"AND {NullQueryParam(ilr, p => p.FamilyName)} " +
                $"AND {NullQueryParam(ilr, p => p.Ukprn)} " +
                $"AND {NullQueryParam(ilr, p => p.StdCode)} " +
                $"AND {NullQueryParam(ilr, p => p.LearnStartDate)} " +
                $"AND {NullQueryParam(ilr, p => p.FundingModel)} " +
                $"AND {NullQueryParam(ilr, p => p.Source)} " +
                $"AND {NotNullQueryParam(ilr, p => p.CreatedAt)} " +
                $"AND {NullQueryParam(ilr, p => p.CompletionStatus)} " +
                $"AND {NullQueryParam(ilr, p => p.PlannedEndDate)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<IlrModel>(sqlToQuery, ilr);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery = "SELECT COUNT(1) FROM [Ilrs]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(int uln, int stdCode)
        {
            var sql = $@"DELETE FROM Ilrs WHERE Uln = {uln} AND StdCode = {stdCode}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM Ilrs";
            DatabaseService.Execute(sql);
        }
    }
}

