using System;
using System.Collections.Generic;
using System.Text;
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

        public static void DeleteRecord(Guid id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from Organisations where id = {idToDelete}";
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

