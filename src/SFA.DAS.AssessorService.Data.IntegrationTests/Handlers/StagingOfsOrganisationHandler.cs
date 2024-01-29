using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class StagingOfsOrganisationHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(StagingOfsOrganisationModel stagingOfsOrganisation)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[StagingOfsOrganisation]" +
                    "([Ukprn]" +
                    ", [RegistrationStatus]" +
                    ", [HighestLevelOfDegreeAwardingPowers])" +
                "VALUES " +
                    "(@ukprn" +
                    ", @registrationStatus" +
                    ", @highestLevelOfDegreeAwardingPowers)";

            DatabaseService.Execute(sqlToInsert, stagingOfsOrganisation);
        }

        public static void InsertRecords(List<StagingOfsOrganisationModel> statingOfsOrganisations)
        {
            foreach (var statingOfsOrganisation in statingOfsOrganisations)
            {
                InsertRecord(statingOfsOrganisation);
            }
        }

        public static StagingOfsOrganisationModel Create(int ukprn, string registrationStatus, string highestLevelOfDegreeAwardingPowers)
        {
            return new StagingOfsOrganisationModel
            {
                Ukprn = ukprn,
                RegistrationStatus = registrationStatus,
                HighestLevelOfDegreeAwardingPowers = highestLevelOfDegreeAwardingPowers
            };
        }

        public static async Task<StagingOfsOrganisationModel> QueryFirstOrDefaultAsync(StagingOfsOrganisationModel stagingOfsOrganisation)
        {
            var sqlToQuery =
                "SELECT" +
                    "[Ukprn]" +
                    ", [RegistrationStatus]" +
                    ", [HighestLevelOfDegreeAwardingPowers]" +
                "FROM [StagingOfsOrganisation] " +
                "WHERE Ukprn = @ukprn " +
                    "AND RegistrationStatus = @registrationStatus " +
                    "AND HighestLevelOfDegreeAwardingPowers = @highestLevelOfDegreeAwardingPowers";

            return await DatabaseService.QueryFirstOrDefaultAsync<StagingOfsOrganisationModel>(sqlToQuery, stagingOfsOrganisation);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [StagingOfsOrganisation]";

            DatabaseService.Execute(sql);
        }
    }
}
