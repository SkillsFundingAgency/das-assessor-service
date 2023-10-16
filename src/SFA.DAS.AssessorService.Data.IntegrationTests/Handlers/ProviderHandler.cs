using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class ProviderHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ProviderModel provider)
        {
            var sql =
                "INSERT INTO [Providers] " +
                    "([Ukprn] " +
                    ", [Name]" +
                    ", [UpdatedOn]) " +
                "VALUES " +
                    "(@ukprn" +
                    ", @name" +
                    ", @updatedOn); ";

            DatabaseService.Execute(sql, provider);
        }

        public static void InsertRecords(List<ProviderModel> providers)
        {
            foreach (var provider in providers)
            {
                InsertRecord(provider);
            }
        }

        public static void DeleteRecord(int ukprn)
        {
            var sql = $@"DELETE from Providers where ukprn = {ukprn}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<int> ukprns)
        {
            foreach (var ukprn in ukprns)
            {
                DeleteRecord(ukprn);
            }
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE from Providers";
            DatabaseService.Execute(sql);
        }
    }
}

