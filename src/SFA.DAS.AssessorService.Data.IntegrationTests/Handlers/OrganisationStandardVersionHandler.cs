using SFA.DAS.AssessorService.Application.Mapping.Structs;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OrganisationStandardVersionHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static OrganisationStandardVersionModel Create(string standardUId, string version, int? organisationStandardId, DateTime? effectiveFrom, DateTime? effectiveTo,
            DateTime? dateVersionApproved, string comments, string status)
        {
            return new OrganisationStandardVersionModel
            {
                StandardUId = standardUId,
                Version = version,
                OrganisationStandardId = organisationStandardId,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                DateVersionApproved = dateVersionApproved,
                Comments = comments,
                Status = status
            };
        }

        public static void InsertRecord(OrganisationStandardVersionModel organisationStandard)
        {
            var sql =
                "INSERT INTO [dbo].[OrganisationStandardVersion]" +
                    "([StandardUId]" +
                    ", [Version]" +
                    ", [OrganisationStandardId]" +
                    ", [EffectiveFrom]" +
                    ", [EffectiveTo]" +
                    ", [DateVersionApproved]" +
                    ", [Comments]" +
                    ", [Status]) " +
                    "VALUES " +
                        "(@standardUId" +
                        ", @version" +
                        ", @organisationStandardId" +
                        ", @effectiveFrom" +
                        ", @effectiveTo" +
                        ", @dateVersionApproved" +
                        ", @comments" +
                        ", @status);";

            DatabaseService.Execute(sql, organisationStandard);
        }

        public static void InsertRecords(List<OrganisationStandardVersionModel> organisationStandardVersions)
        {
            foreach (var orgisationStandardVersion in organisationStandardVersions)
            {
                InsertRecord(orgisationStandardVersion);
            }
        }

        public static async Task<OrganisationStandardVersionModel> QueryFirstOrDefaultAsync(OrganisationStandardVersionModel organisationStandardVersion)
        {
            var sqlToQuery =
                "SELECT " +
                    "[StandardUId]" +
                    ", [Version]" +
                    ", [OrganisationStandardId]" +
                    ", [EffectiveFrom]" +
                    ", [EffectiveTo]" +
                    ", [DateVersionApproved]" +
                    ", [Comments]" +
                    ", [Status] " +
                "FROM [OrganisationStandardVersion] " +
                $"WHERE {NotNullQueryParam(organisationStandardVersion, p => p.StandardUId)} " +
                    $"AND {NullQueryParam(organisationStandardVersion, p => p.Version)} " +
                    // when organisationStandardId is null then OrganisationStandardId is not predicated
                    $"AND OrganisationStandardId = @organisationStandardId OR @organisationStandardId IS NULL " + 
                    $"AND {NullQueryParam(organisationStandardVersion, p => p.EffectiveFrom)} " +
                    $"AND {NullQueryParam(organisationStandardVersion, p => p.EffectiveTo)} " +
                    $"AND {NullQueryParam(organisationStandardVersion, p => p.DateVersionApproved)} " +
                    $"AND {NullQueryParam(organisationStandardVersion, p => p.Comments)} " +
                    $"AND {NotNullQueryParam(organisationStandardVersion, p => p.Status)} ";

            return await DatabaseService.QueryFirstOrDefaultAsync<OrganisationStandardVersionModel, OrganisationStandardVersionModel>(sqlToQuery, organisationStandardVersion);
        }

        public static async Task<int> QueryCountAllAsync()
        {
            var sqlToQuery =
                "SELECT COUNT(1)" +
                "FROM [OrganisationStandardVersion]";

            return await DatabaseService.QueryFirstOrDefaultAsync<int>(sqlToQuery);
        }

        public static void DeleteRecord(int organisationStandardId, string standardUId)
        {
            var sqlToDelete = 
                "DELETE FROM [OrganisationStandardVersion] " +
                "WHERE OrganisationStandardId = @organisationStandardId " +
                    "AND StandardUId = @standardUId";

            DatabaseService.Execute(sqlToDelete, new { OrganisationStandardId = organisationStandardId, StandardUId = standardUId });
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM [OrganisationStandardVersion]";
            DatabaseService.Execute(sql);
        }
    }
}