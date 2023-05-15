using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public static class OrganisationStandardHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStandardModel organisationStandard)
        {
            var sql =
                "set identity_insert [OrganisationStandard] ON; INSERT INTO [dbo].[OrganisationStandard] (Id, [EndPointAssessorOrganisationId],[StandardCode],[EffectiveFrom],[EffectiveTo],[DateStandardApprovedOnRegister]," +
                "[Comments],[Status]) VALUES (@id, @endPointAssessorOrganisationId, @standardCode, @effectiveFrom, @effectiveTo, @dateStandardApprovedOnRegister, @comments, @status); set identity_insert [OrganisationStandard] OFF; ";
        
            DatabaseService.Execute(sql, organisationStandard);
        }

        public static void InsertRecords(List<OrganisationStandardModel> organisationStandards)
        {
            foreach (var org in organisationStandards)
            {
                InsertRecord(org);
            }
        }

        public static EpaOrganisationStandard GetOrganisationStandardByOrgIdStandardCode(string orgId, int standardCode)
        {
            var organisationStandard = DatabaseService.Get<EpaOrganisationStandard>($@"select id,EndPointAssessorOrganisationId as OrganisationId, StandardCode, EffectiveFrom, EffectiveTo, DateStandardApprovedOnRegister, Comments, Status  from OrganisationStandard where endpointassessororganisationid = '{orgId}' and standardCode = {standardCode}");
            return organisationStandard;
        }


        public static void DeleteRecord(int id)
        {
            //var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from OrganisationStandard where id = {id}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecordByOrganisationIdStandardCode(string organisationId, int standardCode)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(organisationId);
            var sql = $@"DELETE from Organisations where [EndPointAssessorOrganisationId] = {idToDelete} and StandardCode = {standardCode}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecords(List<int> ids)
        {
            foreach (var id in ids)
            {
                DeleteRecord(id);
            }
        }
    }
}


