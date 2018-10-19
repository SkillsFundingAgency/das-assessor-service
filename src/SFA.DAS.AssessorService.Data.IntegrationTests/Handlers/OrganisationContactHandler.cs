using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{

    public static class OrganisationContactHandler
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(OrganisationContactModel organisationContact)
        {
            var sql =
                "INSERT INTO [dbo].[OrganisationStandard] ([Id],[DisplayName],[Email],[EndPointAssessorOrganisationId] ,[OrganisationId],[Status] ,[Username],[PhoneNumber])" +
                " VALUES (@Id, @DisplayName, @Email, @EndPointAssessorOrganisationId, @organisationId, @Status, @Username, @PhoneNumber); ";

            DatabaseService.Execute(sql, organisationContact);
        }

        public static void InsertRecords(List<OrganisationContactModel> organisationContacts)
        {
            foreach (var org in organisationContacts)
            {
                InsertRecord(org);
            }
        }

        public static void DeleteRecord(Guid id)
        {
            var idToDelete = SqlStringService.ConvertStringToSqlValueString(id.ToString());
            var sql = $@"DELETE from contacts where id = {idToDelete}";
            DatabaseService.Execute(sql);
        }

        public static void DeleteRecordByUserName(string username)
        {
            var usernameToDelete = SqlStringService.ConvertStringToSqlValueString(username);
            var sql = $@"DELETE from Contacts where [Username] = {usernameToDelete}";
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
