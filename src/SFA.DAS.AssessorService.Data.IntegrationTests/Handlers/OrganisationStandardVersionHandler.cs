using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class OrganisationStandardVersionHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStandardVersionModel organisationStandard)
        {
            var sql =
                "INSERT INTO [dbo].[OrganisationStandardVersion] ([StandardUId], [Version],[OrganisationStandardId],[EffectiveFrom],[EffectiveTo],[DateVersionApproved]," +
                "[Comments],[Status]) VALUES (@standardUId, @version, @organisationStandardId, @effectiveFrom, @effectiveTo, @dateVersionApproved, @comments, @status); ";

            DatabaseService.Execute(sql, organisationStandard);
        }

        public static void InsertRecords(List<OrganisationStandardVersionModel> organisationStandardVersions)
        {
            foreach (var org in organisationStandardVersions)
            {
                InsertRecord(org);
            }
        }

        public static OrganisationStandardVersion GetOrganisationStandardVersionByOrgIdStandardUId(int orgStandardId, string standardUId)
        {
            var organisationStandard = DatabaseService.Get<OrganisationStandardVersion>($@"select StandardUID, Version, OrganisationStandardId, EffectiveFrom, EffectiveTo, DateVersionApproved, Comments, Status  from OrganisationStandardVersion where OrganisationStandardId = '{orgStandardId}' and StandardUId = {standardUId}");
            return organisationStandard;
        }


        public static void DeleteRecord(int orgStandardId, string standardUId)
        {
            var standardUIdToDelete = SqlStringService.ConvertStringToSqlValueString(standardUId);
            var sql = $@"DELETE from OrganisationStandardVersion where OrganisationStandardId = {orgStandardId} and StandardUId = {standardUIdToDelete}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecordByOrganisationStandardId(int orgStandardId)
        {
            var sql = $@"DELETE from OrganisationStandardVersion where OrganisationStandardId = {orgStandardId}";
            DatabaseService.Execute(sql);
        }
    }
}