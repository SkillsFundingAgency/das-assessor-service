using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Mime;
using SFA.DAS.AssessorService.Data.DapperTypeHandlers;
using SFA.DAS.AssessorService.Application.Exceptions;

namespace SFA.DAS.AssessorService.Data
{
    public class RegisterRepository : IRegisterRepository
    {

        private readonly IWebConfiguration _configuration;
        public RegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            SqlMapper.AddTypeHandler(typeof(OrganisationData), new OrganisationDataHandler());
        }

        public async Task<EpaOrganisation> CreateEpaOrganisation(EpaOrganisation org)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var orgData = JsonConvert.SerializeObject(org.OrganisationData);

                connection.Execute(
                    "INSERT INTO [Organisations] ([Id],[CreatedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
                    "[EndPointAssessorUkprn],[Status],[OrganisationTypeId],[OrganisationData]) " +
                    $@"VALUES (@id, getdate(), @name, @organisationId, @ukprn, @status, @organisationTypeId,  @orgData)",
                    new {org.Id, org.Name,org.OrganisationId, org.Ukprn,org.Status,org.OrganisationTypeId,orgData}
                );

                return await GetEpaOrganisationById(org.Id);
            }
        }

        public async Task<bool> EpaOrganisationExistsWithOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
            }
        }

        public async Task<bool> EpaOrganisationExistsWithUkprn(long ukprn)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    $@"WHERE EndPointAssessorUkprn = {ukprn}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
            }
        }

        public async Task<bool> OrganisationTypeExists(int organisationTypeId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [OrganisationType] " +
                    $@"WHERE Id = {organisationTypeId}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisationById(Guid id)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    $@"WHERE Id = '{id}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        public async Task<EpaOrganisation> GetEpaOrganisationByOrganisationId(string organisationId)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlForMainDetails =
                    "select Id, CreatedAt, DeletedAt, EndPointAssessorName as Name,  EndPointAssessorOrganisationId as OrganisationId, EndPointAssessorUkprn as ukprn, " +
                    "primaryContact, Status, UpdatedAt, OrganisationTypeId, OrganisationData " +
                    " FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId = '{organisationId}'";
                var orgs = await connection.QueryAsync<EpaOrganisation>(sqlForMainDetails);
                var org = orgs.FirstOrDefault();
                return org;
            }
        }

        public async Task<EpaOrganisation> UpdateEpaOrganisation(EpaOrganisation org)
        {
            //using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            //{
            //    if (connection.State != ConnectionState.Open)
            //        await connection.OpenAsync();

            //    var orgData = JsonConvert.SerializeObject(org.OrganisationData);

            //    connection.Execute(
            //        $@"UPDATE [Organisations] ([Id],[CreatedAt],[EndPointAssessorName],[EndPointAssessorOrganisationId], " +
            //        $@"[EndPointAssessorUkprn],[Status],[OrganisationTypeId],[OrganisationData]) WHERE [EndPointAssessorOrganisationId] = '{org.OrganisationId}'" +
            //        $@"VALUES (@id, getdate(), @name, @organisationId, @ukprn, @status, @organisationTypeId,  @orgData)",
            //        new { org.Id, org.Name, org.OrganisationId, org.Ukprn, org.Status, org.OrganisationTypeId, orgData }
            //    );

            //    return await GetEpaOrganisationById(org.Id);
            //}

            return null;
        }

        public async Task<bool> EpaOrganisationAlreadyUsingUkprn(long ukprn, string organisationIdToExclude)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();
                var sqlToCheckExists =
                    "select CASE count(0) WHEN 0 THEN 0 else 1 end result FROM [Organisations] " +
                    $@"WHERE EndPointAssessorOrganisationId != '{organisationIdToExclude}' and EndPointAssessorUkprn = {ukprn}";
                return await connection.ExecuteScalarAsync<bool>(sqlToCheckExists);
            }
        }
    }
}
