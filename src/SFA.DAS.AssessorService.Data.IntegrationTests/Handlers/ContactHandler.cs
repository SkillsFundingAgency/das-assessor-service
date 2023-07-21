using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Handlers
{
    public class ContactsHandler : HandlerBase
    {
        private static readonly DatabaseService DatabaseService = new DatabaseService();

        public static void InsertRecord(ContactModel contact)
        {
            var sqlToInsert =
                "INSERT INTO [dbo].[Contacts] " +
                    "([Id]" +
                    ", [CreatedAt]" +
                    ", [DisplayName]" +
                    ", [Email]" +
                    ", [EndpointAssessmentOrganisationId]" +
                    ", [OrganisationId]" +
                    ", [Status]" +
                    ", [Username]) " +
                "VALUES " +
                    "(@id" +
                    ", @createdAt" +
                    ", @displayName" +
                    ", @email" +
                    ", @endpointAssessmentOrganisationId" +
                    ", @organisationId" +
                    ", @username)";

            DatabaseService.Execute(sqlToInsert, contact);
        }

        public static void InsertRecords(List<ContactModel> contacts)
        {
            foreach (var contact in contacts)
            {
                InsertRecord(contact);
            }
        }

        public static ContactModel Create(Guid? id, DateTime createdAt, string displayName, string email, string endpointAssessmentOrganisationId, 
            Guid organisationId, string status, string username)
        {
            return new ContactModel
            {
                Id = id,
                CreatedAt = createdAt,
                DisplayName = displayName,
                Email = email,
                EndPointAssessorOrganisationId = endpointAssessmentOrganisationId,
                OrganisationId = organisationId,
                Status = status,
                Username = username
            };
        }

        public static async Task<ContactModel> QuerySingleOrDefaultAsync(ContactModel contact)
        {
            var sqlToQuery =
                "SELECT " +
                    "[Id]" +
                    ", [CreatedAt]" +
                    ", [DisplayName]" +
                    ", [Email]" +
                    ", [EndpointAssessmentOrganisationId]" +
                    ", [OrganisationId]" +
                    ", [Status]" +
                    ", [Username] " +
                "FROM [Contacts] " +
                "WHERE (Id = @id OR @id IS NULL) " + // when @id is null then Id is not predicated
                    "AND CreatedAt = @createdAt " +
                    "AND DisplayName = @displayName " +
                    "AND Email = @email" +
                    "AND EndpointAssessmentOrganisationId = @endpointAssessmentOrganisationId" +
                    "AND OrganisationId = @organisationId" +
                    "AND Status = @status" +
                    "AND Username = @username";

            return await DatabaseService.QueryFirstOrDefaultAsync(sqlToQuery, contact);
        }

        public static void DeleteAllRecords()
        {
            var sql = "DELETE FROM [Contacts]";

            DatabaseService.Execute(sql);
        }
    }
}
