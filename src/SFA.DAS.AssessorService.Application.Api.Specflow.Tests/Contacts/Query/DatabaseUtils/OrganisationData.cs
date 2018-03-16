using System;
using System.Data;
using System.Linq;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models;
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

        public Guid GetId(string endPointAssessorOrganisationId)
        {
            var ids = _dbConnection.Query<Guid>
                ($"Select Id From Organisations where EndPointAssessorOrganisationId = {endPointAssessorOrganisationId}").ToList();
            return ids.First();
        }
    }
}
