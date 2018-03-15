using System.Data;
using Dapper;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils
{
    public class ContactData
    {
        private readonly IDbConnection _dbConnection;

        public ContactData(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public int Insert(Contact contact)
        {
            const string command = @"INSERT INTO [dbo].[Contacts]
           ([Id]
           ,[CreatedAt]
           ,[DeletedAt]
           ,[DisplayName]
           ,[Email]
           ,[EndPointAssessorOrganisationId]
           ,[OrganisationId]
           ,[Status]
           ,[UpdatedAt]
           ,[Username])
     VALUES
           (@Id,
           @CreatedAt,
           @DeletedAt,
           @DisplayName,
           @Email,
           @EndPointAssessorOrganisationId,
           @OrganisationId,
           @Status,
           @UpdatedAt,
           @Username)";

            var result = _dbConnection.Execute(command, contact);
            return result;
        }
    }
}
