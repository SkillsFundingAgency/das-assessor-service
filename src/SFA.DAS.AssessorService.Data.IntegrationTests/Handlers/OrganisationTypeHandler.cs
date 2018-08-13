using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class OrganisationTypeHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationTypeModel organisationType)
        {
            var sqlToInsert = "set identity_insert [OrganisationType] ON; INSERT INTO [OrganisationType] ([Id] ,[Type],[Status]) VALUES (@id,@type, @Status); set identity_insert [OrganisationType] OFF; ";
            DatabaseService.Execute(sqlToInsert, organisationType);
        }
        public static void InsertRecords(List<OrganisationTypeModel> organisationTypes)
        {
            foreach (var orgType in organisationTypes)
            {
                InsertRecord(orgType);
            }
        }
        public static void DeleteRecord(int idToDelete)
        {
            var sql = $@"DELETE from OrganisationType where id = {idToDelete}; ";
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