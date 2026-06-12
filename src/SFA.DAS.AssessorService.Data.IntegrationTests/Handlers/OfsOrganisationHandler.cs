using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OfsOrganisationHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OfsOrganisationModel ofsOrganisation)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[OfsOrganisation]" +
                    "([Id]" +
                    ", [Ukprn]" +
                    ", [CreatedAt])" +
                "VALUES " +
                    "(@id" +
                    ", @ukprn" +
                    ", @createdAt)";

            DatabaseService.Execute(sqlToInsert, ofsOrganisation);
        }

        public static void InsertRecords(List<OfsOrganisationModel> ofsOrganisations)
        {
            foreach (var ofsOrganisation in ofsOrganisations)
            {
                InsertRecord(ofsOrganisation);
            }
        }

        public static OfsOrganisationModel Create(Guid? id, int ukprn, DateTime createdAt)
        {
            return new OfsOrganisationModel
            {
                Id = id,
                Ukprn = ukprn,
                CreatedAt = createdAt
            };
        }

        public static async Task<OfsOrganisationModel> QueryFirstOrDefaultAsync(OfsOrganisationModel ofsOrganisation)
        {
            var sqlToQuery =
                "SELECT" +
                    "[Id]" +
                    ", [Ukprn]" +
                    ", [CreatedAt]" +
                "FROM [OfsOrganisation] " +
                "WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    "AND Ukprn = @ukprn " +
                    "AND CreatedAt = @createdAt";

            return await DatabaseService.QueryFirstOrDefaultAsync<OfsOrganisationModel>(sqlToQuery, ofsOrganisation);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [OfsOrganisation]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [OfsOrganisation]";

            DatabaseService.Execute(sql);
        }
    }
}
