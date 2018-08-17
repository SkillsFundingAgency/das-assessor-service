using System;
using System.Collections.Generic;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class OrganisationHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationModel organisation)
        {
            var sql =
                "INSERT INTO [Organisations] ([Id] ,[CreatedAt],[DeletedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId],  " +
                "[EndPointAssessorUkprn],[PrimaryContact],[Status],[UpdatedAt],[OrganisationTypeId],[OrganisationData]) VALUES " +
                $@"(@id,@createdAt, @deletedAt, @endPointAssessorName, @endPointAssessorOrganisationId, @endPointAssessorUkprn, @primaryContact, @status, @updatedAt, @organisationTypeId, @organisationData); ";

            DatabaseService.Execute(sql,organisation);
        }

        public static void InsertRecords(List<OrganisationModel> organisations)
        {
            foreach (var org in organisations)
            {
                InsertRecord(org);
            }
        }

        public static EpaOrganisation GetOrganisationByOrgId(string orgId)
        {
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
            var organisation = DatabaseService.Get<EpaOrganisation>($@"select id,createdAt,deletedAt, EndpointAssessorName as Name, EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, PrimaryContact, Status, UpdatedAt,OrganisationTypeId, OrganisationData  from Organisations where endpointassessororganisationid = '{orgId}'");
            return organisation;
        }
        public static bool EpaOrganisationExistsWithOrganisationId(string organisationId)
        {
            var sqlToCheckExists =
                    "select CONVERT(BIT,CASE count(0) WHEN 0 THEN 0 else 1 end) FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
            return (bool) DatabaseService.ExecuteScalar(sqlToCheckExists);
        }

        public static void DeleteRecord(Guid id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from Organisations where id = {idToDelete}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecordByOrganisationId(string organisationId)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(organisationId);
            var sql = $@"DELETE from Organisations where [EndPointAssessorOrganisationId] = {idToDelete}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }
    }
}

