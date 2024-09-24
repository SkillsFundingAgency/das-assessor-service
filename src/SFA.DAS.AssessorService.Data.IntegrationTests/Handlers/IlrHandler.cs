using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class IlrHandler
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
                LearnStartDate = learnStartDate ?? DateTime.UtcNow,
                FundingModel = fundingModel ?? 36,
                Source = source ?? HandlerBase.GetAcademicYear(DateTime.UtcNow),
                CreatedAt = createdAt ?? DateTime.UtcNow,
                CompletionStatus = completionStatus,
                PlannedEndDate = plannedEndDate ?? DateTime.UtcNow.AddMonths(12),
            };
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

