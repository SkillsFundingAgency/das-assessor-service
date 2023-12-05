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
                    ", [Ukprn] " +
                    ", [StdCode]" +
                    ", [Source]" +
                    ", [CreatedAt]" +
                    ", [CompletionStatus])" +
                "VALUES " +
                    "(@id" +
                    ", @uln" +
                    ", @ukprn" +
                    ", @stdCode" +
                    ", @source" +
                    ", @createdAt" +
                    ", @completionStatus); ";

            DatabaseService.Execute(sql, ilr);
        }

        public static void InsertRecords(List<IlrModel> ilrs)
        {
            foreach (var ilr in ilrs)
            {
                InsertRecord(ilr);
            }
        }

        public static IlrModel Create(Guid? id, long uln, int ukprn, int stdCode, int completionStatus, string source = null, DateTime? createdAt = null)
        {
            return new IlrModel
            {
                Id = id,
                Uln = uln,
                Ukprn = ukprn,
                StdCode = stdCode,
                Source = source ?? GetAcademicYear(DateTime.UtcNow),
                CreatedAt = createdAt ?? DateTime.UtcNow,
                CompletionStatus = completionStatus
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

        public static string GetAcademicYear(DateTime date)
        {
            if (date.Month < 8)
            {
                return (date.Year - 1).ToString().Substring(2) + date.Year.ToString().Substring(2);
            }
            else
            {
                return date.Year.ToString().Substring(2) + (date.Year + 1).ToString().Substring(2);
            }
        }
    }
}

