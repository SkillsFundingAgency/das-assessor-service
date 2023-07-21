using Dapper;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OrganisationStandardHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationStandardModel organisationStandard)
        {
            var sql =
                "SET IDENTITY_INSERT [OrganisationStandard] ON; " +
                "INSERT INTO [dbo].[OrganisationStandard] " +
                    "([Id]" +
                    ", [EndPointAssessorOrganisationId]" +
                    ", [StandardCode]" +
                    ", [EffectiveFrom]" +
                    ", [EffectiveTo]" +
                    ", [DateStandardApprovedOnRegister]" +
                    ", [Comments]" +
                    ", [Status]" +
                    ", [StandardReference]) " +
                "VALUES " +
                    "(@id" +
                    ", @endPointAssessorOrganisationId" +
                    ", @standardCode" +
                    ", @effectiveFrom" +
                    ", @effectiveTo" +
                    ", @dateStandardApprovedOnRegister" +
                    ", @comments" +
                    ", @status" +
                    ", @standardReference); " +
                "SET IDENTITY_INSERT [OrganisationStandard] OFF;";
        
            DatabaseService.Execute(sql, organisationStandard);
        }

        public static OrganisationStandardModel Create(int? id, string endpointAssessorOrganisatoinId, int standardCode, DateTime effectiveFrom, DateTime? effectiveTo, 
            DateTime dateStandardApprovedOnRegister, string comments, string status, string standardReference)
        {
            return new OrganisationStandardModel
            {
                Id = id,
                EndPointAssessorOrganisationId = endpointAssessorOrganisatoinId,
                StandardCode = standardCode,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                DateStandardApprovedOnRegister = dateStandardApprovedOnRegister,
                Comments = comments,
                Status = status,
                StandardReference = standardReference
            };
        }

        public static void InsertRecords(List<OrganisationStandardModel> organisationStandards)
        {
            foreach (var organisationStandard in organisationStandards)
            {
                InsertRecord(organisationStandard);
            }
        }

        public static async Task<OrganisationStandardModel> QueryFirstOrDefaultAsync(OrganisationStandardModel organisationStandard)
        {
            var sqlToQuery =
                "SELECT " +
                    "[Id]" +
                    ", [EndPointAssessorOrganisationId]" +
                    ", [StandardCode]" +
                    ", [EffectiveFrom]" +
                    ", [EffectiveTo]" +
                    ", [DateStandardApprovedOnRegister]" +
                    ", [Comments]" +
                    ", [Status]" +
                    ", [StandardReference] " +
                "FROM [OrganisationStandard] " +
                $"WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    $"AND {NotNullQueryParam(organisationStandard, p => p.EndPointAssessorOrganisationId)} " +
                    $"AND {NotNullQueryParam(organisationStandard, p => p.StandardCode)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.EffectiveFrom)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.EffectiveTo)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.EffectiveTo)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.DateStandardApprovedOnRegister)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.Comments)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.Status)} " +
                    $"AND {NullQueryParam(organisationStandard, p => p.StandardReference)}";

            return await DatabaseService.QueryFirstOrDefaultAsync(sqlToQuery, organisationStandard);
        }

        public static void DeleteRecord(int id)
        {
            var sql = $@"DELETE FROM OrganisationStandard WHERE Id = {id}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteAllRecords()
        {
            var sql = $@"DELETE FROM OrganisationStandard";
            DatabaseService.Execute(sql);
        }
    }
}


