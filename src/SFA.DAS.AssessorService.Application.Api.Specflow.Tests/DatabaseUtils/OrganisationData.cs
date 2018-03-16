using System.Data;
using Dapper;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.DatabaseUtils
{
    public class OrganisationData
    {
        private readonly IDbConnection _dbConnection;

        public OrganisationData(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public int Insert(Organisation organisation)
        {
            const string command = @"INSERT INTO [dbo].[Organisations] 
                    ([Id],
                    [CreatedAt],
                    [DeletedAt],
                    [EndPointAssessorName],
                    [EndPointAssessorOrganisationId],
                    [EndPointAssessorUkprn],
                    [PrimaryContact],
                    [Status],
                    [UpdatedAt])
            VALUES
                (@Id,
                @CreatedAt,
                @DeletedAt,
                @EndPointAssessorName,
                @EndPointAssessorOrganisationId,
                @EndPointAssessorUkprn,
                @PrimaryContact,
                @Status,
                @UpdatedAt)";

            var result = _dbConnection.Execute(command, organisation);
            return result;
        }
    }
}
